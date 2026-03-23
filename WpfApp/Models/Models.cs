namespace ServiceDesk.Models;

// ─── Пользователь ───────────────────────────────────────────────
public class User
{
    public int     Id         { get; set; }
    public string  Login      { get; set; } = "";
    public string  Password   { get; set; } = "";
    public string  Role       { get; set; } = "";
    public int?    EmployeeId { get; set; }
    public bool    IsActive   { get; set; } = true;

    // Из JOIN
    public string  EmployeeName { get; set; } = "";

    public string RoleDisplay => Role switch
    {
        "admin"    => "Администратор",
        "manager"  => "Менеджер",
        "executor" => "Исполнитель",
        _          => Role
    };
}

// ─── Сотрудник ──────────────────────────────────────────────────
public class Employee
{
    public int    Id       { get; set; }
    public string FullName { get; set; } = "";
    public string Phone    { get; set; } = "";
    public string Position { get; set; } = "";

    public override string ToString() => FullName;
}

// ─── Тип техники ────────────────────────────────────────────────
public class EquipmentType
{
    public int    Id   { get; set; }
    public string Name { get; set; } = "";
    public override string ToString() => Name;
}

// ─── Статус заявки ──────────────────────────────────────────────
public class RequestStatus
{
    public int    Id   { get; set; }
    public string Name { get; set; } = "";
    public override string ToString() => Name;
}

// ─── Приоритет ──────────────────────────────────────────────────
public class Priority
{
    public int    Id    { get; set; }
    public string Name  { get; set; } = "";
    public int    Level { get; set; }
    public override string ToString() => Name;
}

// ─── Заявка ─────────────────────────────────────────────────────
public class Request
{
    public int       Id                { get; set; }
    public string    Number            { get; set; } = "";
    public DateTime  CreatedDate       { get; set; }
    public string    ClientName        { get; set; } = "";
    public string    ClientPhone       { get; set; } = "";
    public int?      EquipmentTypeId   { get; set; }
    public string    FaultDescription  { get; set; } = "";
    public int?      PriorityId        { get; set; }
    public int?      StatusId          { get; set; }
    public int?      ExecutorId        { get; set; }
    public DateTime? CompletionDate    { get; set; }
    public string    RepairComment     { get; set; } = "";
    public int?      CreatedBy         { get; set; }

    // Из JOIN
    public string EquipmentTypeName { get; set; } = "";
    public string PriorityName      { get; set; } = "";
    public int    PriorityLevel     { get; set; }
    public string StatusName        { get; set; } = "";
    public string ExecutorName      { get; set; } = "";

    public string CreatedDateStr    => CreatedDate.ToString("dd.MM.yyyy HH:mm");
    public string CompletionDateStr => CompletionDate?.ToString("dd.MM.yyyy") ?? "—";
}
