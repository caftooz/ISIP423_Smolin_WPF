using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp
{
    public static class SessionManager
    {
        public static Users CurrentUser { get; private set; }
        public static bool IsLoggedIn => CurrentUser != null;

        public static void Login(Users user) => CurrentUser = user;
        public static void Logout() => CurrentUser = null;
    }
}
