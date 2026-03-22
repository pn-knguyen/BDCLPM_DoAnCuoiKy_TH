using OpenQA.Selenium;

namespace DoAnCuoiKy_TH.Pages
{
    public class RegisterPage
    {
        private readonly IWebDriver _driver;
        public RegisterPage(IWebDriver driver) => _driver = driver;

        private readonly By registerMenuLink = By.LinkText("Register");
        private readonly By firstNameInput = By.Id("customer.firstName");
        private readonly By lastNameInput = By.Id("customer.lastName");
        private readonly By addressInput = By.Id("customer.address.street");
        private readonly By cityInput = By.Id("customer.address.city");
        private readonly By stateInput = By.Id("customer.address.state");
        private readonly By zipCodeInput = By.Id("customer.address.zipCode");
        private readonly By phoneInput = By.Id("customer.phoneNumber");
        private readonly By ssnInput = By.Id("customer.ssn");
        private readonly By usernameInput = By.Id("customer.username");
        private readonly By passwordInput = By.Id("customer.password");
        private readonly By confirmPasswordInput = By.Id("repeatedPassword");
        private readonly By registerBtn = By.CssSelector("input[value='Register']");
        private readonly By successMessage = By.CssSelector("body > div:nth-child(1) > div:nth-child(3) > div:nth-child(2) > p:nth-child(2)");
        private readonly By errorMessage = By.ClassName("error");

        public void ClickRegisterMenu() => _driver.FindElement(registerMenuLink).Click();
        public void EnterFirstName(string text) => _driver.FindElement(firstNameInput).SendKeys(text);
        public void EnterLastName(string text) => _driver.FindElement(lastNameInput).SendKeys(text);
        public void EnterAddress(string text) => _driver.FindElement(addressInput).SendKeys(text);
        public void EnterCity(string text) => _driver.FindElement(cityInput).SendKeys(text);
        public void EnterState(string text) => _driver.FindElement(stateInput).SendKeys(text);
        public void EnterZipCode(string text) => _driver.FindElement(zipCodeInput).SendKeys(text);
        public void EnterPhone(string text) => _driver.FindElement(phoneInput).SendKeys(text);
        public void EnterSSN(string text) => _driver.FindElement(ssnInput).SendKeys(text);
        public void EnterUsername(string text) => _driver.FindElement(usernameInput).SendKeys(text);
        public void EnterPassword(string text) => _driver.FindElement(passwordInput).SendKeys(text);
        public void EnterConfirmPassword(string text) => _driver.FindElement(confirmPasswordInput).SendKeys(text);
        public void ClickRegisterButton() => _driver.FindElement(registerBtn).Click();

        public string GetSuccessMessageText()
        {
            try { return _driver.FindElement(successMessage).Text; } catch { return string.Empty; }
        }

        public string GetErrorMessageText()
        {
            try
            {
                var errorElements = _driver.FindElements(errorMessage);
                foreach (var element in errorElements)
                {
                    if (!string.IsNullOrWhiteSpace(element.Text)) return element.Text;
                }
                return string.Empty;
            }
            catch { return string.Empty; }
        }
    }
}