using JobShopScheduler.Interfaces;
using JobShopScheduler.Models;
using JobShopScheduler.Solvers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace JobShopScheduler.Writers
{
    internal class ConsoleExporter : IScheduleExporter 
    {
        public void Export(Schedule schedule, List<Job> jobs)
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
