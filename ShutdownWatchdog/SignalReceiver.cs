#if __MonoCS__
using System;
using Mono.Unix;
using Mono.Unix.Native;
using ShutdownWatchdog.Util;

namespace ShutdownWatchdog
{
    public class SignalReceiver : RestartableThreadClass
    {
        //TODO: alternative for windows?

        UnixSignal[] Signals { get; set; }

        public SignalReceiver(Signum[] signums) : base(true)
        {
            Signals = new UnixSignal[signums.Length];
            for(int i = 0; i < signums.Length; i++)
            {
                Signals[i] = new UnixSignal(signums[i]);
            }
        }

        public event Action<Signum> SignalReceived;

        void RaiseSignalReceived(UnixSignal signal)
        {
            SignalReceived?.Invoke(signal.Signum);
        }

        protected override void Run()
        {
            while(true)
            {
                int index = UnixSignal.WaitAny(Signals, -1);
                var signal = Signals[index];
                RaiseSignalReceived(signal);
            }
        }
    }
}
#endif