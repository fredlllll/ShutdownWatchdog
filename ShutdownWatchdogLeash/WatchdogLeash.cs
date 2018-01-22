using System;
using System.Net;

namespace ShutdownWatchdogLeash
{
    public class WatchdogLeash
    {
        int feedersPort, watchesPort;

        public WatchdogLeash(int feedersPort = 8181, int watchesPort = 8180)
        {
            this.feedersPort = feedersPort;
            this.watchesPort = watchesPort;
        }

        public void AddProcessWatch(int pid)
        {
            WebClient wc = new WebClient();
            string arg = "{\"pid\":" + pid + "}";
            arg = Uri.EscapeDataString(arg);
            wc.DownloadString("http://localhost:" + watchesPort + "/AddWatch?type=ShutdownWatchdog.Watches.ProcessWatch&arg=" + arg);
        }

        public void Feed(int units = 1)
        {
            WebClient wc = new WebClient();
            wc.DownloadString("http://localhost:" + feedersPort + "/Feed?units=" + units);
        }
    }
}
