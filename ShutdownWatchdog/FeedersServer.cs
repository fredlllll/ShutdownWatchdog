using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using NLog;
using ShutdownWatchdog.Util;

namespace ShutdownWatchdog
{
    public class FeedersServer
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        List<IFeeder> feeders = new List<IFeeder>();

        public event FeederFoodArrivedHandler FoodArrived;

        public void Start()
        {
            var configFile = ConfigFile.GetClassConfig();
            JArray config = configFile.Feeders;// Config.Config.GetConfig<JArray>("watchdog_server");
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
                            try
                            {
                                IFeeder feeder = (IFeeder)ci.Invoke(new object[] { });
                                logger.Info("Created feeder " + jo.Get<string>("name"));
                                AddFeeder(feeder);
                            }
                            catch(Exception e)
                            {
                                logger.Error("Error when trying to instatiate feeder: " + t.FullName);
                                logger.Error(e);
                            }
                        }
                        else
                        {
                            logger.Warn("Couldn't find parameterless constructor for feeder: " + t.FullName);
                        }
                    }
                    else
                    {
                        logger.Warn("could not find type: " + type);
                    }
                }
            }
        }

        public void Stop()
        {
            foreach(var feeder in feeders)
            {
                RemoveFeeder(feeder);
            }
            feeders.Clear();
        }

        private void AddFeeder(IFeeder feeder)
        {
            feeder.FoodArrived += Feeder_FoodArrived;
            feeder.Attach();
        }

        private void Feeder_FoodArrived(IFeeder sender, FoodArrivedArgs args)
        {
            FoodArrived?.Invoke(sender, args);
        }

        private void RemoveFeeder(IFeeder feeder)
        {
            feeder.Detach();
            feeder.FoodArrived -= Feeder_FoodArrived;
        }
    }
}
