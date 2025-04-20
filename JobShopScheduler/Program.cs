using JobShopScheduler.Helpers;
using JobShopScheduler.Interfaces;
using JobShopScheduler.Models;
using JobShopScheduler.Solvers;
using JobShopScheduler.Writers;
using System.Collections.Generic;

namespace JobShopScheduler
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string filePath = "../../../Jobs/jobs_small.csv";
            List<Job> jobs = JobReader.ReadJobs(filePath);

            IScheduleSolver solver = new GeneticAlgorithm(jobs);
            Schedule schedule = solver.Solve();

            IScheduleExporter exporter = new ConsoleExporter();
            exporter.Export(schedule, jobs);
        }
    }
}
