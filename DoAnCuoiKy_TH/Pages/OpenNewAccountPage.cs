using OpenQA.Selenium;

namespace DoAnCuoiKy_TH.Pages
{
    public class OpenNewAccountPage
    {
        private readonly IWebDriver driver;

        private static readonly By UsernameInputLocator = By.Name("username");
        private static readonly By PasswordInputLocator = By.Name("password");
        private static readonly By LoginButtonLocator = By.CssSelector("input[type='submit'][value='Log In']");

        // ===== Menu =====
        private static readonly By OpenNewAccountMenu =
            By.LinkText("Open New Account");

        // ===== Form fields =====
        private static readonly By AccountTypeDropdown =
            By.Id("type");

        private static readonly By FromAccountDropdown =
            By.Id("fromAccountId");

        // ===== Button =====
        private static readonly By OpenNewAccountButton =
            By.XPath("//input[contains(@value,'Open New Account')] | //button[contains(normalize-space(),'Open New Account')]");

        // ===== Result =====
        private static readonly By SuccessTitle =
            By.CssSelector("#rightPanel .title");

        private static readonly By NewAccountId =
            By.Id("newAccountId");

        private static readonly By ResultMessage =
            By.CssSelector("#rightPanel p");

        public OpenNewAccountPage(IWebDriver driver)
        {
            this.driver = driver ?? throw new ArgumentNullException(nameof(driver));
        }

        // ===== Login Actions =====
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

        // ===== Actions =====
        public void NavigateToOpenNewAccount()
        {
            driver.FindElement(OpenNewAccountMenu).Click();
        }

        public void SelectAccountType(string accountType)
        {
            var dropdown = driver.FindElement(AccountTypeDropdown);
            dropdown.SendKeys(accountType);
        }

        public void SelectFromAccount(string account)
        {
            var dropdown = driver.FindElement(FromAccountDropdown);
            dropdown.SendKeys(account);
        }

        public void ClickOpenNewAccount()
        {
            driver.FindElement(OpenNewAccountButton).Click();
        }

        // ===== Result =====
        public string GetSuccessTitle()
        {
            try
            {
                return driver.FindElement(SuccessTitle).Text;
            }
            catch
            {
                return string.Empty;
            }
        }

        public string GetNewAccountId()
        {
            try
            {
                return driver.FindElement(NewAccountId).Text;
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