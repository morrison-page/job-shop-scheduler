using JobShopScheduler.Helpers;
using JobShopScheduler.Interfaces;
using JobShopScheduler.Models;
using JobShopScheduler.Solvers;
using JobShopScheduler.Exporters;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace JobShopScheduler
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string filePath = "../../../Jobs/jobs_huge.csv";
            List<Job> jobs = JobReader.ReadJobs(filePath);

            Stopwatch sw = new();
            IScheduleSolver solver = new GeneticAlgorithm(jobs);

            sw.Start();
            Schedule schedule = solver.Solve();
            for (int i = 0; i < 40; i++)
            {
                Schedule compareSchedule = solver.Solve();
                if (compareSchedule.Fitness < schedule.Fitness)
                    schedule = compareSchedule;
            }
            sw.Stop();
            
            IScheduleExporter exporter = new ConsoleExporter();
            exporter.Export(schedule);
            Console.WriteLine($"{sw.ElapsedMilliseconds.ToString()}ms");
        }
    }
}
