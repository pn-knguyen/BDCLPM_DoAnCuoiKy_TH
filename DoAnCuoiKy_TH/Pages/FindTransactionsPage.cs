using OpenQA.Selenium;

namespace DoAnCuoiKy_TH.Pages
{
    public class FindTransactionsPage
    {
        private readonly IWebDriver _driver;
        public FindTransactionsPage(IWebDriver driver) => _driver = driver;

        private readonly By findTransactionsMenuLink = By.LinkText("Find Transactions");
        private readonly By amountInput = By.Id("amount");
        private readonly By findByAmountBtn = By.Id("findByAmount");

        public void ClickFindTransactionsMenu() => _driver.FindElement(findTransactionsMenuLink).Click();
        public void EnterAmount(string amount) => _driver.FindElement(amountInput).SendKeys(amount);

        public void ClickFindByAmountButton()
        {
            _driver.FindElement(findByAmountBtn).Click();
            System.Threading.Thread.Sleep(3000);
        }

        public string GetResultMessage()
        {
            try
            {
                var successHeader = _driver.FindElements(By.XPath("//h1[normalize-space()='Transaction Results']"));
                if (successHeader.Count > 0 && successHeader[0].Displayed)
                {
                    var rows = _driver.FindElements(By.XPath("//table[@id='transactionTable']/tbody/tr"));
                    return (rows.Count > 0 && !rows[0].Text.Contains("NaN")) ? "has_transactions" : "no_transactions";
                }
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