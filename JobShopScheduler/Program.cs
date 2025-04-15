using JobShopScheduler.Algorithms;
using JobShopScheduler.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace JobShopScheduler
{
    internal class Program
    {
        static void Main(string[] args)
        {
            List<Job> jobs = GetJobs();
            GeneticAlgorithm ga = new(jobs);
            Schedule schedule = ga.Solve();
            PrintSchedule(schedule, jobs);
        }

        public static List<Job> GetJobs()
        {
            string dataSetPath = "../../../Jobs/jobs_small.csv";
            string[] lines = File.ReadAllLines(dataSetPath);

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

        public static void PrintSchedule(Schedule schedule, List<Job> jobs)
        {
            // Deep copy of jobs to avoid mutating original job state
            List<Job> jobsCopy = jobs.Select(static j => new Job
            {
                JobId = j.JobId,
                Operations = j.Operations.Select(static o => new Operation
                {
                    JobId = o.JobId,
                    OperationId = o.OperationId,
                    Subdivision = o.Subdivision,
                    ProcessingTime = o.ProcessingTime
                }).ToList()
            }).ToList();

            // Run scheduler to populate start/end times
            int makespan = GeneticAlgorithm.Evaluate(schedule, jobsCopy);

            // Flatten all operations from all jobs
            List<Operation> scheduledOperations = jobsCopy
                .SelectMany(j => j.Operations)
                .OrderBy(op => op.StartTime)
                .ToList();

            // Print formatted schedule
            Console.WriteLine("\nJob Schedule:\n");
            Console.WriteLine("+--------+--------------+----------------------+------------+----------+----------+");
            Console.WriteLine("| Job ID | Operation ID | Subdivision          | Start Hour | End Hour | Duration |");
            Console.WriteLine("+--------+--------------+----------------------+------------+----------+----------+");

            foreach (Operation op in scheduledOperations)
            {
                Console.WriteLine($"| {op.JobId,6} | {op.OperationId,12} | {op.Subdivision,-20} | {op.StartTime,10} | {op.EndTime,8} | {op.ProcessingTime,8} |");
            }

            Console.WriteLine("+--------+--------------+----------------------+------------+----------+----------+");
            Console.WriteLine($"\nMakespan: {makespan} hours\n");
        }

    }
}
