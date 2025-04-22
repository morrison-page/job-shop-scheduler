using JobShopScheduler.Helpers;
using JobShopScheduler.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;

namespace JobShopScheduler.Tests.Helpers
{
    [TestClass]
    public sealed class JobReaderTests
    {
        // Valid CSV & Valid Data
        [TestMethod]
        public void ReadJobs_ValidInput_ReturnsExpectedJobs()
        {
            // Arrange
            string filePath = "valid_jobs.csv";
            File.WriteAllText(filePath, "JobId,OperationId,Subdivision,ProcessingTime\n1,101,A,5\n1,102,B,10\n2,201,C,15");

            // Act
            List<Job> jobs = JobReader.ReadJobs(filePath);

            // Assert
            Assert.AreEqual(2, jobs.Count, "Should return 2 jobs.");
            Assert.AreEqual(2, jobs[0].Operations.Count, "First job should have 2 operations.");
            Assert.AreEqual(1, jobs[0].JobId, "First job ID should be 1.");
            Assert.AreEqual(101, jobs[0].Operations[0].OperationId, "First operation ID should be 101.");
            Assert.AreEqual("A", jobs[0].Operations[0].Subdivision, "Subdivision should be 'A'.");
            Assert.AreEqual(5, jobs[0].Operations[0].ProcessingTime, "Processing time should be 5.");
        }

        // Missing CSV
        [TestMethod]
        [ExpectedException(typeof(FileNotFoundException))]
        public void ReadJobs_MissingCsvFile_ReturnsError()
        {
            // Arrange
            string filePath = "missing_file.csv";

            // Act
            JobReader.ReadJobs(filePath);
        }

        // Invalid CSV: Missing Header
        [TestMethod]
        [ExpectedException(typeof(InvalidDataException))]
        public void ReadJobs_MissingHeader_ReturnsError()
        {
            // Arrange
            string filePath = "missing_header.csv";
            File.WriteAllText(filePath, "1,101,A,5\n1,102,B,10");

            // Act
            JobReader.ReadJobs(filePath);
        }

        // Invalid CSV: Missing Columns
        [TestMethod]
        [ExpectedException(typeof(IndexOutOfRangeException))]
        public void ReadJobs_MissingColumns_ReturnsError()
        {
            // Arrange
            string filePath = "missing_columns.csv";
            File.WriteAllText(filePath, "JobId,OperationId,Subdivision,ProcessingTime\n1,101,A");

            // Act
            JobReader.ReadJobs(filePath);
        }

        // Valid CSV & Invalid Data: Missing Random Data
        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void ReadJobs_MissingRandomData_ReturnsError()
        {
            // Arrange
            string filePath = "missing_random_data.csv";
            File.WriteAllText(filePath, "JobId,OperationId,Subdivision,ProcessingTime\n1,,A,5");

            // Act
            JobReader.ReadJobs(filePath);
        }

        // Valid CSV & Invalid Data: Invalid Data
        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void ReadJobs_InvalidData_ReturnsError()
        {
            // Arrange
            string filePath = "invalid_data.csv";
            File.WriteAllText(filePath, "JobId,OperationId,Subdivision,ProcessingTime\n1,101,A,InvalidTime");

            // Act
            JobReader.ReadJobs(filePath);
        }
    }
}
