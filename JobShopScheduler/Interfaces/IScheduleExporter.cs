using JobShopScheduler.Models;

namespace JobShopScheduler.Interfaces
{
    /// <summary>
    /// Defines a contract for exporting a schedule.
    /// </summary>
    internal interface IScheduleExporter
    {
        /// <summary>
        /// Exports the provided schedule to a specific output format or destination.
        /// </summary>
        /// <param name="schedule">The schedule to be exported, containing evaluated jobs.</param>
        void Export(Schedule schedule);
    }
}
