using System.Collections.Generic;

namespace JobShopScheduler.Models
{
    public class Schedule
    {
        public List<int> JobSequence { get; set; } = new();
        public int Fitness { get; set; } = int.MaxValue;

        public Schedule Clone() => new()
        {
            JobSequence = new List<int>(JobSequence),
            Fitness = Fitness
        };
    }
}
