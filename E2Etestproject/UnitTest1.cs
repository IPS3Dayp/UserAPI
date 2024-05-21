using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace E2Etestproject
{
    public class Tests
    {
        private IWebDriver _driver;
        private const string BaseUrl = "https://jouwapplicatieurl.com";

        [SetUp]
        public void Setup()
        {
            _driver = new ChromeDriver();
        }

        [TearDown]
        public void TearDown()
        {
            _driver.Quit();
            _driver.Dispose();
        }

        [Test]
        public void UserCanLoginSuccessfully()
        {
            // Navigeer naar de inlogpagina
            _driver.Navigate().GoToUrl($"{BaseUrl}/login");

            // Vul inloggegevens in
            _driver.FindElement(By.Id("username")).SendKeys("johndoe");
            _driver.FindElement(By.Id("password")).SendKeys("password123");
            _driver.FindElement(By.Id("login-button")).Click();

            // Controleer of de gebruiker is ingelogd door de URL van de dashboardpagina te verifiëren
            Assert.AreEqual($"{BaseUrl}/dashboard", _driver.Url);
        }
    }
}
