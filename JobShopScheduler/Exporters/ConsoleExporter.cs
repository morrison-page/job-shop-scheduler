using JobShopScheduler.Interfaces;
using JobShopScheduler.Models;
using System;

namespace JobShopScheduler.Exporters
{
    public class ConsoleExporter : IScheduleExporter 
    {
        public void Export(Schedule schedule)
        {
            Console.WriteLine("\nJob Schedule:\n");
            Console.WriteLine("+--------+--------------+----------------------+------------+----------+----------+");
            Console.WriteLine("| Job ID | Operation ID | Subdivision          | Start Hour | End Hour | Duration |");
            Console.WriteLine("+--------+--------------+----------------------+------------+----------+----------+");

            foreach (Job job in schedule.EvaluatedJobs)
            {
                foreach (Operation op in job.Operations)
                {
                    Console.WriteLine($"| {op.JobId,6} | {op.OperationId,12} | {op.Subdivision,-20} | {op.StartTime,10} | {op.EndTime,8} | {op.ProcessingTime,8} |");
                }
            }

            Console.WriteLine("+--------+--------------+----------------------+------------+----------+----------+");
            Console.WriteLine($"\nMakespan: {schedule.Fitness} hours\n");
        }
    }
}
