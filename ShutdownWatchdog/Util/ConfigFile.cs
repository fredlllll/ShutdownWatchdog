﻿using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using ShutdownWatchdog.Util;

namespace ShutdownWatchdog
{
    public class ConfigFile : DynamicObject
    {
        public string FilePath { get; set; }

        Dictionary<string, object> cache = new Dictionary<string, object>();
        JObject obj;

        public ConfigFile(string file)
        {
            FilePath = file;
            Reload();
        }

        public void Reload()
        {
            cache.Clear();
            obj = JObject.Parse(File.ReadAllText(FilePath));
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            if(cache.TryGetValue(binder.Name, out result)) //try get from cache
            {
                if(binder.ReturnType.Equals(result.GetType())) //type matches
                {
                    return true;
                }
            }

            string jsonName = binder.Name.FirstCharacterToLower();
            if(obj.HasValue(jsonName))
            {
                result = obj.Get(binder.ReturnType, jsonName);
                cache[binder.Name] = result;
                return true;
            }
            return false;
        }

        public override IEnumerable<string> GetDynamicMemberNames()
        {
            return obj.Properties().Select(x => x.Name.FirstCharacterToUpper());
        }

        public static dynamic GetConfig(string name)
        {
            name += "_config.json";

            string file = Files.ResolveFileOrDefault(name);

            return new ConfigFile(file);
        }

        public static dynamic GetClassConfig()
        {
            Type t = ReflectionHelper.GetCallingType();
            string name = Char.ToLowerInvariant(t.Name[0]) + String.Join("", t.Name.Skip(1).Select(x => char.IsUpper(x) ? ("_" + x) : (char.ToLowerInvariant(x).ToString())));
            return GetConfig(name);
        }
    }
}
