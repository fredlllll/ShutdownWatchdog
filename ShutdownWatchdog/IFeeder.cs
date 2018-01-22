using System;

namespace ShutdownWatchdog
{
    public class FoodArrivedArgs : EventArgs
    {
        public double Units { get; set; } = 1;
    }

    public delegate void FeederFoodArrivedHandler(IFeeder sender, FoodArrivedArgs args);

    public interface IFeeder
    {
        event FeederFoodArrivedHandler FoodArrived;

        void Attach();
        void Detach();
    }
}
