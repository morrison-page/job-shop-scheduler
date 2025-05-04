using System.Collections.Generic;

namespace JobShopScheduler.Models
{
    /// <summary>
    /// Represents a schedule for a set of jobs, including their sequence and evaluation results.
    /// </summary>
    public class Schedule
    {
        /// <summary>
        /// Gets or sets the sequence of job IDs in the schedule.
        /// </summary>
        /// <remarks>
        /// The solution is encoded as a list of integers where each integer represents a job ID.
        /// Each occurrence of a job ID corresponds to an operation of that job in sequence.
        /// For example, the sequence [1, 2, 3, 1] means:
        /// - The first occurrence of 1 is Job 1, Operation 1.
        /// - The second occurrence of 1 is Job 1, Operation 2.
        /// - The first occurrence of 2 is Job 2, Operation 1, and so on.
        /// </remarks>
        public List<int> JobSequence { get; set; } = new();

        /// <summary>
        /// Gets or sets the list of evaluated jobs, including their operations and timing details.
        /// </summary>
        public List<Job> EvaluatedJobs { get; set; } = new();

        /// <summary>
        /// Gets or sets the fitness value of the schedule, representing its quality or efficiency in hours to complete.
        /// </summary>
        /// <remarks>
        /// A lower fitness value indicates a better schedule.
        /// </remarks>
        public int Fitness { get; set; } = int.MaxValue;

        /// <summary>
        /// Creates a deep copy of the current schedule.
        /// </summary>
        /// <returns>A new <see cref="Schedule"/> object with the same job sequence and fitness value.</returns>
        public Schedule Clone() => new()
        {
            JobSequence = new List<int>(JobSequence),
            Fitness = Fitness
        };
    }
}
