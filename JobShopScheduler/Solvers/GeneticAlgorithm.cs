using JobShopScheduler.Interfaces;
using JobShopScheduler.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JobShopScheduler.Solvers
{
    /// <summary>
    /// Implements a genetic algorithm to solve the job shop scheduling problem.
    /// </summary>
    public class GeneticAlgorithm : IScheduleSolver
    {
        private const int POPULATION_SIZE = 50;
        private const int GENERATION_QUANTITY = 50;
        private const int SELECTION_CANDIDATES_PER_ROUND = 5;
        private readonly Random _random = new();
        private readonly List<Job> _jobs;

        /// <summary>
        /// Initialises a new instance of the <see cref="GeneticAlgorithm"/> class.
        /// </summary>
        /// <param name="jobs">The list of jobs to be scheduled.</param>
        public GeneticAlgorithm(List<Job> jobs)
        {
            _jobs = jobs;
        }

        /// <summary>
        /// Solves the scheduling problem using a genetic algorithm and returns the best schedule.
        /// </summary>
        /// <returns>A <see cref="Schedule"/> object representing the optimal or near-optimal solution.</returns>
        public Schedule Solve()
        {
            List<Schedule> population = InitialisePopulation();

            Schedule best = population[0];

            for (int g = 0; g < GENERATION_QUANTITY; g++)
            {
                // Evaluate the fitness of each schedule in the population.
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

                // Sort the population by fitness and update the best schedule.
                population = population.OrderBy(c => c.Fitness).ToList();
                if (population[0].Fitness < best.Fitness)
                    best = population[0].Clone();

                // Create a new population using elitism, crossover, and mutation.
                List<Schedule> newPopulation = new(new Schedule[POPULATION_SIZE]);
                newPopulation[0] = best.Clone(); // Elitism

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

            // Evaluate the best schedule with the final job states.
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

        /// <summary>
        /// Initialises the population of schedules with random job sequences.
        /// </summary>
        /// <returns>A list of randomly generated <see cref="Schedule"/> objects.</returns>
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

        /// <summary>
        /// Evaluates the fitness of a schedule by simulating the job execution.
        /// </summary>
        /// <param name="schedule">The schedule to evaluate.</param>
        /// <param name="jobs">The list of jobs to simulate.</param>
        /// <returns>The fitness value of the schedule, representing the makespan.</returns>
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

        /// <summary>
        /// Selects a parent schedule from the population using tournament selection.
        /// </summary>
        /// <param name="pop">The population of schedules.</param>
        /// <returns>The selected <see cref="Schedule"/>.</returns>
        private Schedule Selection(List<Schedule> pop) =>
            pop.OrderBy(_ => _random.Next())
               .Take(SELECTION_CANDIDATES_PER_ROUND)
               .OrderBy(static c => c.Fitness)
               .First();

        /// <summary>
        /// Performs a Partially Mapped Crossover (PMX) variant between two parent schedules
        /// to produce a child schedule. This ensures the child inherits a valid sequence
        /// of job operations while preserving genetic diversity.
        /// </summary>
        /// <param name="a">The first parent schedule.</param>
        /// <param name="b">The second parent schedule.</param>
        /// <returns>A new <see cref="Schedule"/> object representing the child.</returns>
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

        /// <summary>
        /// Mutates a schedule by swapping two random genes in its job sequence.
        /// </summary>
        /// <remarks>
        /// This uses a swap mutation strategy, where two random positions in the job sequence
        /// are selected, and their values are swapped. This ensures that the mutation returns a 
        /// valid schedule while introducing small random changes.
        /// </remarks>
        /// <param name="s">The schedule to mutate.</param>
        private void Mutate(Schedule s)
        {
            int a = _random.Next(s.JobSequence.Count);
            int b = _random.Next(s.JobSequence.Count);
            (s.JobSequence[a], s.JobSequence[b]) = (s.JobSequence[b], s.JobSequence[a]);
        }
    }
}
