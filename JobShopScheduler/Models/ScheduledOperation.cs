namespace JobShopScheduler.Models
{
    public class ScheduledOperation
    {
        public int JobId { get; set; }
        public int OperationId { get; set; }
        public string Subdivision { get; set; }
        public int StartTime { get; set; }
        public int EndTime => StartTime + ProcessingTime;
        public int ProcessingTime { get; set; }
    }
}
