﻿using System.Collections.Generic;
using System.Diagnostics;

namespace ShutdownWatchdog.Util
{
    public static class ProcessUtil
    {
        public static Process BeginRunExecutableWithRedirect(string filename, string arguments, Dictionary<string, string> environmentVariables = null, string workingDirectory = null, bool start = true)
        {
            Process p = CreateProcess(filename, arguments, redirectStdOut: true, redirectStdErr: true, enableRisingEvents: true, environmentVariables: environmentVariables, workingDirectory: workingDirectory);

            if(start)
            {
                p.Start();
            }

            return p;
        }

        private static Process CreateProcess(string filename, string arguments, bool useShellExecute = false, bool createNoWindow = true, bool redirectStdOut = false, bool redirectStdErr = false, bool enableRisingEvents = false, Dictionary<string, string> environmentVariables = null, string workingDirectory = null)
        {
            Process p = new Process()
            {
                StartInfo = new ProcessStartInfo()
                {
                    FileName = filename,
                    Arguments = arguments,
                    UseShellExecute = useShellExecute,
                    CreateNoWindow = createNoWindow,
                    RedirectStandardOutput = redirectStdOut,
                    RedirectStandardError = redirectStdErr
                },
                EnableRaisingEvents = enableRisingEvents,
            };

            if(environmentVariables != null)
            {
                foreach(var kv in environmentVariables)
                {
                    p.StartInfo.EnvironmentVariables.Add(kv.Key, kv.Value);
                }
            }

            if(workingDirectory != null)
            {
                p.StartInfo.WorkingDirectory = workingDirectory;
            }

            return p;
        }
    }
}
