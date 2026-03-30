using Microsoft.VisualStudio.TestTools.UnitTesting;
using WpfApp;
using System.Linq;

namespace UnitTestProject
{
    [TestClass]
    public class RegistrUnitTest
    {
        [TestMethod]
        public void RegisterTestSuccess()
        {
            // Позитивный тест: регистрация нового пользователя (TC_FUNC_POS_002)
            string login = "TestUser_" + System.Guid.NewGuid().ToString().Substring(0, 8);
            string email = login + "@test.com";
            string password = "TestPass123";

            string result = RegistrationService.Register(login, password, password, email);
            Assert.IsNull(result, "Регистрация должна быть успешной");

            // Проверяем, что пользователь добавлен в БД
            Assert.IsTrue(Core.Context.Users.Any(u => u.Login == login && u.Email == email));

            // Удаляем тестового пользователя из БД
            var user = Core.Context.Users.First(u => u.Login == login);
            Core.Context.Users.Remove(user);
            Core.Context.SaveChanges();
        }

        [TestMethod]
        public void RegisterTestFail()
        {
            // Негативный тест 1: пустые поля (TC_BOUND_NEG_007)
            Assert.IsNotNull(RegistrationService.Register("", "", "", ""));
            Assert.IsNotNull(RegistrationService.Register("", "pass", "pass", ""));
            Assert.IsNotNull(RegistrationService.Register("login", "", "", "email@test.com"));

            // Негативный тест 2: null значения
            Assert.IsNotNull(RegistrationService.Register(null, null, null, null));
            Assert.IsNotNull(RegistrationService.Register(null, "pass", "pass", "email@test.com"));

            // Негативный тест 3: пароли не совпадают
            Assert.IsNotNull(RegistrationService.Register("newuser", "pass1", "pass2", "new@test.com"));

            // Негативный тест 4: дублирующийся логин (TC_VAL_NEG_005)
            Assert.IsNotNull(RegistrationService.Register("Alex", "pass123", "pass123", "unique@test.com"));

            // Негативный тест 5: дублирующийся email
            Assert.IsNotNull(RegistrationService.Register("uniquelogin", "pass123", "pass123", "alexandersmolin07@gmail.com"));
        }
    }
}
