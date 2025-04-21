using JobShopScheduler.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JobShopScheduler.Tests.Models
{
    [TestClass]
    public sealed class OperationTests
    {
        [TestMethod]
        public void Operation_DefaultValues_AreCorrect()
        {
            // Arrange
            Operation operation = new();

            // Assert
            Assert.AreEqual(0, operation.JobId, "Default JobId should be 0.");
            Assert.AreEqual(0, operation.OperationId, "Default OperationId should be 0.");
            Assert.AreEqual(string.Empty, operation.Subdivision, "Default Subdivision should be an empty string.");
            Assert.AreEqual(0, operation.ProcessingTime, "Default ProcessingTime should be 0.");
            Assert.AreEqual(0, operation.StartTime, "Default StartTime should be 0.");
        }

        [TestMethod]
        public void Operation_PropertyInitialisation_IsCorrect()
        {
            // Arrange
            Operation operation = new()
            {
                JobId = 1,
                OperationId = 101,
                Subdivision = "Assembly",
                ProcessingTime = 5,
                StartTime = 10
            };

            // Assert
            Assert.AreEqual(1, operation.JobId, "JobId should be initialised correctly.");
            Assert.AreEqual(101, operation.OperationId, "OperationId should be initialised correctly.");
            Assert.AreEqual("Assembly", operation.Subdivision, "Subdivision should be initialised correctly.");
            Assert.AreEqual(5, operation.ProcessingTime, "ProcessingTime should be initialised correctly.");
            Assert.AreEqual(10, operation.StartTime, "StartTime should be initialised correctly.");
        }

        [TestMethod]
        public void Operation_EndTimeCalculation_IsCorrect()
        {
            // Arrange
            Operation operation = new()
            {
                StartTime = 10,
                ProcessingTime = 5
            };

            // Act
            int endTime = operation.EndTime;

            // Assert
            Assert.AreEqual(15, endTime, "EndTime should be StartTime + ProcessingTime.");
        }

        [TestMethod]
        public void Operation_StartTime_UpdatesEndTime()
        {
            // Arrange
            Operation operation = new()
            {
                StartTime = 10,
                ProcessingTime = 5
            };

            // Act
            operation.StartTime = 20;

            // Assert
            Assert.AreEqual(25, operation.EndTime, "EndTime should update when StartTime changes.");
        }
    }
}
