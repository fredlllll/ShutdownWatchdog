using System;

namespace ShutdownWatchdog
{
    public class FoodArrivedArgs : EventArgs
    {
        public double Units { get; set; }
    }

    public delegate void FoodArrivedHandler(IFeeder sender, FoodArrivedArgs args);

    public interface IFeeder
    {
        event FoodArrivedHandler FoodArrived;

        void Attach();
        void Detach();
    }
}
