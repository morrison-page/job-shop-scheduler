using JobShopScheduler.Models;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace JobShopScheduler.Helpers
{
    public static class JobReader
    {
        public static List<Job> ReadJobs(string filePath)
        {
            string[] lines = File.ReadAllLines(filePath);

            if (lines.Length == 0 || lines[0] != "JobId,OperationId,Subdivision,ProcessingTime")
            {
                throw new InvalidDataException("The CSV file is missing the required header.");
            }

            List<Operation> operations = lines.Skip(1) // Skip header
                .Select(static line => line.Split(','))
                .Select(static values => new Operation
                {
                    JobId = int.Parse(values[0]),
                    OperationId = int.Parse(values[1]),
                    Subdivision = values[2],
                    ProcessingTime = int.Parse(values[3])
                })
                .ToList();

            // Group operations by JobId
            IEnumerable<Job> jobGroups = operations
                .GroupBy(op => op.JobId)
                .Select(static group => new Job
                {
                    JobId = group.Key,
                    Operations = group
                        .OrderBy(static op => op.OperationId) // Ensure operations are in order
                        .ToList()
                });

            return jobGroups.ToList();
        }
    }
}
