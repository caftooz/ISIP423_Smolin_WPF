using System;
using System.Linq;

namespace WpfApp
{
    public static class RegistrationService
    {
        public static string Register(string login, string password, string passwordRepeat, string email)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(login) ||
                string.IsNullOrEmpty(password) || string.IsNullOrEmpty(passwordRepeat))
            {
                return "Все поля должны быть заполнены";
            }

            if (Core.Context.Users.Any(u => u.Email == email))
            {
                return "Пользователь с такой почтой уже существует";
            }

            if (Core.Context.Users.Any(u => u.Login == login))
            {
                return "Пользователь с таким логином уже существует";
            }

            if (password != passwordRepeat)
            {
                return "Пароли не совпадают";
            }

            Users newUser = new Users()
            {
                Login = login,
                Email = email,
                Password = password
            };
            Core.Context.Users.Add(newUser);
            Core.Context.SaveChanges();

            Core.UserID = newUser.Id;
            return null; // null означает успех
        }
    }
}
