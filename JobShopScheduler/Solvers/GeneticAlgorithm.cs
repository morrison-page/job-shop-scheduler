using JobShopScheduler.Interfaces;
using JobShopScheduler.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JobShopScheduler.Solvers
{
    public class GeneticAlgorithm : IScheduleSolver
    {
        private const int POPULATION_SIZE = 50;
        private const int GENERATION_QUANTITY = 50;
        private const int SELECTION_CANDIDATES_PER_ROUND = 5;
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
                Parallel.ForEach(population, schedule =>
                {
                    schedule.Fitness = Evaluate(schedule, _jobs.Select(j => new Job
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
                });

                population = population.OrderBy(c => c.Fitness).ToList();
                if (population[0].Fitness < best.Fitness)
                    best = population[0].Clone();

                List<Schedule> newPopulation = new(new Schedule[POPULATION_SIZE]);
                newPopulation[0] = best.Clone(); // elitism

                Parallel.For(1, POPULATION_SIZE, i =>
                {
                    Schedule parent1 = Selection(population);
                    Schedule parent2 = Selection(population);

                    Schedule child = Crossover(parent1, parent2);
                    Mutate(child);

                    newPopulation[i] = child;
                });

                population = newPopulation;
            }

            List<Job> finalEvaluatedJobs = _jobs.Select(j => new Job
            {
                JobId = j.JobId,
                Operations = j.Operations.Select(o => new Operation
                {
                    JobId = o.JobId,
                    OperationId = o.OperationId,
                    Subdivision = o.Subdivision,
                    ProcessingTime = o.ProcessingTime
                }).ToList()
            }).ToList();

            Evaluate(best, finalEvaluatedJobs);
            best.EvaluatedJobs = finalEvaluatedJobs;

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

        private int Evaluate(Schedule schedule, List<Job> jobs)
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

        private Schedule Selection(List<Schedule> pop) => pop.OrderBy(_ => _random.Next()).Take(SELECTION_CANDIDATES_PER_ROUND).OrderBy(static c => c.Fitness).First();

        private Schedule Crossover(Schedule a, Schedule b)
        {
            int size = a.JobSequence.Count;
            List<int> childGenes = new(new int[size]);
            Dictionary<int, int> seen = new();

            int start = _random.Next(size / 2);
            int end = _random.Next(start + 1, size);

            // Copy segment from parent A
            for (int i = start; i < end; i++)
            {
                childGenes[i] = a.JobSequence[i];
                seen.TryAdd(childGenes[i], 0);
                seen[childGenes[i]]++;
            }

            // Fill rest from B
            int bIndex = 0;
            for (int i = 0; i < size; i++)
            {
                if (i >= start && i < end) continue;
                while (seen.TryGetValue(b.JobSequence[bIndex], out int count) &&
                       count >= _jobs.First(j => j.JobId == b.JobSequence[bIndex]).Operations.Count)
                    bIndex++;

                int gene = b.JobSequence[bIndex++];
                childGenes[i] = gene;
                seen.TryAdd(gene, 0);
                seen[gene]++;
            }

            return new Schedule { JobSequence = childGenes };
        }

        private void Mutate(Schedule s)
        {
            int a = _random.Next(s.JobSequence.Count);
            int b = _random.Next(s.JobSequence.Count);
            (s.JobSequence[a], s.JobSequence[b]) = (s.JobSequence[b], s.JobSequence[a]);
        }
    }
}
