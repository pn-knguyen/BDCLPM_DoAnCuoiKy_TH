using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace DoAnCuoiKy_TH.Pages
{
    public class RequestLoanPage
    {
        private readonly IWebDriver _driver;
        public RequestLoanPage(IWebDriver driver) => _driver = driver;

        private readonly By requestLoanMenuLink = By.LinkText("Request Loan");
        private readonly By loanAmountInput = By.Id("amount");
        private readonly By downPaymentInput = By.Id("downPayment");
        private readonly By fromAccountSelect = By.Id("fromAccountId");
        private readonly By applyNowBtn = By.CssSelector("input[value='Apply Now']");
        private readonly By loanStatusResult = By.Id("loanStatus");

        public void ClickRequestLoanMenu() => _driver.FindElement(requestLoanMenuLink).Click();
        public void EnterLoanAmount(string amount) => _driver.FindElement(loanAmountInput).SendKeys(amount);
        public void EnterDownPayment(string downPayment) => _driver.FindElement(downPaymentInput).SendKeys(downPayment);

        public void SelectFromAccount(string accountId)
        {
            if (string.IsNullOrWhiteSpace(accountId)) return;
            try
            {
                new SelectElement(_driver.FindElement(fromAccountSelect)).SelectByText(accountId);
            }
            catch { }
        }

        public void ClickApplyNow()
        {
            _driver.FindElement(applyNowBtn).Click();
            System.Threading.Thread.Sleep(3000);
        }

        public string GetLoanStatusText()
        {
            try { return _driver.FindElement(loanStatusResult).Text.Trim().ToLower(); }
            catch { return "không tìm thấy kết quả"; }
        }

        public bool IsFromAccountDropdownDisplayed() => _driver.FindElement(fromAccountSelect).Displayed;
        public int GetAccountDropdownOptionCount() => new SelectElement(_driver.FindElement(fromAccountSelect)).Options.Count;
        public string GetSelectedAccountText() => new SelectElement(_driver.FindElement(fromAccountSelect)).SelectedOption.Text.Trim();
        public bool IsFromAccountLabelDisplayed()
        {
            try { return _driver.FindElement(By.XPath("//b[contains(text(), 'From account #')]")).Displayed; }
            catch { return false; }
        }
    }
}