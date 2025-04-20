using JobShopScheduler.Models;

namespace JobShopScheduler.Interfaces
{
    internal interface IScheduleSolver
    {
        Schedule Solve();
    }
}
