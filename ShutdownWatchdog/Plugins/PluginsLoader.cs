using System;
using System.IO;
using System.Reflection;
using Newtonsoft.Json.Linq;
using NLog;

namespace ShutdownWatchdog.Plugins
{
    public static class PluginsLoader
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public static void LoadPlugins()
        {
            var configFile = ConfigFile.GetClassConfig();
            JArray config = configFile.Plugins;// Config.Config.GetConfig<JArray>("plugins_loader");
            if(config != null)
            {
                foreach(JObject jo in config)
                {
                    bool load = jo.Get<bool>("load", false);
                    if(load)
                    {
                        string name = jo.Get<string>("name", "unnamed");
                        string file = jo.Get<string>("file");
                        if(!File.Exists(file))
                        {
                            logger.Warn("Tried to load plugin, but file didnt exist: " + file);
                            continue;
                        }

                        var assemblyName = AssemblyName.GetAssemblyName(file);
                        AppDomain.CurrentDomain.Load(assemblyName);
                    }
                }
            }
        }
    }
}
