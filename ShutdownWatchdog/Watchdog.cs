using System;
using System.Threading;

namespace ShutdownWatchdog
{
    public class Watchdog : RestartableThreadClass
    {
        public DateTime ShutdownTime { get; protected set; }

        TimeSpan foodUnit;

        public Watchdog() : base(false)
        {
            foodUnit = TimeSpan.FromSeconds(300);//todo: read form settings?
        }

        public override void Start()
        {
            ShutdownTime = DateTime.UtcNow + foodUnit;
            base.Start();
        }

        public void Feed(double units = 1)
        {
            ShutdownTime += TimeSpan.FromSeconds(foodUnit.TotalSeconds * units);
        }

        protected override void Run()
        {
            try
            {
                while(true)
                {
                    if(DateTime.UtcNow > ShutdownTime)
                    {
                        ShutdownMachine();
                    }
                    Thread.Sleep(5000);
                }
            }
            catch(ThreadInterruptedException)
            {
                //ending
            }
        }

        void ShutdownMachine()
        {

        }
    }
}
