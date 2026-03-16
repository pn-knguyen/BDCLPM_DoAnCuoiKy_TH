using DoAnCuoiKy.Models;
using DoAnCuoiKy_TH.DataProviders;
using NUnit.Framework;

namespace DoAnCuoiKy_TH.Tests
{
    /// <summary>
    /// Tests for verifying Excel data provider functionality.
    /// </summary>
    [TestFixture]
    public class ExcelDataProviderTests
    {
        /// <summary>
        /// Verifies that the Excel provider can successfully read test case data from the file.
        /// </summary>
        [Test]
        public void TestReadExcelFile_ShouldLoadTestCasesSuccessfully()
        {
            // Arrange
            const string excelSheetName = "TC_Nguyen";
            const string testCaseFilter = "TC_";

            // Act
            var testCases = ExcelDataProvider.GetTestCases(excelSheetName, testCaseFilter);

            // Assert
            Assert.That(testCases, Is.Not.Null, "Test cases collection should not be null");
            Assert.That(testCases.Any(), Is.True, "Should load at least one test case from Excel");

            // Log test case details
            foreach (var testCase in testCases)
            {
                LogTestCaseDetails(testCase);
            }
        }

        /// <summary>
        /// Verifies that test cases contain valid structure and data.
        /// </summary>
        [Test]
        public void TestExcelData_ShouldContainValidTestCaseStructure()
        {
            // Arrange
            const string excelSheetName = "TC_Nguyen";
            const string testCaseFilter = "TC_";

            // Act
            var testCases = ExcelDataProvider.GetTestCases(excelSheetName, testCaseFilter).ToList();

            // Assert
            Assert.That(testCases, Is.Not.Empty, "At least one test case should be present");

            foreach (var testCase in testCases)
            {
                Assert.That(testCase.TestCaseId, Is.Not.Null.And.Not.Empty, "Test case ID should not be empty");
                Assert.That(testCase.Steps, Is.Not.Null, "Steps should not be null");
                Assert.That(testCase.Steps.Count, Is.GreaterThan(0), "Test case should have at least one step");
            }
        }

        /// <summary>
        /// Logs the details of a test case for verification purposes.
        /// </summary>
        private void LogTestCaseDetails(DoAnCuoiKy.Models.TestCaseData testCase)
        {
            TestContext.Out.WriteLine("════════════════════════════════════════");
            TestContext.Out.WriteLine($"Test Case ID: {testCase.TestCaseId}");
            TestContext.Out.WriteLine($"Function: {testCase.Function}");
            TestContext.Out.WriteLine($"Big Item: {testCase.BigItem}");
            TestContext.Out.WriteLine($"Medium Item: {testCase.MediumItem}");
            TestContext.Out.WriteLine($"Small Item: {testCase.SmallItem}");
            TestContext.Out.WriteLine($"Pre-Condition: {testCase.PreCondition}");
            TestContext.Out.WriteLine($"Number of Steps: {testCase.Steps.Count}");

            foreach (var step in testCase.Steps)
            {
                TestContext.Out.WriteLine($"  Step {step.StepNumber}: {step.StepAction}");
                TestContext.Out.WriteLine($"    Test Data: {step.TestData}");
            }

            TestContext.Out.WriteLine($"Expected Result: {testCase.Expected}");
            TestContext.Out.WriteLine("════════════════════════════════════════");
        }
    }
}