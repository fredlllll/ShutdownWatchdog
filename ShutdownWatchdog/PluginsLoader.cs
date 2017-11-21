using System;
using System.IO;
using System.Reflection;
using Newtonsoft.Json.Linq;

namespace ShutdownWatchdog
{
    public static class PluginsLoader
    {
        public static void LoadPlugins()
        {
            var config = Config.GetConfig<JArray>("plugins_loader");
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
