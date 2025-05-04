using JobShopScheduler.Models;

namespace JobShopScheduler.Interfaces
{
    /// <summary>
    /// Defines a contract for solving the scheduling problem and generating a schedule.
    /// </summary>
    internal interface IScheduleSolver
    {
        /// <summary>
        /// Solves the scheduling problem and returns the resulting <see cref="Schedule"/> with evaluated jobs.
        /// </summary>
        /// <returns>A <see cref="Schedule"/> object representing the solution to the scheduling problem.</returns>
        Schedule Solve();
    }
}
