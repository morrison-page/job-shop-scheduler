using JobShopScheduler.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace JobShopScheduler.Algorithms
{
    public class GeneticAlgorithm
    {
        private const int POPULATION_SIZE = 100;
        private const int GENERATION_QUANTITY = 100;
        private readonly Random _random = new();
        private readonly List<Job> _jobs;

        public GeneticAlgorithm(List<Job> jobs)
        {
            _jobs = jobs;
        }

        public Schedule Solve()
        {
            List<Schedule> population = InitialisePopulation();

            Schedule best = population[0];

            for (int g = 0; g < GENERATION_QUANTITY; g++)
            {
                foreach (Schedule s in population)
                    s.Fitness = Evaluate(s, _jobs.Select(j => new Job
                    {
                        JobId = j.JobId,
                        Operations = j.Operations.Select(o => new Operation
                        {
                            JobId = o.JobId,
                            OperationId = o.OperationId,
                            Subdivision = o.Subdivision,
                            ProcessingTime = o.ProcessingTime
                        }).ToList()
                    }).ToList());
            }

            return best;
        }

        private List<Schedule> InitialisePopulation()
        {
            List<int> genePool = _jobs.SelectMany(j => Enumerable.Repeat(j.JobId, j.Operations.Count)).ToList();
            List<Schedule> population = new();

            for (int i = 0; i < POPULATION_SIZE; i++)
            {
                List<int> genes = genePool.OrderBy(_ => _random.Next()).ToList();
                population.Add(new Schedule { JobSequence = genes });
            }

            return population;
        }

        public static int Evaluate(Schedule schedule, List<Job> jobs)
        {
            Dictionary<string, int> machineAvailability = new();
            Dictionary<int, int> jobOperationAvailability = new();

            // Reset job state
            foreach (Job job in jobs)
            {
                job.Reset();
                jobOperationAvailability[job.JobId] = 0;
            }

            int globalEndTime = 0;

            foreach (int jobId in schedule.JobSequence)
            {
                Job job = jobs.First(j => j.JobId == jobId);
                Operation op = job.GetNextOperation();

                string machine = op.Subdivision;

                int readyTime = Math.Max(
                    jobOperationAvailability[jobId],
                    machineAvailability.TryGetValue(machine, out int available) ? available : 0
                );

                op.StartTime = readyTime;
                machineAvailability[machine] = op.EndTime;
                jobOperationAvailability[jobId] = op.EndTime;

                globalEndTime = Math.Max(globalEndTime, op.EndTime);
            }

            return globalEndTime;
        }
    }

}
