using OpenQA.Selenium;

namespace DoAnCuoiKy_TH.Pages
{
    public class TransferFundPage
    {
        private readonly IWebDriver driver;


        // ===== Locators =====
        private static readonly By UsernameInputLocator = By.Name("username");
        private static readonly By PasswordInputLocator = By.Name("password");
        private static readonly By LoginButtonLocator = By.CssSelector("input[type='submit'][value='Log In']");
        public void EnterUsername(string username)
        {
            var usernameInput = driver.FindElement(UsernameInputLocator);
            usernameInput.Clear();
            usernameInput.SendKeys(username);
        }
        public void EnterPassword(string password)
        {
            var passwordInput = driver.FindElement(PasswordInputLocator);
            passwordInput.Clear();
            passwordInput.SendKeys(password);
        }
        public void ClickLogin()
        {
            driver.FindElement(LoginButtonLocator).Click();
        }

        // Menu
        private static readonly By TransferFundsMenu =
            By.LinkText("Transfer Funds");

        // Form fields
        private static readonly By AmountInput =
            By.Id("amount");

        private static readonly By FromAccountDropdown =
            By.Id("fromAccountId");

        private static readonly By ToAccountDropdown =
            By.Id("toAccountId");

        // Button
        private static readonly By TransferButton =
            By.CssSelector("input[type='submit'][value='Transfer']");

        // Result message
        private static readonly By SuccessMessage =
            By.CssSelector("#rightPanel .title");

        private static readonly By ResultMessage =
            By.CssSelector("#rightPanel p");

        public TransferFundPage(IWebDriver driver)
        {
            this.driver = driver ?? throw new ArgumentNullException(nameof(driver));
        }

        // ===== Actions =====

        public void NavigateToTransferFunds()
        {
            driver.FindElement(TransferFundsMenu).Click();
        }

        public void EnterAmount(string amount)
        {
            var element = driver.FindElement(AmountInput);
            element.Clear();
            element.SendKeys(amount);
        }

        public void SelectFromAccount(string account)
        {
            var dropdown = driver.FindElement(FromAccountDropdown);
            dropdown.SendKeys(account);
        }

        public void SelectToAccount(string account)
        {
            var dropdown = driver.FindElement(ToAccountDropdown);
            dropdown.SendKeys(account);
        }

        public void ClickTransfer()
        {
            driver.FindElement(TransferButton).Click();
        }

        public string GetSuccessTitle()
        {
            try
            {
                return driver.FindElement(SuccessMessage).Text;
            }
            catch
            {
                return string.Empty;
            }
        }

        public string GetResultMessage()
        {
            try
            {
                return driver.FindElement(ResultMessage).Text;
            }
            catch
            {
                return string.Empty;
            }
        }
    }
}