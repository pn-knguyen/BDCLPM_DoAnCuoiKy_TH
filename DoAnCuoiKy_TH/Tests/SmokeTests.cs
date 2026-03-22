using DoAnCuoiKy.Models;
using DoAnCuoiKy_TH.DataProviders;
using DoAnCuoiKy_TH.Pages;
using DoAnCuoiKy_TH.Utilities;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;

namespace DoAnCuoiKy_TH.Tests.Smoke
{
    [TestFixture]
    public class SmokeTests : DriverFactory
    {
        private const string ExcelSheetName = "TC_Tram";
        private const string TestCasePrefixFilter = "TC_";
        private static readonly string[] AllowedTestCases = { "TC_30" };

        [TestCaseSource(nameof(GetSmokeTestCases))]
        public void ExecuteSmokeTestCase(string testCaseId, string function, string bigItem)
        {
            var testCase = ExcelDataProvider.GetTestCaseById(ExcelSheetName, TestCasePrefixFilter, testCaseId);

            var registerPage = new RegisterPage(driver);
            var loginPage = new LoginPage(driver);
            var billPayPage = new BillPayPage(driver);
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(15));

            string actualResult = "";
            bool isTestPassed = false;

            // Dùng tên ngẫu nhiên để bước Register (Bước 12-15) luôn PASS
            string dynamicUser = "testuser" + DateTime.Now.Ticks.ToString().Substring(12);

            try
            {
                // BƯỚC 1-2: Mở trang và click Register
                driver.Navigate().GoToUrl("https://parabank.parasoft.com/parabank/register.htm");

                // BƯỚC 4-11: Nhập thông tin cá nhân (Theo ảnh image_5f7545.png)
                registerPage.EnterFirstName("Test");
                registerPage.EnterLastName("User");
                registerPage.EnterAddress("123 Test Street");
                registerPage.EnterCity("TestCity");
                registerPage.EnterState("CA");
                registerPage.EnterZipCode("12345");
                registerPage.EnterPhone("123456789");
                registerPage.EnterSSN("12345");

                // BƯỚC 12-14: Nhập Username/Password
                registerPage.EnterUsername(dynamicUser);
                registerPage.EnterPassword("testpass");
                registerPage.EnterConfirmPassword("testpass");

                // BƯỚC 15: Click Register
                registerPage.ClickRegisterButton();

                // BƯỚC 16: Kiểm tra đăng ký thành công
                wait.Until(d => d.FindElement(By.XPath("//p[contains(text(), 'Your account was created successfully')]")));

                // BƯỚC 17: Click Log Out
                wait.Until(d => d.FindElement(By.LinkText("Log Out"))).Click();

                // BƯỚC 18: Đăng nhập tài khoản vừa tạo
                loginPage.EnterUsername(dynamicUser);
                loginPage.EnterPassword("testpass");
                loginPage.ClickLogin();

                // BƯỚC 19: Kiểm tra Account Overview hiển thị
                wait.Until(d => {
                    var el = d.FindElement(By.XPath("//h1[normalize-space()='Accounts Overview']"));
                    return el.Displayed ? el : null;
                });

                // BƯỚC 20: Vào menu Bill Pay
                billPayPage.ClickBillPayMenu();

                // BƯỚC 21-29: Nhập thông tin Bill Pay (Theo ảnh image_5f7545.png)
                billPayPage.EnterPayeeName("Test Payee");
                billPayPage.EnterAddress("Test Address");
                billPayPage.EnterCity("Test City");
                billPayPage.EnterState("CA");
                billPayPage.EnterZipCode("12345");
                billPayPage.EnterPhone("123456789");
                billPayPage.EnterAccount("11111");
                billPayPage.EnterVerifyAccount("11111");
                billPayPage.EnterAmount("50");

                // BƯỚC 30: Click Send Payment
                billPayPage.ClickSendPaymentButton();

                // BƯỚC 31: Kiểm tra thanh toán thành công
                string billStatus = billPayPage.GetResultMessage();

                // BƯỚC 32: Click Log Out
                driver.FindElement(By.LinkText("Log Out")).Click();

                // BƯỚC 33: Kiểm tra trở về trang Login
                var loginBtn = wait.Until(d => d.FindElement(By.CssSelector("input[value='Log In']")));

                if (billStatus.Contains("success") && loginBtn.Displayed)
                {
                    actualResult = "Cả 4 bước Register, Login, Bill Pay, Logout đều thành công. Hệ thống hoạt động đúng trong toàn bộ luồng chính";
                    isTestPassed = true;
                }
            }
            catch (Exception ex)
            {
                actualResult = "Lỗi tại bước thực thi: " + ex.Message;
                isTestPassed = false;
            }

            // Ghi kết quả vào Excel
            string resultStatus = isTestPassed ? "PASS" : "FAIL";
            ExcelDataProvider.WriteTestResults(testCase.Steps, actualResult, resultStatus, ExcelSheetName);

            Assert.That(isTestPassed, Is.True);
        }

        private static IEnumerable<object[]> GetSmokeTestCases()
        {
            foreach (var data in ExcelDataProvider.GetNamedTestCases(ExcelSheetName, TestCasePrefixFilter))
            {
                if (data.Length > 0 && AllowedTestCases.Contains(data[0].ToString()!.ToUpper()))
                    yield return data;
            }
        }
    }
}