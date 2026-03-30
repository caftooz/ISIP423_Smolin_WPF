using Microsoft.VisualStudio.TestTools.UnitTesting;
using WpfApp;

namespace UnitTestProject
{
    [TestClass]
    public class AuthUnitTest
    {
        [TestMethod]
        public void AuthTest()
        {
            Assert.IsTrue(AuthService.Auth("Alex", "Alex291207"));
            Assert.IsFalse(AuthService.Auth("test", "test"));
            Assert.IsFalse(AuthService.Auth("", ""));
            Assert.IsFalse(AuthService.Auth(" ", " "));
        }

        [TestMethod]
        public void AuthTestSuccess()
        {
            // Позитивные тесты: все пользователи из БД должны успешно авторизоваться
            Assert.IsTrue(AuthService.Auth("Alex", "Alex291207"));
            Assert.IsTrue(AuthService.Auth("uesr", "us123"));
            Assert.IsTrue(AuthService.Auth("fufa", "00("));
            Assert.IsTrue(AuthService.Auth("ooooooooooooooooooooooooooooooooooop", "sdjksydfnh8oi475092097*(&%^jnos8923764)"));
        }

        [TestMethod]
        public void AuthTestFail()
        {
            // Негативные тесты: авторизация не должна проходить

            // Пустые поля (TC_BOUND_NEG_007)
            Assert.IsFalse(AuthService.Auth("", ""));
            Assert.IsFalse(AuthService.Auth("", "Alex291207"));
            Assert.IsFalse(AuthService.Auth("Alex", ""));

            // Пробелы вместо данных (TC_BOUND_NEG_007)
            Assert.IsFalse(AuthService.Auth(" ", " "));
            Assert.IsFalse(AuthService.Auth("   ", "   "));

            // Несуществующий логин (TC_FUNC_NEG_012)
            Assert.IsFalse(AuthService.Auth("nonexistent", "password123"));
            Assert.IsFalse(AuthService.Auth("notfound@fake.com", "test"));

            // Неверный пароль при существующем логине
            Assert.IsFalse(AuthService.Auth("Alex", "wrongpassword"));
            Assert.IsFalse(AuthService.Auth("uesr", "wrongpass"));

            // Null значения
            Assert.IsFalse(AuthService.Auth(null, null));
            Assert.IsFalse(AuthService.Auth(null, "Alex291207"));
            Assert.IsFalse(AuthService.Auth("Alex", null));
        }
    }
}
