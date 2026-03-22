using System.Globalization;
using DoAnCuoiKy.Models;
using DoAnCuoiKy_TH.DataProviders;
using DoAnCuoiKy_TH.Pages;
using DoAnCuoiKy_TH.Utilities;
using NUnit.Framework;
using OpenQA.Selenium;

namespace DoAnCuoiKy_TH.Tests.UpdateContactInfo
{
    [TestFixture]
    public class UpdateContactInfoTests : DriverFactory
    {
        #region Constants

        private const string ExcelSheetName = "TC_Nguyen";
        private const string TestCasePrefixFilter = "TC_";
        private const int MinEnabledTestCaseNumber = 17;
        private const int MaxEnabledTestCaseNumber = 21;

        #endregion

        private UpdateContactInfoPage? updatePage;

        [TestCaseSource(nameof(GetVisibleUpdateContactTestCases))]
        public void ExecuteUpdateContactTestCase(string testCaseId, string function, string bigItem)
        {
            if (!ShouldRunTestCase(testCaseId, out _))
            {
                Assert.Ignore($"Skipping {testCaseId}. Only {TestCasePrefixFilter}{MinEnabledTestCaseNumber:00} to {TestCasePrefixFilter}{MaxEnabledTestCaseNumber:00} are enabled.");
            }

            var testCase = ExcelDataProvider.GetTestCaseById(ExcelSheetName, TestCasePrefixFilter, testCaseId);
            updatePage = new UpdateContactInfoPage(driver);

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

        private static IEnumerable<object[]> GetVisibleUpdateContactTestCases()
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

            if (updatePage == null)
                throw new InvalidOperationException("UpdateContactInfoPage is not initialized");

            if (action.Contains("username"))
            {
                updatePage.EnterUsername(testData);
            }
            else if (action.Contains("password"))
            {
                updatePage.EnterPassword(testData);
            }
            else if (action.Contains("login"))
            {
                updatePage.ClickLogin();
                Thread.Sleep(1000);
            }
            else if ((action.Contains("click") || action.Contains("submit")) && (action.Contains("update profile") || action.Contains("update")))
            {
                updatePage.ClickUpdate();
                Thread.Sleep(1000);
            }
            else if (action.Contains("navigate") || action.Contains("vào menu") || action.Contains("menu") || action.Contains("update contact"))
            {
                updatePage.NavigateToUpdateContact();
                Thread.Sleep(1500);
            }
            else if (action.Contains("first name") || action.Contains("tên"))
            {
                updatePage.EnterFirstName(testData);
            }
            else if (action.Contains("last name") || action.Contains("họ"))
            {
                updatePage.EnterLastName(testData);
            }
            else if (action.Contains("address") || action.Contains("địa chỉ"))
            {
                updatePage.EnterAddress(testData);
            }
            else if (action.Contains("city") || action.Contains("thành phố"))
            {
                updatePage.EnterCity(testData);
            }
            else if (action.Contains("state") || action.Contains("tỉnh"))
            {
                updatePage.EnterState(testData);
            }
            else if (action.Contains("zip") || action.Contains("mã bưu điện"))
            {
                updatePage.EnterZipCode(testData);
            }
            else if (action.Contains("phone") || action.Contains("số điện thoại"))
            {
                updatePage.EnterPhone(testData);
            }
            else if (action.Contains("update") || action.Contains("submit"))
            {
                updatePage.ClickUpdate();
                Thread.Sleep(1000);
            }
        }

        private string GetActualResult()
        {
            if (updatePage == null)
                throw new InvalidOperationException("UpdateContactInfoPage is not initialized");

            try
            {
                string firstNameError = updatePage.GetFirstNameError();
                if (!string.IsNullOrWhiteSpace(firstNameError))
                    return firstNameError;

                string lastNameError = updatePage.GetLastNameError();
                if (!string.IsNullOrWhiteSpace(lastNameError))
                    return lastNameError;

                string streetError = updatePage.GetStreetError();
                if (!string.IsNullOrWhiteSpace(streetError))
                    return streetError;

                string cityError = updatePage.GetCityError();
                if (!string.IsNullOrWhiteSpace(cityError))
                    return cityError;

                string stateError = updatePage.GetStateError();
                if (!string.IsNullOrWhiteSpace(stateError))
                    return stateError;

                string zipCodeError = updatePage.GetZipCodeError();
                if (!string.IsNullOrWhiteSpace(zipCodeError))
                    return zipCodeError;

                string title = updatePage.GetSuccessTitle();
                string message = updatePage.GetResultMessage();

                if (!string.IsNullOrEmpty(title)
                    && !title.Equals("Update Contact Info", StringComparison.OrdinalIgnoreCase)
                    && !title.Equals("Update Profile", StringComparison.OrdinalIgnoreCase))
                {
                    return title;
                }

                if (!string.IsNullOrEmpty(message))
                {
                    if (message.Contains("updated address and phone number", StringComparison.OrdinalIgnoreCase)
                        || message.Contains("have been added to the system", StringComparison.OrdinalIgnoreCase))
                    {
                        return "Profile Updated";
                    }

                    return message;
                }

                return "Update completed";
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