using JobShopScheduler.Models;
using System.Collections.Generic;

namespace JobShopScheduler.Interfaces
{
    internal interface IScheduleExporter
    {
        void Export(Schedule schedule);
    }
}
