using ShutdownWatchdog.Plugins;

namespace ShutdownWatchdog
{
    class Program
    {
        static Watchdog watchdog = new Watchdog();
        static FeedersServer feedersServer;
        static WatchesServer watchesServer;

        #if __MonoCS__
        static SignalReceiver signalReceiver;
        #endif

        static void Main(string[] args)
        {
            Logging.Logging.SetupLogging();

            PluginsLoader.LoadPlugins();

            
            feedersServer = new FeedersServer();
            feedersServer.FoodArrived += FeedersServer_FoodArrived;
            
            watchesServer = new WatchesServer();
            watchesServer.FoodArrived += WatchesServer_FoodArrived;

            watchdog.Start();
            feedersServer.Start();
            watchesServer.Start();

            #if __MonoCS__
            signalReceiver = new SignalReceiver(new Mono.Unix.Native.Signum[] {
                Mono.Unix.Native.Signum.SIGINT,
                Mono.Unix.Native.Signum.SIGTERM,
            });

            signalReceiver.SignalReceived += SignalReceiver_SignalReceived;
#endif
        }

        private static void FeedersServer_FoodArrived(IFeeder sender, FoodArrivedArgs args)
        {
            watchdog.Feed(args.Units);
        }

        private static void WatchesServer_FoodArrived(FoodArrivedArgs args)
        {
            watchdog.Feed(args.Units);
        }

        #if __MonoCS__
        private static void SignalReceiver_SignalReceived(Mono.Unix.Native.Signum obj)
        {
            watchdog.Stop();
            feedersServer.Stop();
            watchesServer.Stop();
        }
#endif
    }
}
