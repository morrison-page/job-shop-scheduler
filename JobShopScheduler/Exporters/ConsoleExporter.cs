using JobShopScheduler.Interfaces;
using JobShopScheduler.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace JobShopScheduler.Exporters
{
    public class ConsoleExporter : IScheduleExporter 
    {
        public void Export(Schedule schedule)
        {
            Console.WriteLine("Press any key to display the next time slot or 'q' to quit...\n");
            Console.WriteLine("\nJob Schedule:\n");

            List<string> subdivisions = schedule.EvaluatedJobs
                .SelectMany(job => job.Operations)
                .Select(op => op.Subdivision)
                .Distinct()
                .ToList();

            const int timeColumnWidth = 19;
            const int columnWidth = 17;

            Console.Write($"+{new string('-', timeColumnWidth)}+");
            foreach (string subdivision in subdivisions)
            {
                Console.Write(new string('-', Math.Max(columnWidth, subdivision.Length + 2)) + "+");
            }
            Console.WriteLine();
            Console.Write($"| Time              |");
            foreach (string subdivision in subdivisions)
            {
                Console.Write($" {subdivision.PadRight(columnWidth - 2)} |");
            }
            Console.WriteLine();
            Console.Write($"+{new string('-', timeColumnWidth)}+");
            foreach (string subdivision in subdivisions)
            {
                Console.Write(new string('-', Math.Max(columnWidth, subdivision.Length + 2)) + "+");
            }
            Console.WriteLine();

            DateTime startTime = new DateTime(2023, 1, 2, 9, 0, 0); // Monday 09:00
            var timeSlots = schedule.EvaluatedJobs
                .SelectMany(job => job.Operations)
                .GroupBy(op => new { op.StartTime, op.EndTime })
                .OrderBy(g => g.Key.StartTime);

            foreach (var timeSlot in timeSlots)
            {
                var key = Console.ReadKey(intercept: true).Key;
                if (key == ConsoleKey.Q)
                {
                    Console.WriteLine("\nExiting operation display.");
                    return;
                }

                var slotStart = startTime.AddHours(timeSlot.Key.StartTime);
                var slotEnd = startTime.AddHours(timeSlot.Key.EndTime);
                Console.Write($"| {($"{slotStart:ddd HH:mm} - {slotEnd:HH:mm}").PadRight(columnWidth)} |");

                foreach (var subdivision in subdivisions)
                {
                    var operation = timeSlot.FirstOrDefault(op => op.Subdivision == subdivision);
                    if (operation != null)
                    {
                        Console.Write($"   Job{operation.JobId} - Op{operation.OperationId}".PadRight(Math.Max(columnWidth, subdivision.Length + 2)) + "|");
                    }
                    else
                    {
                        Console.Write(new string(' ', Math.Max(columnWidth, subdivision.Length + 2)) + "|");
                    }
                }
                Console.WriteLine();
            }

            Console.Write($"+-------------------+");
            foreach (var subdivision in subdivisions)
            {
                Console.Write(new string('-', Math.Max(columnWidth, subdivision.Length + 2)) + "+");
            }
            Console.WriteLine();
            Console.WriteLine($"\nMakespan: {schedule.Fitness} hours\n");

            Console.WriteLine("Export complete. Press 'q' to go back.");
            while (Console.ReadKey(intercept: true).Key != ConsoleKey.Q) { }
        }
    }
}
