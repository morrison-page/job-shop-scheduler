using JobShopScheduler.Exporters;
using JobShopScheduler.Models;
using JobShopScheduler.Solvers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace JobShopScheduler.Tests.Solvers
{
    [TestClass]
    public sealed class GeneticAlgorithmTests
    {
        private List<Job> CreateMockJobs(int minJobs, int maxJobs, int minOperations, int maxOperations)
        {
            Random random = new();
            List<Job> jobs = new();

            int jobQuantity = random.Next(minJobs, maxJobs + 1);

            for (int jobId = 1; jobId <= jobQuantity; jobId++)
            {
                int operationQuantity = random.Next(minOperations, maxOperations + 1);
                List<Operation> operations = new();

                for (int operationId = 1; operationId <= operationQuantity; operationId++)
                {
                    operations.Add(new Operation
                    {
                        JobId = jobId,
                        OperationId = operationId,
                        Subdivision = $"Subdivision{random.Next(1, 4)}",
                        ProcessingTime = random.Next(1, 10)
                    });
                }

                jobs.Add(new Job
                {
                    JobId = jobId,
                    Operations = operations
                });
            }

            return jobs;
        }

        [TestMethod]
        public void Solve_ReturnedSchedule_IsValid()
        {
            // Arrange
            List<Job> jobs = CreateMockJobs(
                minJobs: 1,
                maxJobs: 60,
                minOperations: 1,
                maxOperations: 10
            );

            GeneticAlgorithm solver = new(jobs);
            ConsoleExporter exporter = new();

            // Act
            Schedule schedule = solver.Solve();
            exporter.Export(schedule, jobs); // Export the schedule to console

            // Assert
            Dictionary<string, int> machineAvailability = new();
            Dictionary<int, int> jobOperationAvailability = new();

            foreach (Job job in schedule.EvaluatedJobs)
            {
                job.Reset(); // Reset job state
                jobOperationAvailability[job.JobId] = 0;
            }

            foreach (int jobId in schedule.JobSequence)
            {
                Job job = schedule.EvaluatedJobs.First(j => j.JobId == jobId);
                Operation op = job.GetNextOperation();

                // Validate operation order within the job
                Assert.IsTrue(op.OperationId > 0, "OperationId must be positive.");
                // Assertion isn't using == as the operationId isn't guaranteed to be sequential
                Assert.IsTrue(op.OperationId >= job.NextOpIndex, $"Operations must be executed in order. Job|Operation:{job.JobId}|{op.OperationId}");

                // Validate start time constraints
                string machine = op.Subdivision;
                int readyTime = Math.Max(
                    jobOperationAvailability[jobId],
                    machineAvailability.TryGetValue(machine, out int available) ? available : 0
                );

                Assert.IsTrue(op.StartTime >= readyTime, $"Operation StartTime must respect machine and job availability. Job|Operation:{job.JobId}|{op.OperationId}");

                // Update availability
                machineAvailability[machine] = op.EndTime;
                jobOperationAvailability[jobId] = op.EndTime;
            }
        }

        [TestMethod]
        public void Solve_ImprovesFitness()
        {
            // Arrange
            List<Job> jobs = CreateMockJobs(
                minJobs: 1,
                maxJobs: 5,
                minOperations: 1,
                maxOperations: 5
            );

            Schedule SequentialSolve(List<Job> jobs)
            {
                Schedule schedule = new();
                int currentTime = 0;

                foreach (Job job in jobs)
                {
                    foreach (Operation operation in job.Operations)
                    {
                        operation.StartTime = currentTime;
                        currentTime = operation.EndTime;

                        schedule.JobSequence.Add(job.JobId);
                    }
                }

                schedule.EvaluatedJobs = jobs;
                return schedule;
            }

            int CalculateFitness(Schedule schedule) =>
                schedule.EvaluatedJobs
                    .SelectMany(job => job.Operations)
                    .Max(op => op.EndTime);

            // Act
            Schedule sequentialSchedule = SequentialSolve(jobs);
            GeneticAlgorithm solver = new(jobs);
            Schedule gaSchedule = solver.Solve();

            int sequentialFitness = CalculateFitness(sequentialSchedule);

            // Assert
            Assert.IsTrue(gaSchedule.Fitness <= sequentialFitness, $"Genetic Algorithm fitness ({gaSchedule.Fitness}) should not be worse than Sequential fitness ({sequentialFitness}).");
        }

        [TestMethod]
        public void Solve_HandlesSingleJob()
        {
            // Arrange
            List<Job> jobs = CreateMockJobs(
                minJobs: 1,
                maxJobs: 1,
                minOperations: 1,
                maxOperations: 1
            );
            GeneticAlgorithm solver = new(jobs);

            // Act
            Schedule schedule = solver.Solve();

            // Assert
            Assert.IsNotNull(schedule, "Schedule should not be null for a single job.");
            Assert.AreEqual(1, schedule.JobSequence.Count, "Schedule should contain one job sequence.");
        }
    }
}