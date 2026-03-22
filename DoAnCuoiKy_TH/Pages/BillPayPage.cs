using OpenQA.Selenium;

namespace DoAnCuoiKy_TH.Pages
{
    public class BillPayPage
    {
        private readonly IWebDriver _driver;
        public BillPayPage(IWebDriver driver) => _driver = driver;

        private readonly By billPayMenuLink = By.LinkText("Bill Pay");
        private readonly By payeeNameInput = By.Name("payee.name");
        private readonly By addressInput = By.Name("payee.address.street");
        private readonly By cityInput = By.Name("payee.address.city");
        private readonly By stateInput = By.Name("payee.address.state");
        private readonly By zipCodeInput = By.Name("payee.address.zipCode");
        private readonly By phoneInput = By.Name("payee.phoneNumber");
        private readonly By accountInput = By.Name("payee.accountNumber");
        private readonly By verifyAccountInput = By.Name("verifyAccount");
        private readonly By amountInput = By.Name("amount");
        private readonly By sendPaymentBtn = By.CssSelector("input[value='Send Payment']");

        public void ClickBillPayMenu() => _driver.FindElement(billPayMenuLink).Click();
        public void EnterPayeeName(string text) => _driver.FindElement(payeeNameInput).SendKeys(text);
        public void EnterAddress(string text) => _driver.FindElement(addressInput).SendKeys(text);
        public void EnterCity(string text) => _driver.FindElement(cityInput).SendKeys(text);
        public void EnterState(string text) => _driver.FindElement(stateInput).SendKeys(text);
        public void EnterZipCode(string text) => _driver.FindElement(zipCodeInput).SendKeys(text);
        public void EnterPhone(string text) => _driver.FindElement(phoneInput).SendKeys(text);
        public void EnterAccount(string text) => _driver.FindElement(accountInput).SendKeys(text);
        public void EnterVerifyAccount(string text) => _driver.FindElement(verifyAccountInput).SendKeys(text);
        public void EnterAmount(string text) => _driver.FindElement(amountInput).SendKeys(text);

        public void ClickSendPaymentButton()
        {
            _driver.FindElement(sendPaymentBtn).Click();
            System.Threading.Thread.Sleep(3000);
        }

        public string GetResultMessage()
        {
            try
            {
                var successHeader = _driver.FindElements(By.XPath("//h1[text()='Bill Payment Complete']"));
                if (successHeader.Count > 0 && successHeader[0].Displayed) return "success";

                var errors = _driver.FindElements(By.ClassName("error"));
                foreach (var error in errors)
                {
                    if (error.Displayed && !string.IsNullOrWhiteSpace(error.Text)) return "error: " + error.Text;
                }
                return "Không thấy thông báo kết quả";
            }
            catch { return "Không thấy thông báo kết quả"; }
        }
    }
}