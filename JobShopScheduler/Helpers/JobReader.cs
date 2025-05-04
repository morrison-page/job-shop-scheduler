using JobShopScheduler.Models;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace JobShopScheduler.Helpers
{
    /// <summary>
    /// Provides functionality to read and parse job data from a CSV file.
    /// </summary>
    public static class JobReader
    {
        /// <summary>
        /// Reads job data from a CSV file and converts it into a list of <see cref="Job"/> objects.
        /// </summary>
        /// <param name="filePath">The path to the CSV file containing job data.</param>
        /// <returns>A list of <see cref="Job"/> objects, each containing its associated operations.</returns>
        /// <exception cref="InvalidDataException">Thrown if the CSV file is missing the required header.</exception>
        public static List<Job> ReadJobs(string filePath)
        {
            string[] lines = File.ReadAllLines(filePath);

            // Validate the file header.
            if (lines.Length == 0 || lines[0] != "JobId,OperationId,Subdivision,ProcessingTime")
            {
                throw new InvalidDataException("The CSV file is missing the required header.");
            }

            // Parse operations from the file, skipping the header.
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

            // Group operations by JobId and create Job objects.
            IEnumerable<Job> jobGroups = operations
                .GroupBy(op => op.JobId)
                .Select(static group => new Job
                {
                    JobId = group.Key,
                    Operations = group
                        .OrderBy(static op => op.OperationId)
                        .ToList()
                });

            return jobGroups.ToList();
        }
    }
}
