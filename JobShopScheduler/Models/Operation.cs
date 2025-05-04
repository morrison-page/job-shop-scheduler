namespace JobShopScheduler.Models
{
    /// <summary>
    /// Represents an operation within a job, including its processing details and timing.
    /// </summary>
    public class Operation
    {
        /// <summary>
        /// Gets or sets the identifier of the job to which this operation belongs.
        /// </summary>
        public int JobId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier for this operation within the job.
        /// </summary>
        public int OperationId { get; set; }

        /// <summary>
        /// Gets or sets the subdivision or resource where this operation will be performed.
        /// </summary>
        public string Subdivision { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the time required to complete this operation, in hours.
        /// </summary>
        public int ProcessingTime { get; set; }

        /// <summary>
        /// Gets or sets the start time of the operation, computed during schedule evaluation.
        /// </summary>
        public int StartTime { get; set; }

        /// <summary>
        /// Gets the end time of the operation, calculated as the sum of <see cref="StartTime"/> and <see cref="ProcessingTime"/>.
        /// </summary>
        public int EndTime => StartTime + ProcessingTime;
    }
}
