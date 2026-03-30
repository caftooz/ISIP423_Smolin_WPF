using Microsoft.VisualStudio.TestTools.UnitTesting;
using WpfApp;

namespace UnitTestProject
{
    [TestClass]
    public class CaptchaUnitTest
    {
        [TestMethod]
        public void CaptchaGenerateTest()
        {
            // Проверка генерации капчи
            var captcha = new CaptchaService();
            string code = captcha.GenerateCaptcha();

            Assert.IsNotNull(code);
            Assert.AreEqual(5, code.Length);
        }

        [TestMethod]
        public void CaptchaGenerateUniqueTest()
        {
            // Проверка уникальности генерации
            var captcha = new CaptchaService();
            string code1 = captcha.GenerateCaptcha();
            string code2 = captcha.GenerateCaptcha();

            Assert.AreNotEqual(code1, code2);
        }

        [TestMethod]
        public void CaptchaValidateSuccessTest()
        {
            // Позитивный тест: правильный ввод капчи
            var captcha = new CaptchaService();
            string code = captcha.GenerateCaptcha();

            Assert.IsTrue(captcha.ValidateCaptcha(code));
        }

        [TestMethod]
        public void CaptchaValidateFailWrongInputTest()
        {
            // Негативный тест: неверный ввод
            var captcha = new CaptchaService();
            captcha.GenerateCaptcha();

            Assert.IsFalse(captcha.ValidateCaptcha("XXXXX"));
        }

        [TestMethod]
        public void CaptchaValidateFailEmptyInputTest()
        {
            // Негативный тест: пустой ввод
            var captcha = new CaptchaService();
            captcha.GenerateCaptcha();

            Assert.IsFalse(captcha.ValidateCaptcha(""));
            Assert.IsFalse(captcha.ValidateCaptcha(null));
        }

        [TestMethod]
        public void CaptchaValidateFailNoCaptchaGeneratedTest()
        {
            // Негативный тест: капча не была сгенерирована
            var captcha = new CaptchaService();

            Assert.IsFalse(captcha.ValidateCaptcha("test"));
        }

        [TestMethod]
        public void CaptchaValidateCaseSensitiveTest()
        {
            // Проверка чувствительности к регистру
            var captcha = new CaptchaService();
            string code = captcha.GenerateCaptcha();

            if (code != code.ToUpper())
                Assert.IsFalse(captcha.ValidateCaptcha(code.ToUpper()));
            if (code != code.ToLower())
                Assert.IsFalse(captcha.ValidateCaptcha(code.ToLower()));
        }

        [TestMethod]
        public void CaptchaFailedAttemptsCounterTest()
        {
            // Проверка счётчика неудачных попыток
            var captcha = new CaptchaService();

            Assert.AreEqual(0, captcha.FailedAttempts);
            Assert.IsFalse(captcha.IsCaptchaRequired);

            captcha.IncrementFailedAttempts();
            Assert.AreEqual(1, captcha.FailedAttempts);
            Assert.IsFalse(captcha.IsCaptchaRequired);

            captcha.IncrementFailedAttempts();
            Assert.AreEqual(2, captcha.FailedAttempts);
            Assert.IsFalse(captcha.IsCaptchaRequired);

            captcha.IncrementFailedAttempts();
            Assert.AreEqual(3, captcha.FailedAttempts);
            Assert.IsTrue(captcha.IsCaptchaRequired);
        }

        [TestMethod]
        public void CaptchaResetFailedAttemptsTest()
        {
            // Проверка сброса счётчика
            var captcha = new CaptchaService();
            captcha.IncrementFailedAttempts();
            captcha.IncrementFailedAttempts();
            captcha.IncrementFailedAttempts();

            Assert.IsTrue(captcha.IsCaptchaRequired);

            captcha.ResetFailedAttempts();
            Assert.AreEqual(0, captcha.FailedAttempts);
            Assert.IsFalse(captcha.IsCaptchaRequired);
        }

        [TestMethod]
        public void CaptchaNotRequiredBeforeThreeFailsTest()
        {
            // Капча не нужна до 3 неудачных попыток
            var captcha = new CaptchaService();
            Assert.IsFalse(captcha.IsCaptchaRequired);

            captcha.IncrementFailedAttempts();
            Assert.IsFalse(captcha.IsCaptchaRequired);

            captcha.IncrementFailedAttempts();
            Assert.IsFalse(captcha.IsCaptchaRequired);
        }

        [TestMethod]
        public void CaptchaRequiredAfterThreeFailsTest()
        {
            // Капча требуется после 3 неудачных попыток
            var captcha = new CaptchaService();
            captcha.IncrementFailedAttempts();
            captcha.IncrementFailedAttempts();
            captcha.IncrementFailedAttempts();

            Assert.IsTrue(captcha.IsCaptchaRequired);
        }

        [TestMethod]
        public void CaptchaRequiredAfterMoreThanThreeFailsTest()
        {
            // Капча продолжает требоваться при >3 попытках
            var captcha = new CaptchaService();
            for (int i = 0; i < 10; i++)
                captcha.IncrementFailedAttempts();

            Assert.IsTrue(captcha.IsCaptchaRequired);
        }

        [TestMethod]
        public void CaptchaRegenerateAfterValidationTest()
        {
            // После регенерации старый код не работает
            var captcha = new CaptchaService();
            string oldCode = captcha.GenerateCaptcha();
            string newCode = captcha.GenerateCaptcha();

            Assert.IsTrue(captcha.ValidateCaptcha(newCode));
            if (oldCode != newCode)
                Assert.IsFalse(captcha.ValidateCaptcha(oldCode));
        }

        [TestMethod]
        public void CaptchaGetCurrentTest()
        {
            // Проверка получения текущей капчи
            var captcha = new CaptchaService();
            Assert.IsNull(captcha.GetCurrentCaptcha());

            string code = captcha.GenerateCaptcha();
            Assert.AreEqual(code, captcha.GetCurrentCaptcha());
        }

        [TestMethod]
        public void CaptchaLengthAlwaysFiveTest()
        {
            // Проверка длины капчи при множественной генерации
            var captcha = new CaptchaService();
            for (int i = 0; i < 50; i++)
            {
                string code = captcha.GenerateCaptcha();
                Assert.AreEqual(5, code.Length);
            }
        }

        [TestMethod]
        public void CaptchaContainsOnlyValidCharsTest()
        {
            // Проверка, что капча содержит только допустимые символы
            var captcha = new CaptchaService();
            string validChars = "ABCDEFGHJKLMNPQRSTUVWXYZabcdefghjkmnpqrstuvwxyz23456789";

            for (int i = 0; i < 50; i++)
            {
                string code = captcha.GenerateCaptcha();
                foreach (char c in code)
                {
                    Assert.IsTrue(validChars.Contains(c.ToString()),
                        $"Символ '{c}' не входит в допустимый набор");
                }
            }
        }
    }
}
