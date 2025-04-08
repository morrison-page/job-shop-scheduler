using JobShopScheduler.Models;

namespace JobShopScheduler
{
    internal class Program
    {
        static void Main(string[] args)
        {
            List<JobOperation> operations = GetOperations();
        }

        public static List<JobOperation> GetOperations()
        {
            string dataSetPath = "../../../Jobs/jobs_small.csv";
            string[] lines = File.ReadAllLines(dataSetPath);

            List<JobOperation> operations = lines.Skip(1)
                .Select(line => line.Split(','))
                .Select(values => new JobOperation
                {
                    JobId = int.Parse(values[0]),
                    OperationId = int.Parse(values[1]),
                    Subdivision = values[2],
                    ProcessingTime = int.Parse(values[3])
                })
                .ToList();

            return operations;
        }
    }
}
