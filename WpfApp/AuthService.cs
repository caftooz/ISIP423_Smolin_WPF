using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp
{
    public static class AuthService
    {
        public static bool Auth(string login, string password)
        {
            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
                return false;

            if (Core.Context.Users.Any(u => u.Login == login && u.Password == password))
            {
                Core.UserID = Core.Context.Users
                    .First(u => u.Login == login && u.Password == password).Id;
                return true;
            }

            return false;
        }
    }
}
