using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace ShutdownWatchdog
{
    public class WatchdogServer
    {
        List<IFeeder> feeders = new List<IFeeder>();

        public Watchdog Watchdog { get; protected set; }

        public WatchdogServer(Watchdog dog)
        {
            Watchdog = dog;
        }

        public void Start()
        {
            var config = Config.GetConfig<JArray>("watchdog_server");
            foreach(JObject jo in config)
            {
                bool load = jo.Get<bool>("load", false);
                if(load)
                {
                    string type = jo.Get<string>("type");
                    Type t = ReflectionHelper.GetType(type);
                    if(t != null)
                    {
                        var ci = t.GetConstructor(Type.EmptyTypes);
                        if(ci != null)
                        {
                            IFeeder feeder = (IFeeder)ci.Invoke(new object[] { });
                            AddFeeder(feeder);
                        }
                    }
                }
            }
        }

        public void Stop()
        {
            foreach(var feeder in feeders)
            {
                feeder.Detach();
            }
            feeders.Clear();
        }

        public void AddFeeder(IFeeder feeder)
        {
            feeder.FoodArrived += Feeder_FoodArrived;
            feeder.Attach();
        }

        private void Feeder_FoodArrived(IFeeder sender, FoodArrivedArgs args)
        {
            Watchdog.Feed(args.Units);
        }

        public void RemoveFeeder(IFeeder feeder)
        {
            feeder.Detach();
            feeder.FoodArrived -= Feeder_FoodArrived;
        }
    }
}
