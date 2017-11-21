using System.Collections.Generic;
using System.Net;
using System.Threading;

namespace ShutdownWatchdog
{
    class HttpFeeder : RestartableThreadClass, IFeeder
    {
        HttpListener listener;

        public HttpFeeder() : base(false)
        {
            listener = new HttpListener();
            listener.Prefixes.Add("http://*:80/");
        }

        public event FoodArrivedHandler FoodArrived;

        public void Attach()
        {
            listener.Start();
            Start();
        }

        public void Detach()
        {
            listener.Stop();
            Stop();
        }

        protected override void Run()
        {
            try
            {
                while(true)
                {
                    HttpListenerContext context = listener.GetContext();
                    if(context.Request.Url.AbsolutePath == "Feed")
                    {
                        double units = 1;
                        try
                        {
                            string val = context.Request.QueryString["units"];
                            if(!double.TryParse(val, out units))
                            {
                                units = 1;
                            }
                        }
                        catch(KeyNotFoundException)
                        {

                        }
                        FoodArrived?.Invoke(this, new FoodArrivedArgs { Units = units });
                    }
                    context.Response.Close();
                }
            }
            catch(ThreadInterruptedException)
            {

            }
        }
    }
}
