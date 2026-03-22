using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Edge;

namespace DoAnCuoiKy_TH.Utilities
{
    public class DriverFactory
    {
        protected IWebDriver driver = null!;

        private const string BaseUrl = "https://parabank.parasoft.com/parabank/index.htm";

        private const int ImplicitWaitSeconds = 5;

        [SetUp]
        public void Setup()
        {
            var options = new EdgeOptions();
            options.AddArgument("start-maximized");

            driver = new EdgeDriver(options);
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(ImplicitWaitSeconds);
            driver.Navigate().GoToUrl(BaseUrl);
        }

        [TearDown]
        public void TearDown()
        {
            if (driver != null)
            {
                try
                {
                    driver.Quit();
                }
                finally
                {
                    driver.Dispose();
                }
            }
        }
    }
}