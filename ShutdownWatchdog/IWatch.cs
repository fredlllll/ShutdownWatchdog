using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShutdownWatchdog
{
    public enum WatcherStatus
    {
        OK,
        NotOK
    }

    public interface IWatch
    {
        WatcherStatus Watch();
    }
}
