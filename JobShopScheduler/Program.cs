using JobShopScheduler.Models;

namespace JobShopScheduler
{
    internal class Program
    {
        static void Main(string[] args)
        {
            List<JobOperation> operations = GetOperations();
            GeneticAlgorithm ga = new(operations);
            List<ScheduledOperation> schedule = ga.Solve();
            PrintSchedule(schedule);
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

        public static void PrintSchedule(List<ScheduledOperation> schedule)
        {
            int makespan = schedule.Max(op => op.EndTime);

            Console.WriteLine("\nJob Schedule:\n");
            Console.WriteLine("+--------+--------------+----------------------+------------+----------+----------+");
            Console.WriteLine("| Job ID | Operation ID | Subdivision          | Start Hour | End Hour | Duration |");
            Console.WriteLine("+--------+--------------+----------------------+------------+----------+----------+");

            List<ScheduledOperation> orderedOperations = schedule.OrderBy(op => op.StartTime).ToList();

            foreach (ScheduledOperation operation in orderedOperations)
            {
                Console.WriteLine($"| {operation.JobId,6}" +
                    $" | {operation.OperationId,12}" +
                    $" | {operation.Subdivision,-20}" +
                    $" | {operation.StartTime,10}" +
                    $" | {operation.EndTime,8}" +
                    $" | {operation.ProcessingTime,8}" +
                    $" |");
            }
            Console.WriteLine("+--------+--------------+----------------------+------------+----------+----------+");
            Console.WriteLine($"\nMakespan: {makespan}\n");
        }
    }
}
