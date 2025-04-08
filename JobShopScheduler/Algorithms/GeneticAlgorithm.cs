using JobShopScheduler.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace JobShopScheduler.Algorithms
{
    public class GeneticAlgorithm
    {
        private const int POPULATION_SIZE = 100;
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
    }
}
