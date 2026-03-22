using System.Globalization;
using DoAnCuoiKy.Models;
using DoAnCuoiKy_TH.DataProviders;
using DoAnCuoiKy_TH.Pages;
using DoAnCuoiKy_TH.Utilities;
using NUnit.Framework;
using OpenQA.Selenium;

namespace DoAnCuoiKy_TH.Tests.OpenNewAccount
{
    [TestFixture]
    public class OpenNewAccountTests : DriverFactory
    {
        #region Constants

        private const string ExcelSheetName = "TC_Nguyen";
        private const string TestCasePrefixFilter = "TC_";
        private const int MinEnabledTestCaseNumber = 12;
        private const int MaxEnabledTestCaseNumber = 17;

        #endregion

        private OpenNewAccountPage? openAccountPage;

        [TestCaseSource(nameof(GetVisibleOpenAccountTestCases))]
        public void ExecuteOpenNewAccountTestCase(string testCaseId, string function, string bigItem)
        {
            if (!ShouldRunTestCase(testCaseId, out _))
            {
                Assert.Ignore($"Skipping {testCaseId}. Only {TestCasePrefixFilter}{MinEnabledTestCaseNumber:00} to {TestCasePrefixFilter}{MaxEnabledTestCaseNumber:00} are enabled.");
            }

            var testCase = ExcelDataProvider.GetTestCaseById(ExcelSheetName, TestCasePrefixFilter, testCaseId);
            openAccountPage = new OpenNewAccountPage(driver);

            LogTestCaseInfo(testCaseId, function, bigItem);

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
            string updatedExcelRange = ExcelDataProvider.WriteTestResults(
                testCase.Steps, actualResult, resultStatus, ExcelSheetName, notes
            );

            LogTestResults(testCase.Expected, actualResult, resultStatus, updatedExcelRange);

            Assert.That(isTestPassed, Is.True,
                $"Test case {testCaseId} failed. Expected: {testCase.Expected}, Actual: {actualResult}");
        }

        private static IEnumerable<object[]> GetVisibleOpenAccountTestCases()
        {
            foreach (var testCaseData in ExcelDataProvider.GetNamedTestCases(ExcelSheetName, TestCasePrefixFilter))
            {
                if (testCaseData.Length == 0 || testCaseData[0] is not string testCaseId)
                    continue;

                if (ShouldRunTestCase(testCaseId, out _))
                    yield return testCaseData;
            }
        }

        private static bool ShouldRunTestCase(string testCaseId, out int caseNumber)
        {
            caseNumber = 0;

            if (string.IsNullOrWhiteSpace(testCaseId) ||
                !testCaseId.StartsWith(TestCasePrefixFilter, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            var numericPart = testCaseId[TestCasePrefixFilter.Length..];

            if (!int.TryParse(numericPart, NumberStyles.None, CultureInfo.InvariantCulture, out caseNumber))
            {
                return false;
            }

            return caseNumber >= MinEnabledTestCaseNumber && caseNumber <= MaxEnabledTestCaseNumber;
        }

        private void ExecuteTestStep(TestStep step)
        {
            var action = step.StepAction?.ToLower() ?? "";
            var testData = step.TestData ?? "";

            if (openAccountPage == null)
                throw new InvalidOperationException("OpenNewAccountPage is not initialized");

            if (action.Contains("username"))
            {
                openAccountPage.EnterUsername(testData);
            }
            else if (action.Contains("password"))
            {
                openAccountPage.EnterPassword(testData);
            }
            else if (action.Contains("login"))
            {
                openAccountPage.ClickLogin();
                Thread.Sleep(1000);
            }
            else if ((action.Contains("click") || action.Contains("submit")) && action.Contains("open new account"))
            {
                openAccountPage.ClickOpenNewAccount();
                Thread.Sleep(1000);
            }
            else if (action.Contains("navigate") || action.Contains("vào menu") || action.Contains("menu") || action.Contains("open new account"))
            {
                openAccountPage.NavigateToOpenNewAccount();
                Thread.Sleep(1500);
            }
            else if (action.Contains("account type") || action.Contains("loại tài khoản"))
            {
                openAccountPage.SelectAccountType(testData);
            }
            else if (action.Contains("from account") || action.Contains("tài khoản nguồn"))
            {
                openAccountPage.SelectFromAccount(testData);
            }
            else if (action.Contains("open") || action.Contains("submit"))
            {
                openAccountPage.ClickOpenNewAccount();
                Thread.Sleep(1000);
            }
        }

        private string GetActualResult()
        {
            if (openAccountPage == null)
                throw new InvalidOperationException("OpenNewAccountPage is not initialized");

            try
            {
                string title = openAccountPage.GetSuccessTitle();
                string accountId = openAccountPage.GetNewAccountId();
                string message = openAccountPage.GetResultMessage();

                if (!string.IsNullOrEmpty(title) && !title.Equals("Open New Account", StringComparison.OrdinalIgnoreCase))
                    return title;

                if (!string.IsNullOrEmpty(accountId))
                    return "Account Opened!";

                if (!string.IsNullOrEmpty(message))
                    return message;

                return "Open account completed";
            }
            catch (Exception ex)
            {
                return $"Error checking result: {ex.Message}";
            }
        }

        private static bool IsResultMatchingExpected(string expectedText, string actualText)
        {
            if (string.IsNullOrWhiteSpace(expectedText))
                return false;

            var expected = NormalizeText(expectedText);
            var actual = NormalizeText(actualText);

            return actual.Contains(expected) || expected.Contains(actual) || actual.Equals(expected);
        }

        private static string NormalizeText(string input)
        {
            return (input ?? string.Empty)
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
                if (!Directory.Exists(screenshotDirectory))
                {
                    Directory.CreateDirectory(screenshotDirectory);
                }

                string fileName = $"{testCaseId}_{DateTime.Now:yyyyMMdd_HHmmss}.png";
                string screenshotPath = Path.Combine(screenshotDirectory, fileName);

                var screenshot = ((ITakesScreenshot)driver).GetScreenshot();
                screenshot.SaveAsFile(screenshotPath);

                return screenshotPath;
            }
            catch (Exception ex)
            {
                return $"Failed to capture screenshot: {ex.Message}";
            }
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