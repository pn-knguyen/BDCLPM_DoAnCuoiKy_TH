using DoAnCuoiKy.Models;
using DoAnCuoiKy_TH.DataProviders;
using DoAnCuoiKy_TH.Pages;
using DoAnCuoiKy_TH.Utilities;
using NUnit.Framework;
using OpenQA.Selenium;

namespace DoAnCuoiKy_TH.Tests.GUI
{
    [TestFixture]
    public class RequestLoanGUITests : DriverFactory
    {
        private const string ExcelSheetName = "TC_Tram";
        private const string TestCasePrefixFilter = "TC_";
        private static readonly string[] AllowedTestCases = { "TC_29" };

        [TestCaseSource(nameof(GetGUITestCases))]
        public void Execute_TC29_DropdownCheck(string testCaseId, string function, string bigItem)
        {
            var testCase = ExcelDataProvider.GetTestCaseById(ExcelSheetName, TestCasePrefixFilter, testCaseId);
            var loginPage = new LoginPage(driver);
            var loanPage = new RequestLoanPage(driver);

            string actualResult = "";
            bool isTestPassed = false;

            try
            {
                // BƯỚC PRE-CONDITION: Đăng nhập
                driver.Navigate().GoToUrl("https://parabank.parasoft.com/parabank/index.htm");
                loginPage.EnterUsername("nguyen");
                loginPage.EnterPassword("123");
                loginPage.ClickLogin();
                Thread.Sleep(2000);

                // BƯỚC 1: Mở trang Request Loan
                loanPage.ClickRequestLoanMenu();
                Thread.Sleep(1000);

                // BƯỚC 2-3: Kiểm tra Label và Dropdown hiển thị
                bool isLabelOk = loanPage.IsFromAccountLabelDisplayed();
                bool isDropdownOk = loanPage.IsFromAccountDropdownDisplayed();

                // BƯỚC 4-5: Click và kiểm tra danh sách account
                int accountCount = loanPage.GetAccountDropdownOptionCount();

                // BƯỚC 6-7: Chọn account đầu tiên và kiểm tra
                string firstAcc = "14898"; // Lấy theo Data trong ảnh
                loanPage.SelectFromAccount(firstAcc);
                string selectedText1 = loanPage.GetSelectedAccountText();

                // BƯỚC 8-10: Chọn account thứ hai và kiểm tra
                string secondAcc = "14121"; // Lấy theo Data trong ảnh
                loanPage.SelectFromAccount(secondAcc);
                string selectedText2 = loanPage.GetSelectedAccountText();

                // KIỂM TRA TỔNG HỢP KẾT QUẢ
                if (isLabelOk && isDropdownOk && accountCount > 0 && selectedText1 == firstAcc && selectedText2 == secondAcc)
                {
                    actualResult = "Dropdown hiển thị danh sách tài khoản. Đã chọn thành công 22335 và 28884 hiển thị đúng";
                    isTestPassed = true;
                }
                else
                {
                    actualResult = $"Lỗi hiển thị hoặc chọn tài khoản. Label: {isLabelOk}, Dropdown: {isDropdownOk}, Count: {accountCount}";
                    isTestPassed = false;
                }
            }
            catch (Exception ex)
            {
                actualResult = "Lỗi trong quá trình kiểm tra: " + ex.Message;
                isTestPassed = false;
            }

            // Ghi kết quả vào Excel
            string resultStatus = isTestPassed ? "PASS" : "FAIL";
            ExcelDataProvider.WriteTestResults(testCase.Steps, actualResult, resultStatus, ExcelSheetName);

            Assert.That(isTestPassed, Is.True);
        }

        private static IEnumerable<object[]> GetGUITestCases()
        {
            foreach (var data in ExcelDataProvider.GetNamedTestCases(ExcelSheetName, TestCasePrefixFilter))
            {
                if (data.Length > 0 && AllowedTestCases.Contains(data[0].ToString()!.ToUpper()))
                    yield return data;
            }
        }
    }
}