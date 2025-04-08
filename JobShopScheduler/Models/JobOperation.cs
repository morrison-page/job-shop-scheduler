namespace JobShopScheduler.Models
{
    public class JobOperation
    {
        public int JobId { get; set; }
        public int OperationId { get; set; }
        public string Subdivision { get; set; }
        public int ProcessingTime { get; set; }
    }
}