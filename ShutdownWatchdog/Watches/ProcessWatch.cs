using System.Diagnostics;
using Newtonsoft.Json.Linq;
using NLog;

namespace ShutdownWatchdog.Watches
{
    public class ProcessWatch : IWatch
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        Process process;

        public ProcessWatch(JObject arg)
        {
            process = Process.GetProcessById(arg.Get<int>("pid")); //will throw argument exception if process doesnt exist
        }

        public WatcherStatus Watch()
        {
            process.Refresh();
            if(process.HasExited)
            {
                logger.Info("process " + process.Id + " has ended");
                return WatcherStatus.NotOK;
            }
            return WatcherStatus.OK;
        }
    }
}
