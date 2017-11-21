using System.IO;
using System.Reflection;

namespace ShutdownWatchdog
{
    public static class Directories
    {
        public static string AssemblyDir
        {
            get
            {
                return Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            }
        }

        public static string ProgramDir
        {
            get
            {
                return Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            }
        }
    }
}
