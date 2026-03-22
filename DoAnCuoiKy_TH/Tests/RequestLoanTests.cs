using DoAnCuoiKy.Models;
using DoAnCuoiKy_TH.DataProviders;
using DoAnCuoiKy_TH.Pages;
using DoAnCuoiKy_TH.Utilities;
using NUnit.Framework;
using OpenQA.Selenium;

namespace DoAnCuoiKy_TH.Tests.RequestLoan
{
    [TestFixture]
    public class RequestLoanTests : DriverFactory
    {
        private const string ExcelSheetName = "TC_Tram";
        private const string TestCasePrefixFilter = "TC_";
        private static readonly string[] AllowedTestCases = { "TC_21", "TC_23" };

        private RequestLoanPage? requestLoanPage;

        [TestCaseSource(nameof(GetRequestLoanTestCases))]
        public void ExecuteRequestLoanTestCase(string testCaseId, string function, string bigItem)
        {
            var testCase = ExcelDataProvider.GetTestCaseById(ExcelSheetName, TestCasePrefixFilter, testCaseId);
            requestLoanPage = new RequestLoanPage(driver);

            // Bước đăng nhập bắt buộc (Pre-condition)
            PerformLogin("nguyen", "123");

            string actualResult;
            bool isTestPassed;
            string notes = "";

            try
            {
                foreach (var step in testCase.Steps)
                {
                    ExecuteTestStep(step);
                }

                // Lấy kết quả thực tế dựa trên status Approved/Denied
                actualResult = GetActualResult(testCaseId);
                isTestPassed = IsResultMatchingExpected(testCase.Expected, actualResult);
            }
            catch (Exception ex)
            {
                actualResult = "Lỗi hệ thống: " + ex.Message;
                isTestPassed = false;
            }

            // Ghi kết quả trả về Excel
            string resultStatus = isTestPassed ? "PASS" : "FAIL";
            ExcelDataProvider.WriteTestResults(testCase.Steps, actualResult, resultStatus, ExcelSheetName, notes);

            Assert.That(isTestPassed, Is.True, $"Test {testCaseId} không khớp kết quả mong đợi.");
        }

        private void PerformLogin(string user, string pass)
        {
            try
            {
                driver.FindElement(By.Name("username")).SendKeys(user);
                driver.FindElement(By.Name("password")).SendKeys(pass);
                driver.FindElement(By.CssSelector("input[value='Log In']")).Click();
                System.Threading.Thread.Sleep(2000);
            }
            catch { /* Đã đăng nhập sẵn */ }
        }

        private void ExecuteTestStep(TestStep step)
        {
            var action = step.StepAction?.ToLower() ?? "";
            var data = step.TestData ?? "";

            if (requestLoanPage == null) return;

            if (action.Contains("mở trang")) requestLoanPage.ClickRequestLoanMenu();
            else if (action.Contains("loan amount")) requestLoanPage.EnterLoanAmount(data);
            else if (action.Contains("down payment")) requestLoanPage.EnterDownPayment(data);
            else if (action.Contains("chọn account")) requestLoanPage.SelectFromAccount(data);
            else if (action.Contains("nhấn appply now")) requestLoanPage.ClickApplyNow();
        }

        private string GetActualResult(string testCaseId)
        {
            if (requestLoanPage == null) return "Lỗi trang";

            string status = requestLoanPage.GetLoanStatusText();

            // Khớp với Expected Result trong Excel của bạn
            if (status.Contains("approved"))
                return "Hiển thị thông báo khoản vay được duyệt";

            if (status.Contains("denied"))
                return "Hiển thị thông báo khoản vay bị từ chối";

            return "Kết quả khác: " + status;
        }

        private static IEnumerable<object[]> GetRequestLoanTestCases()
        {
            foreach (var data in ExcelDataProvider.GetNamedTestCases(ExcelSheetName, TestCasePrefixFilter))
            {
                if (data.Length > 0 && AllowedTestCases.Contains(data[0].ToString()!.ToUpper()))
                    yield return data;
            }
        }

        private static bool IsResultMatchingExpected(string expected, string actual)
        {
            // So sánh không phân biệt hoa thường và bỏ qua khoảng trắng thừa
            return actual.Trim().ToLower().Contains(expected.Trim().ToLower()) ||
                   expected.Trim().ToLower().Contains(actual.Trim().ToLower());
        }
    }
}