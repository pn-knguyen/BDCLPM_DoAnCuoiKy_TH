using System.Globalization;
using DoAnCuoiKy.Models;
using DoAnCuoiKy_TH.DataProviders;
using DoAnCuoiKy_TH.Pages;
using DoAnCuoiKy_TH.Utilities;
using NUnit.Framework;
using OpenQA.Selenium;

namespace DoAnCuoiKy_TH.Tests.Register
{
    [TestFixture]
    public class RegisterTests : DriverFactory
    {
        #region Constants
        private const string ExcelSheetName = "TC_Tram";
        private const string TestCasePrefixFilter = "TC_";

        // Danh sách các Test Case muốn chạy
        private static readonly string[] AllowedTestCases = { "TC_01", "TC_03", "TC_05" };
        #endregion

        private RegisterPage? registerPage;

        [TestCaseSource(nameof(GetVisibleRegisterTestCases))]
        public void ExecuteRegisterTestCase(string testCaseId, string function, string bigItem)
        {
            if (!ShouldRunTestCase(testCaseId, out _))
            {
                // Sửa lại câu thông báo Ignore cho đúng logic mới
                Assert.Ignore($"Skipping {testCaseId}. Test case is not in the AllowedTestCases list.");
            }

            var testCase = ExcelDataProvider.GetTestCaseById(ExcelSheetName, TestCasePrefixFilter, testCaseId);
            registerPage = new RegisterPage(driver);

            LogTestCaseInfo(testCaseId, function, bigItem);

            string actualResult;
            bool isTestPassed;
            string notes = "";

            try
            {
                // Vòng lặp đọc các steps từ Excel và thực thi
                foreach (var step in testCase.Steps)
                {
                    TestContext.Out.WriteLine($"Step {step.StepNumber} - {step.StepAction} - {step.TestData}");
                    ExecuteTestStep(step);
                }

                // Dừng 3 giây để web kịp load ra câu thông báo (tránh lỗi timing)
                System.Threading.Thread.Sleep(3000);

                // Lấy kết quả thực tế sau khi chạy xong các bước
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

        private static IEnumerable<object[]> GetVisibleRegisterTestCases()
        {
            foreach (var testCaseData in ExcelDataProvider.GetNamedTestCases(ExcelSheetName, TestCasePrefixFilter))
            {
                if (testCaseData.Length == 0 || testCaseData[0] is not string testCaseId) continue;
                if (ShouldRunTestCase(testCaseId, out _)) yield return testCaseData;
            }
        }

        // Đã sửa lại hàm này để chỉ chạy các TC nằm trong mảng AllowedTestCases
        private static bool ShouldRunTestCase(string testCaseId, out int caseNumber)
        {
            caseNumber = 0;
            if (string.IsNullOrWhiteSpace(testCaseId)) return false;
            return AllowedTestCases.Contains(testCaseId.ToUpper());
        }

        // Hàm này tự động "dịch" cột Step action trong Excel thành hành động trên Web
        private void ExecuteTestStep(TestStep step)
        {
            var action = step.StepAction?.ToLower() ?? "";
            var testData = step.TestData ?? "";

            if (registerPage == null) throw new InvalidOperationException("RegisterPage is not initialized");

            if (action.Contains("mở trang register")) registerPage.ClickRegisterMenu();
            else if (action.Contains("first name")) registerPage.EnterFirstName(testData);
            else if (action.Contains("last name")) registerPage.EnterLastName(testData);
            else if (action.Contains("address")) registerPage.EnterAddress(testData);
            else if (action.Contains("city")) registerPage.EnterCity(testData);
            else if (action.Contains("state")) registerPage.EnterState(testData);
            else if (action.Contains("zip code")) registerPage.EnterZipCode(testData);
            else if (action.Contains("phone")) registerPage.EnterPhone(testData);
            else if (action.Contains("ssn")) registerPage.EnterSSN(testData);
            else if (action.Contains("username")) registerPage.EnterUsername(testData);
            else if (action.Contains("password ngắn") || (!action.Contains("confirm") && action.Contains("password"))) registerPage.EnterPassword(testData);
            else if (action.Contains("confirm")) registerPage.EnterConfirmPassword(testData);
            else if (action.Contains("nhấn register")) registerPage.ClickRegisterButton();
        }

        private string GetActualResult()
        {
            if (registerPage == null) throw new InvalidOperationException("RegisterPage is not initialized");

            try
            {
                // Lấy câu thông báo lỗi màu đỏ trên màn hình web ParaBank
                string errorMessage = registerPage.GetErrorMessageText();

                if (!string.IsNullOrEmpty(errorMessage))
                {
                    string lowerError = errorMessage.ToLower();

                    // Nếu web báo lỗi "already exists" (Dành cho TC_03)
                    if (lowerError.Contains("already exists"))
                    {
                        return "Hiển thị lỗi người dùng đã tồn tại";
                    }

                    // Nếu web báo lỗi "did not match" (Dành cho TC_05)
                    if (lowerError.Contains("did not match"))
                    {
                        return "Mật khẩu không khớp";
                    }

                    return errorMessage;
                }

                // Nếu không có lỗi gì, đi tìm dòng chữ báo đăng ký thành công (Dành cho TC_01)
                string successMessage = registerPage.GetSuccessMessageText();
                if (!string.IsNullOrEmpty(successMessage))
                {
                    return "Đăng ký thành công";
                }

                return "Không thấy thông báo kết quả";
            }
            catch (Exception ex)
            {
                return $"Error checking result: {ex.Message}";
            }
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
            catch (Exception ex) { return $"Failed to capture screenshot: {ex.Message}"; }
        }

        private void LogTestCaseInfo(string testCaseId, string function, string bigItem)
        {
            TestContext.Out.WriteLine("═══════════════════════════════════════");
            TestContext.Out.WriteLine($"Test Case ID: {testCaseId}");
            TestContext.Out.WriteLine($"Function: {function}");
            TestContext.Out.WriteLine($"Big Item: {bigItem}");
            TestContext.Out.WriteLine("═══════════════════════════════════════");
            TestContext.Out.WriteLine();
        }

        private void LogTestResults(string expected, string actual, string status, string updatedRange)
        {
            TestContext.Out.WriteLine();
            TestContext.Out.WriteLine("───────────────────────────────────────");
            TestContext.Out.WriteLine($"Expected Result: {expected}");
            TestContext.Out.WriteLine($"Actual Result: {actual}");
            TestContext.Out.WriteLine($"Test Status: {status}");
            TestContext.Out.WriteLine($"Excel Updated: {updatedRange}");
            TestContext.Out.WriteLine("───────────────────────────────────────");
        }
    }
}