using System;

namespace WpfApp
{
    public static class SessionManager
    {
        public static Users CurrentUser { get; private set; }
        public static bool IsLoggedIn => CurrentUser != null;
        public static event Action OnLogin;

        public static void Login(Users user) { CurrentUser = user; OnLogin?.Invoke(); }
        public static void Logout() { CurrentUser = null; OnLogin?.Invoke(); }
    }
}
