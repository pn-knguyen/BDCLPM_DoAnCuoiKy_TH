using System.Globalization;
using DoAnCuoiKy.Models;
using DoAnCuoiKy_TH.DataProviders;
using DoAnCuoiKy_TH.Pages;
using DoAnCuoiKy_TH.Utilities;
using NUnit.Framework;
using OpenQA.Selenium;

namespace DoAnCuoiKy_TH.Tests.FindTransactions
{
    [TestFixture]
    public class FindTransactionsTests : DriverFactory
    {
        private const string ExcelSheetName = "TC_Tram";
        private const string TestCasePrefixFilter = "TC_";

        // CHỈ CHẠY MỖI TC_16
        private static readonly string[] AllowedTestCases = { "TC_16" };

        private FindTransactionsPage? findTransactionsPage;

        [TestCaseSource(nameof(GetFindTransactionsTestCases))]
        public void ExecuteFindTransactionsTestCase(string testCaseId, string function, string bigItem)
        {
            var testCase = ExcelDataProvider.GetTestCaseById(ExcelSheetName, TestCasePrefixFilter, testCaseId);
            findTransactionsPage = new FindTransactionsPage(driver);

            LogTestCaseInfo(testCaseId, function, bigItem);

            // BƯỚC PRE-CONDITION: TỰ ĐỘNG ĐĂNG NHẬP
            PerformLogin("nguyen", "123");

            string actualResult;
            bool isTestPassed;
            string notes = "";

            try
            {
                foreach (var step in testCase.Steps)
                {
                    TestContext.Out.WriteLine($"Step {step.StepNumber} - {step.StepAction} - {step.TestData}");
                    ExecuteTestStep(step);
                }

                actualResult = GetActualResult();
                isTestPassed = IsResultMatchingExpected(testCase.Expected, actualResult);
            }
            catch (Exception ex)
            {
                actualResult = ex.Message;
                isTestPassed = false;
            }

            if (!isTestPassed)
            {
                string screenshotPath = CaptureScreenshot(testCaseId);
                notes = $"Screenshot: {screenshotPath}";
                TestContext.Out.WriteLine($"Screenshot saved at: {screenshotPath}");
            }

            string resultStatus = isTestPassed ? "PASS" : "FAIL";
            string updatedExcelRange = ExcelDataProvider.WriteTestResults(testCase.Steps, actualResult, resultStatus, ExcelSheetName, notes);
            LogTestResults(testCase.Expected, actualResult, resultStatus, updatedExcelRange);

            Assert.That(isTestPassed, Is.True, $"Test case {testCaseId} failed. Expected: {testCase.Expected}, Actual: {actualResult}");
        }

        private static IEnumerable<object[]> GetFindTransactionsTestCases()
        {
            foreach (var testCaseData in ExcelDataProvider.GetNamedTestCases(ExcelSheetName, TestCasePrefixFilter))
            {
                if (testCaseData.Length > 0 && testCaseData[0] is string testCaseId && AllowedTestCases.Contains(testCaseId.ToUpper()))
                {
                    yield return testCaseData;
                }
            }
        }

        private void PerformLogin(string username, string password)
        {
            try
            {
                driver.FindElement(By.Name("username")).SendKeys(username);
                driver.FindElement(By.Name("password")).SendKeys(password);
                driver.FindElement(By.CssSelector("input[value='Log In']")).Click();
                System.Threading.Thread.Sleep(2000);
            }
            catch (Exception ex)
            {
                TestContext.Out.WriteLine($"Lỗi Đăng nhập Pre-condition: {ex.Message}");
            }
        }

        private void ExecuteTestStep(TestStep step)
        {
            var action = step.StepAction?.ToLower() ?? "";
            var testData = (step.TestData ?? "").Replace("'", ""); // Xóa nháy đơn nếu có

            if (findTransactionsPage == null) return;

            // Dịch thao tác từ Excel sang hành động trên Web
            if (action.Contains("mở trang") || action.Contains("menu"))
                findTransactionsPage.ClickFindTransactionsMenu();

            else if (action.Contains("số tiền") || action.Contains("amount"))
                findTransactionsPage.EnterAmount(testData);

            else if (action.Contains("tìm kiếm") || action.Contains("find"))
                findTransactionsPage.ClickFindByAmountButton();
        }

        private string GetActualResult()
        {
            if (findTransactionsPage == null) return "Lỗi trang";

            string rawMessage = findTransactionsPage.GetResultMessage();

            // --- TRƯỜNG HỢP 1: CÓ GIAO DỊCH ---
            if (rawMessage == "has_transactions")
            {
                return "Hiển thị giao dịch khớp với số tiền cần tìm";
            }

            // --- TRƯỜNG HỢP 2: KHÔNG CÓ GIAO DỊCH ---
            if (rawMessage == "no_transactions")
            {
                return "Không hiển thị giao dịch nào";
            }

            // --- TRƯỜNG HỢP 3: LỖI WEB (Lỗi đỏ) ---
            if (rawMessage.ToLower().Contains("error"))
            {
                return "Hiển thị thông báo lỗi";
            }

            return rawMessage;
        }

        private static bool IsResultMatchingExpected(string expectedText, string actualText)
        {
            if (string.IsNullOrWhiteSpace(expectedText)) return false;
            var expected = NormalizeText(expectedText);
            var actual = NormalizeText(actualText);
            return actual.Contains(expected) || expected.Contains(actual) || actual.Equals(expected);
        }

        private static string NormalizeText(string input)
        {
            return (input ?? string.Empty)
                .Normalize(System.Text.NormalizationForm.FormC)
                .Replace('\u00A0', ' ')
                .Replace("\r", " ")
                .Replace("\n", " ")
                .Replace("\t", " ")
                .ToLowerInvariant()
                .Trim();
        }

        private string CaptureScreenshot(string testCaseId)
        {
            try
            {
                string screenshotDirectory = Path.Combine(AppContext.BaseDirectory, "Screenshots");
                if (!Directory.Exists(screenshotDirectory)) Directory.CreateDirectory(screenshotDirectory);
                string fileName = $"{testCaseId}_{DateTime.Now:yyyyMMdd_HHmmss}.png";
                string screenshotPath = Path.Combine(screenshotDirectory, fileName);
                var screenshot = ((ITakesScreenshot)driver).GetScreenshot();
                screenshot.SaveAsFile(screenshotPath);
                return screenshotPath;
            }
            catch { return ""; }
        }

        private void LogTestCaseInfo(string testCaseId, string function, string bigItem)
        {
            TestContext.Out.WriteLine("═══════════════════════════════════════");
            TestContext.Out.WriteLine($"Test Case ID: {testCaseId}");
        }

        private void LogTestResults(string expected, string actual, string status, string updatedRange)
        {
            TestContext.Out.WriteLine("───────────────────────────────────────");
            TestContext.Out.WriteLine($"Expected Result: {expected}");
            TestContext.Out.WriteLine($"Actual Result: {actual}");
            TestContext.Out.WriteLine($"Test Status: {status}");
        }
    }
}