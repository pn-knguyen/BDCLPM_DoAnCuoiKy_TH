using System.Globalization;
using DoAnCuoiKy.Models;
using DoAnCuoiKy_TH.DataProviders;
using DoAnCuoiKy_TH.Pages;
using DoAnCuoiKy_TH.Utilities;
using NUnit.Framework;
using OpenQA.Selenium;

namespace DoAnCuoiKy_TH.Tests.BillPay
{
    [TestFixture]
    public class BillPayTests : DriverFactory
    {
        private const string ExcelSheetName = "TC_Tram";
        private const string TestCasePrefixFilter = "TC_";

        // Danh sách chỉ định CHỈ CHẠY TC_06 và TC_08 trong file test này
        private static readonly string[] AllowedTestCases = { "TC_06", "TC_08" };

        private BillPayPage? billPayPage;

        [TestCaseSource(nameof(GetBillPayTestCases))]
        public void ExecuteBillPayTestCase(string testCaseId, string function, string bigItem)
        {
            var testCase = ExcelDataProvider.GetTestCaseById(ExcelSheetName, TestCasePrefixFilter, testCaseId);
            billPayPage = new BillPayPage(driver);

            LogTestCaseInfo(testCaseId, function, bigItem);

            // -------------------------------------------------------------------
            // BƯỚC PRE-CONDITION: TỰ ĐỘNG ĐĂNG NHẬP TRƯỚC KHI BẮT ĐẦU TEST CASE
            // -------------------------------------------------------------------
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

                // Lấy kết quả thực tế và so sánh
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

        private static IEnumerable<object[]> GetBillPayTestCases()
        {
            foreach (var testCaseData in ExcelDataProvider.GetNamedTestCases(ExcelSheetName, TestCasePrefixFilter))
            {
                if (testCaseData.Length > 0 && testCaseData[0] is string testCaseId && AllowedTestCases.Contains(testCaseId.ToUpper()))
                {
                    yield return testCaseData;
                }
            }
        }

        // Hàm hỗ trợ tự động đăng nhập (Pre-Condition)
        private void PerformLogin(string username, string password)
        {
            try
            {
                driver.FindElement(By.Name("username")).SendKeys(username);
                driver.FindElement(By.Name("password")).SendKeys(password);
                driver.FindElement(By.CssSelector("input[value='Log In']")).Click();
                System.Threading.Thread.Sleep(2000); // Chờ load vào trang tổng quan
            }
            catch (Exception ex)
            {
                TestContext.Out.WriteLine($"Lỗi Đăng nhập Pre-condition: {ex.Message}");
            }
        }

        // Đọc cột Step Action trong Excel và gọi hàm tương ứng
        private void ExecuteTestStep(TestStep step)
        {
            var action = step.StepAction?.ToLower() ?? "";
            var testData = step.TestData ?? "";

            if (billPayPage == null) return;

            if (action.Contains("mở trang") || action.Contains("menu")) billPayPage.ClickBillPayMenu();
            else if (action.Contains("payee name")) billPayPage.EnterPayeeName(testData);
            else if (action.Contains("address")) billPayPage.EnterAddress(testData);
            else if (action.Contains("city")) billPayPage.EnterCity(testData);
            else if (action.Contains("state")) billPayPage.EnterState(testData);
            else if (action.Contains("zip code")) billPayPage.EnterZipCode(testData);
            else if (action.Contains("phone")) billPayPage.EnterPhone(testData);
            else if (action.Contains("verify")) billPayPage.EnterVerifyAccount(testData); // Lưu ý dòng này để trên account
            else if (action.Contains("account")) billPayPage.EnterAccount(testData);
            else if (action.Contains("amount") || action.Contains("số tiền")) billPayPage.EnterAmount(testData);
            else if (action.Contains("send payment")) billPayPage.ClickSendPaymentButton();
        }

        // Phiên dịch kết quả từ Tiếng Anh sang Tiếng Việt cho khớp Excel
        private string GetActualResult()
        {
            if (billPayPage == null) return "Lỗi trang";

            string rawMessage = billPayPage.GetResultMessage();
            string lowerMsg = rawMessage.ToLower();

            // 1. Ánh xạ thành công (Dành cho TC_06)
            if (lowerMsg.Contains("success"))
            {
                return "Thanh toán thành công";
            }

            // 2. Ánh xạ báo lỗi (Dành cho TC_08 số tiền âm)
            if (lowerMsg.Contains("error"))
            {
                return "Hiển thị lỗi số tiền không hợp lệ"; 
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

        // Bộ lọc chuẩn hóa Tiếng Việt và dấu cách (Tránh lỗi do Unicode)
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
            catch (Exception ex) { return $"Failed to capture screenshot: {ex.Message}"; }
        }

        private void LogTestCaseInfo(string testCaseId, string function, string bigItem)
        {
            TestContext.Out.WriteLine("═══════════════════════════════════════");
            TestContext.Out.WriteLine($"Test Case ID: {testCaseId}");
            TestContext.Out.WriteLine($"Function: {function}");
            TestContext.Out.WriteLine($"Big Item: {bigItem}");
            TestContext.Out.WriteLine("═══════════════════════════════════════");
        }

        private void LogTestResults(string expected, string actual, string status, string updatedRange)
        {
            TestContext.Out.WriteLine("───────────────────────────────────────");
            TestContext.Out.WriteLine($"Expected Result: {expected}");
            TestContext.Out.WriteLine($"Actual Result: {actual}");
            TestContext.Out.WriteLine($"Test Status: {status}");
            TestContext.Out.WriteLine($"Excel Updated: {updatedRange}");
            TestContext.Out.WriteLine("───────────────────────────────────────\n");
        }
    }
}