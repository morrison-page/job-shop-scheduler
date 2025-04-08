using System.Collections.Generic;

namespace JobShopScheduler.Models
{
    public class Job
    {
        public int JobId { get; set; }
        public List<Operation> Operations { get; set; } = new();
        public int NextOpIndex { get; set; } = 0;

        public bool HasNextOperation => NextOpIndex < Operations.Count;

        public Operation GetNextOperation() => Operations[NextOpIndex++];

        public void Reset() => NextOpIndex = 0;
    }

}
