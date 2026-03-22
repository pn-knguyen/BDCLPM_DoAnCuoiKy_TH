using OpenQA.Selenium;

namespace DoAnCuoiKy_TH.Pages
{
    public class UpdateContactInfoPage
    {
        private readonly IWebDriver driver;

        // ===== Login Locators =====
        private static readonly By UsernameInputLocator = By.Name("username");
        private static readonly By PasswordInputLocator = By.Name("password");
        private static readonly By LoginButtonLocator = By.CssSelector("input[type='submit'][value='Log In']");

        // ===== Menu =====
        private static readonly By UpdateContactMenu =
            By.LinkText("Update Contact Info");

        // ===== Form fields =====
        private static readonly By FirstNameInput = By.Id("customer.firstName");
        private static readonly By LastNameInput = By.Id("customer.lastName");
        private static readonly By AddressInput = By.Id("customer.address.street");
        private static readonly By CityInput = By.Id("customer.address.city");
        private static readonly By StateInput = By.Id("customer.address.state");
        private static readonly By ZipCodeInput = By.Id("customer.address.zipCode");
        private static readonly By PhoneInput = By.Id("customer.phoneNumber");

        // ===== Error Locators =====
        private static readonly By FirstNameError = By.Id("firstName-error");
        private static readonly By LastNameError = By.Id("lastName-error");
        private static readonly By StreetError = By.Id("street-error");
        private static readonly By CityError = By.Id("city-error");
        private static readonly By StateError = By.Id("state-error");
        private static readonly By ZipCodeError = By.Id("zipCode-error");

        // ===== Button =====
        private static readonly By UpdateButton =
            By.XPath("//input[contains(@value,'Update Profile')] | //button[contains(normalize-space(),'Update Profile')]");

        // ===== Result =====
        private static readonly By SuccessTitle =
            By.CssSelector("#rightPanel .title");

        private static readonly By ResultMessage =
            By.CssSelector("#rightPanel p");

        public UpdateContactInfoPage(IWebDriver driver)
        {
            this.driver = driver ?? throw new ArgumentNullException(nameof(driver));
        }

        // ===== Login =====
        public void EnterUsername(string username)
        {
            var e = driver.FindElement(UsernameInputLocator);
            e.Clear();
            e.SendKeys(username);
        }

        public void EnterPassword(string password)
        {
            var e = driver.FindElement(PasswordInputLocator);
            e.Clear();
            e.SendKeys(password);
        }

        public void ClickLogin()
        {
            driver.FindElement(LoginButtonLocator).Click();
        }

        // ===== Actions =====
        public void NavigateToUpdateContact()
        {
            driver.FindElement(UpdateContactMenu).Click();
        }

        public void EnterFirstName(string value)
        {
            var e = driver.FindElement(FirstNameInput);
            e.Clear();
            e.SendKeys(value);
        }

        public void EnterLastName(string value)
        {
            var e = driver.FindElement(LastNameInput);
            e.Clear();
            e.SendKeys(value);
        }

        public void EnterAddress(string value)
        {
            var e = driver.FindElement(AddressInput);
            e.Clear();
            e.SendKeys(value);
        }

        public void EnterCity(string value)
        {
            var e = driver.FindElement(CityInput);
            e.Clear();
            e.SendKeys(value);
        }

        public void EnterState(string value)
        {
            var e = driver.FindElement(StateInput);
            e.Clear();
            e.SendKeys(value);
        }

        public void EnterZipCode(string value)
        {
            var e = driver.FindElement(ZipCodeInput);
            e.Clear();
            e.SendKeys(value);
        }

        public void EnterPhone(string value)
        {
            var e = driver.FindElement(PhoneInput);
            e.Clear();
            e.SendKeys(value);
        }

        public void ClickUpdate()
        {
            driver.FindElement(UpdateButton).Click();
        }

        // ===== Error Handling =====

        public string GetFirstNameError() => GetTextSafe(FirstNameError);
        public string GetLastNameError() => GetTextSafe(LastNameError);
        public string GetStreetError() => GetTextSafe(StreetError);
        public string GetCityError() => GetTextSafe(CityError);
        public string GetStateError() => GetTextSafe(StateError);
        public string GetZipCodeError() => GetTextSafe(ZipCodeError);

        public string GetErrorByField(string fieldName)
        {
            try
            {
                var locator = By.Id($"{fieldName}-error");
                return driver.FindElement(locator).Text;
            }
            catch
            {
                return string.Empty;
            }
        }

        private string GetTextSafe(By locator)
        {
            try
            {
                return driver.FindElement(locator).Text;
            }
            catch
            {
                return string.Empty;
            }
        }

        // ===== Result =====
        public string GetSuccessTitle()
        {
            return GetTextSafe(SuccessTitle);
        }

        public string GetResultMessage()
        {
            return GetTextSafe(ResultMessage);
        }
    }
}