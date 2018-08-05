using System;
using System.Threading.Tasks;

namespace Deveel.Workflows.Timers
{
    public interface IScheduleCallback
    {
        Task NotifyAsync();
    }
}
