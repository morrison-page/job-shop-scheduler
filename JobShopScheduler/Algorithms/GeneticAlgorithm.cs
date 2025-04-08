using JobShopScheduler.Models;
using System.Collections.Generic;

public class GeneticAlgorithm
{
    private const int POPULATION_SIZE = 100;

    private static Random _random = new Random();
    
    private readonly List<JobOperation> _dataSet;
    
    private List<List<ScheduledOperation>> _population = new();

    public GeneticAlgorithm(List<JobOperation> DataSet)
    {
        _dataSet = DataSet;
    }

    public List<ScheduledOperation> Solve()
    {
        InitialisePopulation();
        List<ScheduledOperation> bestSchedule = _population.OrderBy(s => s.Max(op => op.EndTime)).FirstOrDefault();
        return bestSchedule;
    }

    private void InitialisePopulation()
    {
        for (int i = 0; i < POPULATION_SIZE; i++)
        {
            List<ScheduledOperation> schedule = new();
            List<JobOperation> localDataSet = new(_dataSet);

            while (true)
            {
                // Pick a random operation from the dataset.
                JobOperation selectedOperation = localDataSet[_random.Next(localDataSet.Count)];

                // Check the operations job order constraints.
                bool isLowestOperation = !localDataSet.Any(op =>
                    op.JobId == selectedOperation.JobId &&
                    op.OperationId < selectedOperation.OperationId);

                // If valid insert into schedule & delete from local dataset.
                if (isLowestOperation) 
                {
                    // Calculate the start time considering constraints.
                    IEnumerable<ScheduledOperation> sameSubdivisionOps = schedule
                        .Where(op => op.Subdivision == selectedOperation.Subdivision)
                        .OrderBy(op => op.EndTime);

                    IEnumerable<ScheduledOperation> sameJobOps = schedule
                        .Where(op => op.JobId == selectedOperation.JobId)
                        .OrderBy(op => op.OperationId);

                    // Takes the largest value of the last operation
                    // in the same subdivision and last operation in
                    // the same job and if either don't exist, start at 0.
                    int startTime = sameSubdivisionOps.Any() && sameJobOps.Any()
                       ? Math.Max(sameSubdivisionOps.Last().EndTime, sameJobOps.Last().EndTime)
                       : sameSubdivisionOps.Any()
                           ? sameSubdivisionOps.Last().EndTime
                           : sameJobOps.Any()
                               ? sameJobOps.Last().EndTime
                               : 0;
                    

                    schedule.Add(new ScheduledOperation
                    {
                        JobId = selectedOperation.JobId,
                        OperationId = selectedOperation.OperationId,
                        Subdivision = selectedOperation.Subdivision,
                        StartTime = startTime,
                        ProcessingTime = selectedOperation.ProcessingTime
                    });

                    localDataSet.Remove(selectedOperation);
                }

                if (schedule.Count == _dataSet.Count) break;
            }

            _population.Add(schedule);
        }
    }
}
