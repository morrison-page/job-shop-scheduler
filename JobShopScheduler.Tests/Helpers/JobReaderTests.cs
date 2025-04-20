namespace JobShopScheduler.Tests.Helpers
{
    [TestClass]
    public sealed class JobReaderTests
    {
        // Valid CSV & Valid Data
        [TestMethod]
        public void ReadJobs_ValidInput_ReturnsExpectedJobs()
        {
            throw new NotImplementedException();
        }

        // Missing CSV
        [TestMethod]
        public void ReadJobs_MissingCsvFile_ReturnsError()
        {
            throw new NotImplementedException();
        }

        // Invalid CSV
        [TestMethod]
        public void ReadJobs_MissingHeader_ReturnsError()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void ReadJobs_MissingColumns_ReturnsError()
        {
            throw new NotImplementedException();
        }

        // Valid CSV & Invalid Data
        [TestMethod]
        public void ReadJobs_MissingRandomData_ReturnsError()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void ReadJobs_InvalidData_ReturnsError()
        {
            throw new NotImplementedException();
        }
    }
}
