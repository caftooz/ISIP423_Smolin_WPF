using System.Security.Cryptography;
using System.Text;
using Npgsql;
using ServiceDesk.Models;

namespace ServiceDesk.Data;

public static class DatabaseHelper
{
    // ── Строка подключения ────────────────────────────────────────
    private static string _connectionString =
        "Host=localhost;Port=5432;Database=servicedesk;Username=postgres;Password=0909";

    public static void SetConnectionString(string cs) => _connectionString = cs;

    private static NpgsqlConnection GetConnection()
    {
        var conn = new NpgsqlConnection(_connectionString);
        conn.Open();
        return conn;
    }

    // ── Хэш пароля ───────────────────────────────────────────────
    public static string HashPassword(string password)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(password));
        return Convert.ToHexString(bytes).ToLower();
    }

    // ── Проверка соединения ───────────────────────────────────────
    public static bool TestConnection(out string error)
    {
        error = "";
        try { using var c = GetConnection(); return true; }
        catch (Exception ex) { error = ex.Message; return false; }
    }

    // ═══════════════════════════════════════════════════════════════
    //  АВТОРИЗАЦИЯ
    // ═══════════════════════════════════════════════════════════════
    public static User? Authenticate(string login, string password)
    {
        var hash = HashPassword(password);
        using var conn = GetConnection();
        using var cmd  = new NpgsqlCommand(@"
            SELECT u.id, u.login, u.role, u.employee_id, u.is_active,
                   COALESCE(e.full_name,'') AS emp_name
            FROM   users u
            LEFT JOIN employees e ON e.id = u.employee_id
            WHERE  u.login = @l AND u.password = @p AND u.is_active = TRUE", conn);
        cmd.Parameters.AddWithValue("l", login);
        cmd.Parameters.AddWithValue("p", hash);
        using var r = cmd.ExecuteReader();
        if (!r.Read()) return null;
        return new User
        {
            Id           = r.GetInt32(0),
            Login        = r.GetString(1),
            Role         = r.GetString(2),
            EmployeeId   = r.IsDBNull(3) ? null : r.GetInt32(3),
            IsActive     = r.GetBoolean(4),
            EmployeeName = r.GetString(5)
        };
    }

    // ═══════════════════════════════════════════════════════════════
    //  ЗАЯВКИ
    // ═══════════════════════════════════════════════════════════════
    public static List<Request> GetRequests(int? executorFilter = null,
                                            string? search      = null)
    {
        var list = new List<Request>();
        using var conn = GetConnection();

        var sql = @"
            SELECT r.id, r.number, r.created_date, r.client_name, r.client_phone,
                   r.equipment_type_id, COALESCE(et.name,''),
                   r.fault_description,
                   r.priority_id, COALESCE(p.name,''), COALESCE(p.level,0),
                   r.status_id,   COALESCE(s.name,''),
                   r.executor_id, COALESCE(e.full_name,''),
                   r.completion_date, COALESCE(r.repair_comment,''),
                   r.created_by
            FROM   requests r
            LEFT JOIN equipment_types et ON et.id = r.equipment_type_id
            LEFT JOIN priorities       p  ON p.id  = r.priority_id
            LEFT JOIN request_statuses s  ON s.id  = r.status_id
            LEFT JOIN employees        e  ON e.id  = r.executor_id
            WHERE 1=1 ";

        if (executorFilter.HasValue) sql += " AND r.executor_id = @ex ";
        if (!string.IsNullOrWhiteSpace(search))
            sql += " AND (LOWER(r.number) LIKE @q OR LOWER(r.client_name) LIKE @q) ";
        sql += " ORDER BY r.created_date DESC";

        using var cmd = new NpgsqlCommand(sql, conn);
        if (executorFilter.HasValue)
            cmd.Parameters.AddWithValue("ex", executorFilter.Value);
        if (!string.IsNullOrWhiteSpace(search))
            cmd.Parameters.AddWithValue("q", $"%{search.ToLower()}%");

        using var rd = cmd.ExecuteReader();
        while (rd.Read())
        {
            list.Add(new Request
            {
                Id                = rd.GetInt32(0),
                Number            = rd.GetString(1),
                CreatedDate       = rd.GetDateTime(2),
                ClientName        = rd.GetString(3),
                ClientPhone       = rd.GetString(4),
                EquipmentTypeId   = rd.IsDBNull(5) ? null : rd.GetInt32(5),
                EquipmentTypeName = rd.GetString(6),
                FaultDescription  = rd.GetString(7),
                PriorityId        = rd.IsDBNull(8) ? null : rd.GetInt32(8),
                PriorityName      = rd.GetString(9),
                PriorityLevel     = rd.GetInt32(10),
                StatusId          = rd.IsDBNull(11) ? null : rd.GetInt32(11),
                StatusName        = rd.GetString(12),
                ExecutorId        = rd.IsDBNull(13) ? null : rd.GetInt32(13),
                ExecutorName      = rd.GetString(14),
                CompletionDate    = rd.IsDBNull(15) ? null : rd.GetDateTime(15),
                RepairComment     = rd.GetString(16),
                CreatedBy         = rd.IsDBNull(17) ? null : rd.GetInt32(17)
            });
        }
        return list;
    }

    public static Request? GetRequest(int id)
    {
        return GetRequests().FirstOrDefault(r => r.Id == id);
    }

    public static string GetNextNumber()
    {
        using var conn = GetConnection();
        using var cmd  = new NpgsqlCommand("SELECT next_request_number()", conn);
        return cmd.ExecuteScalar()?.ToString() ?? "ЗЯВ-0000";
    }

    public static void SaveRequest(Request req)
    {
        using var conn = GetConnection();
        if (req.Id == 0)
        {
            using var cmd = new NpgsqlCommand(@"
                INSERT INTO requests
                    (number, created_date, client_name, client_phone,
                     equipment_type_id, fault_description, priority_id,
                     status_id, executor_id, completion_date, repair_comment, created_by)
                VALUES (@num,@cd,@cn,@cp,@et,@fd,@pr,@st,@ex,@compd,@rc,@cb)", conn);
            AddRequestParams(cmd, req);
            cmd.ExecuteNonQuery();
        }
        else
        {
            using var cmd = new NpgsqlCommand(@"
                UPDATE requests SET
                    client_name=@cn, client_phone=@cp,
                    equipment_type_id=@et, fault_description=@fd,
                    priority_id=@pr, status_id=@st, executor_id=@ex,
                    completion_date=@compd, repair_comment=@rc
                WHERE id=@id", conn);
            AddRequestParams(cmd, req);
            cmd.Parameters.AddWithValue("id", req.Id);
            cmd.ExecuteNonQuery();
        }
    }

    private static void AddRequestParams(NpgsqlCommand cmd, Request r)
    {
        cmd.Parameters.AddWithValue("num",   r.Number);
        cmd.Parameters.AddWithValue("cd",    r.CreatedDate);
        cmd.Parameters.AddWithValue("cn",    r.ClientName);
        cmd.Parameters.AddWithValue("cp",    (object?)r.ClientPhone ?? DBNull.Value);
        cmd.Parameters.AddWithValue("et",    (object?)r.EquipmentTypeId ?? DBNull.Value);
        cmd.Parameters.AddWithValue("fd",    (object?)r.FaultDescription ?? DBNull.Value);
        cmd.Parameters.AddWithValue("pr",    (object?)r.PriorityId ?? DBNull.Value);
        cmd.Parameters.AddWithValue("st",    (object?)r.StatusId ?? DBNull.Value);
        cmd.Parameters.AddWithValue("ex",    (object?)r.ExecutorId ?? DBNull.Value);
        cmd.Parameters.AddWithValue("compd", (object?)r.CompletionDate ?? DBNull.Value);
        cmd.Parameters.AddWithValue("rc",    (object?)r.RepairComment ?? DBNull.Value);
        cmd.Parameters.AddWithValue("cb",    (object?)r.CreatedBy ?? DBNull.Value);
    }

    public static void DeleteRequest(int id)
    {
        using var conn = GetConnection();
        using var cmd  = new NpgsqlCommand("DELETE FROM requests WHERE id=@id", conn);
        cmd.Parameters.AddWithValue("id", id);
        cmd.ExecuteNonQuery();
    }

    public static void UpdateRequestStatus(int requestId, int statusId)
    {
        using var conn = GetConnection();
        using var cmd  = new NpgsqlCommand(
            "UPDATE requests SET status_id=@s WHERE id=@id", conn);
        cmd.Parameters.AddWithValue("s",  statusId);
        cmd.Parameters.AddWithValue("id", requestId);
        cmd.ExecuteNonQuery();
    }

    // ═══════════════════════════════════════════════════════════════
    //  СПРАВОЧНИКИ
    // ═══════════════════════════════════════════════════════════════

    // -- Типы техники --
    public static List<EquipmentType> GetEquipmentTypes()
    {
        var list = new List<EquipmentType>();
        using var conn = GetConnection();
        using var cmd  = new NpgsqlCommand("SELECT id,name FROM equipment_types ORDER BY name", conn);
        using var r    = cmd.ExecuteReader();
        while (r.Read()) list.Add(new EquipmentType { Id = r.GetInt32(0), Name = r.GetString(1) });
        return list;
    }
    public static void SaveEquipmentType(EquipmentType et)
    {
        using var conn = GetConnection();
        if (et.Id == 0)
        {
            using var cmd = new NpgsqlCommand("INSERT INTO equipment_types(name) VALUES(@n)", conn);
            cmd.Parameters.AddWithValue("n", et.Name); cmd.ExecuteNonQuery();
        }
        else
        {
            using var cmd = new NpgsqlCommand("UPDATE equipment_types SET name=@n WHERE id=@id", conn);
            cmd.Parameters.AddWithValue("n", et.Name); cmd.Parameters.AddWithValue("id", et.Id);
            cmd.ExecuteNonQuery();
        }
    }
    public static void DeleteEquipmentType(int id)
    {
        using var conn = GetConnection();
        using var cmd  = new NpgsqlCommand("DELETE FROM equipment_types WHERE id=@id", conn);
        cmd.Parameters.AddWithValue("id", id); cmd.ExecuteNonQuery();
    }

    // -- Статусы --
    public static List<RequestStatus> GetStatuses()
    {
        var list = new List<RequestStatus>();
        using var conn = GetConnection();
        using var cmd  = new NpgsqlCommand("SELECT id,name FROM request_statuses ORDER BY id", conn);
        using var r    = cmd.ExecuteReader();
        while (r.Read()) list.Add(new RequestStatus { Id = r.GetInt32(0), Name = r.GetString(1) });
        return list;
    }
    public static void SaveStatus(RequestStatus s)
    {
        using var conn = GetConnection();
        if (s.Id == 0)
        {
            using var cmd = new NpgsqlCommand("INSERT INTO request_statuses(name) VALUES(@n)", conn);
            cmd.Parameters.AddWithValue("n", s.Name); cmd.ExecuteNonQuery();
        }
        else
        {
            using var cmd = new NpgsqlCommand("UPDATE request_statuses SET name=@n WHERE id=@id", conn);
            cmd.Parameters.AddWithValue("n", s.Name); cmd.Parameters.AddWithValue("id", s.Id);
            cmd.ExecuteNonQuery();
        }
    }
    public static void DeleteStatus(int id)
    {
        using var conn = GetConnection();
        using var cmd  = new NpgsqlCommand("DELETE FROM request_statuses WHERE id=@id", conn);
        cmd.Parameters.AddWithValue("id", id); cmd.ExecuteNonQuery();
    }

    // -- Приоритеты --
    public static List<Priority> GetPriorities()
    {
        var list = new List<Priority>();
        using var conn = GetConnection();
        using var cmd  = new NpgsqlCommand("SELECT id,name,level FROM priorities ORDER BY level", conn);
        using var r    = cmd.ExecuteReader();
        while (r.Read()) list.Add(new Priority { Id = r.GetInt32(0), Name = r.GetString(1), Level = r.GetInt32(2) });
        return list;
    }
    public static void SavePriority(Priority p)
    {
        using var conn = GetConnection();
        if (p.Id == 0)
        {
            using var cmd = new NpgsqlCommand("INSERT INTO priorities(name,level) VALUES(@n,@l)", conn);
            cmd.Parameters.AddWithValue("n", p.Name); cmd.Parameters.AddWithValue("l", p.Level);
            cmd.ExecuteNonQuery();
        }
        else
        {
            using var cmd = new NpgsqlCommand("UPDATE priorities SET name=@n,level=@l WHERE id=@id", conn);
            cmd.Parameters.AddWithValue("n", p.Name); cmd.Parameters.AddWithValue("l", p.Level);
            cmd.Parameters.AddWithValue("id", p.Id); cmd.ExecuteNonQuery();
        }
    }
    public static void DeletePriority(int id)
    {
        using var conn = GetConnection();
        using var cmd  = new NpgsqlCommand("DELETE FROM priorities WHERE id=@id", conn);
        cmd.Parameters.AddWithValue("id", id); cmd.ExecuteNonQuery();
    }

    // -- Сотрудники --
    public static List<Employee> GetEmployees()
    {
        var list = new List<Employee>();
        using var conn = GetConnection();
        using var cmd  = new NpgsqlCommand(
            "SELECT id,full_name,COALESCE(phone,''),COALESCE(position,'') FROM employees ORDER BY full_name", conn);
        using var r = cmd.ExecuteReader();
        while (r.Read()) list.Add(new Employee
        {
            Id       = r.GetInt32(0),
            FullName = r.GetString(1),
            Phone    = r.GetString(2),
            Position = r.GetString(3)
        });
        return list;
    }
    public static void SaveEmployee(Employee e)
    {
        using var conn = GetConnection();
        if (e.Id == 0)
        {
            using var cmd = new NpgsqlCommand(
                "INSERT INTO employees(full_name,phone,position) VALUES(@n,@p,@pos)", conn);
            cmd.Parameters.AddWithValue("n", e.FullName);
            cmd.Parameters.AddWithValue("p",   (object?)e.Phone    ?? DBNull.Value);
            cmd.Parameters.AddWithValue("pos", (object?)e.Position ?? DBNull.Value);
            cmd.ExecuteNonQuery();
        }
        else
        {
            using var cmd = new NpgsqlCommand(
                "UPDATE employees SET full_name=@n,phone=@p,position=@pos WHERE id=@id", conn);
            cmd.Parameters.AddWithValue("n",   e.FullName);
            cmd.Parameters.AddWithValue("p",   (object?)e.Phone    ?? DBNull.Value);
            cmd.Parameters.AddWithValue("pos", (object?)e.Position ?? DBNull.Value);
            cmd.Parameters.AddWithValue("id",  e.Id);
            cmd.ExecuteNonQuery();
        }
    }
    public static void DeleteEmployee(int id)
    {
        using var conn = GetConnection();
        using var cmd  = new NpgsqlCommand("DELETE FROM employees WHERE id=@id", conn);
        cmd.Parameters.AddWithValue("id", id); cmd.ExecuteNonQuery();
    }

    // ═══════════════════════════════════════════════════════════════
    //  ПОЛЬЗОВАТЕЛИ
    // ═══════════════════════════════════════════════════════════════
    public static List<User> GetUsers()
    {
        var list = new List<User>();
        using var conn = GetConnection();
        using var cmd  = new NpgsqlCommand(@"
            SELECT u.id, u.login, u.role, u.is_active,
                   u.employee_id, COALESCE(e.full_name,'')
            FROM users u LEFT JOIN employees e ON e.id=u.employee_id
            ORDER BY u.login", conn);
        using var r = cmd.ExecuteReader();
        while (r.Read()) list.Add(new User
        {
            Id           = r.GetInt32(0),
            Login        = r.GetString(1),
            Role         = r.GetString(2),
            IsActive     = r.GetBoolean(3),
            EmployeeId   = r.IsDBNull(4) ? null : r.GetInt32(4),
            EmployeeName = r.GetString(5)
        });
        return list;
    }
    public static void SaveUser(User u, string? plainPassword = null)
    {
        using var conn = GetConnection();
        if (u.Id == 0)
        {
            using var cmd = new NpgsqlCommand(@"
                INSERT INTO users(login,password,role,employee_id,is_active)
                VALUES(@l,@pw,@r,@e,@a)", conn);
            cmd.Parameters.AddWithValue("l",  u.Login);
            cmd.Parameters.AddWithValue("pw", HashPassword(plainPassword ?? "changeme"));
            cmd.Parameters.AddWithValue("r",  u.Role);
            cmd.Parameters.AddWithValue("e",  (object?)u.EmployeeId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("a",  u.IsActive);
            cmd.ExecuteNonQuery();
        }
        else
        {
            if (!string.IsNullOrWhiteSpace(plainPassword))
            {
                using var cmd2 = new NpgsqlCommand(
                    "UPDATE users SET login=@l,password=@pw,role=@r,employee_id=@e,is_active=@a WHERE id=@id", conn);
                cmd2.Parameters.AddWithValue("l",  u.Login);
                cmd2.Parameters.AddWithValue("pw", HashPassword(plainPassword));
                cmd2.Parameters.AddWithValue("r",  u.Role);
                cmd2.Parameters.AddWithValue("e",  (object?)u.EmployeeId ?? DBNull.Value);
                cmd2.Parameters.AddWithValue("a",  u.IsActive);
                cmd2.Parameters.AddWithValue("id", u.Id);
                cmd2.ExecuteNonQuery();
            }
            else
            {
                using var cmd2 = new NpgsqlCommand(
                    "UPDATE users SET login=@l,role=@r,employee_id=@e,is_active=@a WHERE id=@id", conn);
                cmd2.Parameters.AddWithValue("l",  u.Login);
                cmd2.Parameters.AddWithValue("r",  u.Role);
                cmd2.Parameters.AddWithValue("e",  (object?)u.EmployeeId ?? DBNull.Value);
                cmd2.Parameters.AddWithValue("a",  u.IsActive);
                cmd2.Parameters.AddWithValue("id", u.Id);
                cmd2.ExecuteNonQuery();
            }
        }
    }
    public static void DeleteUser(int id)
    {
        using var conn = GetConnection();
        using var cmd  = new NpgsqlCommand("DELETE FROM users WHERE id=@id", conn);
        cmd.Parameters.AddWithValue("id", id); cmd.ExecuteNonQuery();
    }
}
