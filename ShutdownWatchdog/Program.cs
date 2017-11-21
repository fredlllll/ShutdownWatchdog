namespace ShutdownWatchdog
{
    class Program
    {
        static Watchdog watchdog = new Watchdog();
        static WatchdogServer watchdogServer;

        static SignalReceiver signalReceiver;

        static void Main(string[] args)
        {
            watchdog.Start();
            watchdogServer = new WatchdogServer(watchdog);
            watchdogServer.Start();

            signalReceiver = new SignalReceiver(new Mono.Unix.Native.Signum[] {
                Mono.Unix.Native.Signum.SIGINT,
                Mono.Unix.Native.Signum.SIGTERM,
            });

            signalReceiver.SignalReceived += SignalReceiver_SignalReceived;
        }

        private static void SignalReceiver_SignalReceived(Mono.Unix.Native.Signum obj)
        {
            watchdog.Stop();
            watchdogServer.Stop();
        }
    }
}
