using System;
using Mono.Unix;
using Mono.Unix.Native;

namespace ShutdownWatchdog
{
    public class SignalReceiver : RestartableThreadClass
    {
        //TODO: alternative for windows?

        UnixSignal[] Signals { get; set; }

        public SignalReceiver(Signum[] signums) : base(true)
        {
#if __MonoCS__
            Signals = new UnixSignal[signums.Length];
            for(int i = 0; i < signums.Length; i++)
            {
                Signals[i] = new UnixSignal(signums[i]);
            }
#endif
        }

        public event Action<Signum> SignalReceived;

        void RaiseSignalReceived(UnixSignal signal)
        {
            SignalReceived?.Invoke(signal.Signum);
        }

        protected override void Run()
        {
#if __MonoCS__
            while(true)
            {
                int index = UnixSignal.WaitAny(Signals, -1);
                var signal = Signals[index];
                RaiseSignalReceived(signal);
            }
#endif
        }
    }
}
