using System.Collections.Generic;

namespace JobShopScheduler.Models
{
    /// <summary>
    /// Represents a job consisting of a sequence of operations to be performed.
    /// </summary>
    public class Job
    {
        /// <summary>
        /// Gets or sets the unique identifier for the job.
        /// </summary>
        public int JobId { get; set; }

        /// <summary>
        /// Gets or sets the list of operations associated with the job.
        /// </summary>
        public List<Operation> Operations { get; set; } = new();

        /// <summary>
        /// Gets or sets the index of the next operation to be executed.
        /// </summary>
        public int NextOpIndex { get; set; } = 0;

        /// <summary>
        /// Gets a boolean indicating whether the job has more operations to execute.
        /// </summary>
        public bool HasNextOperation => NextOpIndex < Operations.Count;

        /// <summary>
        /// Retrieves the next operation to be executed and increments the operation index.
        /// </summary>
        /// <returns>The next <see cref="Operation"/> in the sequence.</returns>
        public Operation GetNextOperation() => Operations[NextOpIndex++];

        /// <summary>
        /// Resets the operation index to the beginning of the job's operation list.
        /// </summary>
        public void Reset() => NextOpIndex = 0;
    }
}
