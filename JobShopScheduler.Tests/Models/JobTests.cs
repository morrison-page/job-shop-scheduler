using JobShopScheduler.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace JobShopScheduler.Tests.Models
{
    [TestClass]
    public sealed class JobTests
    {
        [TestMethod]
        public void Job_DefaultValues_AreCorrect()
        {
            // Arrange
            Job job = new();

            // Assert
            Assert.AreEqual(0, job.JobId, "Default JobId should be 0.");
            Assert.AreEqual(0, job.NextOpIndex, "Default NextOpIndex should be 0.");
            Assert.IsNotNull(job.Operations, "Operations list should not be null.");
            Assert.AreEqual(0, job.Operations.Count, "Operations list should be empty by default.");
        }

        [TestMethod]
        public void Job_PropertyInitialisation_IsCorrect()
        {
            // Arrange
            List<Operation> operations = new()
            {
                new Operation { JobId = 1, OperationId = 101 },
                new Operation { JobId = 1, OperationId = 102 }
            };

            Job job = new()
            {
                JobId = 1,
                Operations = operations,
                NextOpIndex = 1
            };

            // Assert
            Assert.AreEqual(1, job.JobId, "JobId should be initialised correctly.");
            Assert.AreEqual(operations, job.Operations, "Operations should be initialised correctly.");
            Assert.AreEqual(1, job.NextOpIndex, "NextOpIndex should be initialised correctly.");
        }

        [TestMethod]
        public void Job_HasNextOperation_ReturnsTrue_WhenOperationsRemain()
        {
            // Arrange
            Job job = new()
            {
                Operations = new List<Operation>
                {
                    new Operation { JobId = 1, OperationId = 101 }
                },
                NextOpIndex = 0
            };

            // Act
            bool hasNext = job.HasNextOperation;

            // Assert
            Assert.IsTrue(hasNext, "HasNextOperation should return true when operations remain.");
        }

        [TestMethod]
        public void Job_HasNextOperation_ReturnsFalse_WhenNoOperationsRemain()
        {
            // Arrange
            Job job = new()
            {
                Operations = new List<Operation>
                {
                    new Operation { JobId = 1, OperationId = 101 }
                },
                NextOpIndex = 1
            };

            // Act
            bool hasNext = job.HasNextOperation;

            // Assert
            Assert.IsFalse(hasNext, "HasNextOperation should return false when no operations remain.");
        }

        [TestMethod]
        public void Job_GetNextOperation_ReturnsCorrectOperation()
        {
            // Arrange
            List<Operation> operations = new()
            {
                new Operation { JobId = 1, OperationId = 101 },
                new Operation { JobId = 1, OperationId = 102 }
            };

            Job job = new()
            {
                Operations = operations,
                NextOpIndex = 0
            };

            // Act
            Operation nextOperation = job.GetNextOperation();

            // Assert
            Assert.AreEqual(operations[0], nextOperation, "GetNextOperation should return the correct operation.");
            Assert.AreEqual(1, job.NextOpIndex, "NextOpIndex should increment after calling GetNextOperation.");
        }

        [TestMethod]
        [ExpectedException(typeof(System.ArgumentOutOfRangeException))]
        public void Job_GetNextOperation_ThrowsException_WhenNoOperationsRemain()
        {
            // Arrange
            Job job = new()
            {
                Operations = new List<Operation>(),
                NextOpIndex = 0
            };

            // Act
            job.GetNextOperation();
        }

        [TestMethod]
        public void Job_Reset_SetsNextOpIndexToZero()
        {
            // Arrange
            Job job = new()
            {
                Operations = new List<Operation>
                {
                    new Operation { JobId = 1, OperationId = 101 }
                },
                NextOpIndex = 1
            };

            // Act
            job.Reset();

            // Assert
            Assert.AreEqual(0, job.NextOpIndex, "Reset should set NextOpIndex to 0.");
        }
    }
}
