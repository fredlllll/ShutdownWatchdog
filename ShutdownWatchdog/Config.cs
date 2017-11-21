using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json.Linq;

namespace ShutdownWatchdog
{
    public static class Config
    {
        static string[] folders;

        static Config()
        {
            folders = new string[] {
                Directories.AssemblyDir,
                Directories.ProgramDir,
            };
        }

        static IEnumerator<string> GetFileCandidates(string name)
        {
            for(int i = 0; i < folders.Length; i++)
            {
                yield return Path.Combine(folders[i], name + "_config.json");
            }
        }

        public static T GetConfig<T>(string name) where T : JToken
        {
            var enumerator = GetFileCandidates(name);
            while(enumerator.MoveNext())
            {
                if(File.Exists(enumerator.Current))
                {
                    return (T)JToken.Parse(File.ReadAllText(enumerator.Current));
                }
            }
            return null;
        }
    }
}
