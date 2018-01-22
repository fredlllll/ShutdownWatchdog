using System;
using System.Linq;
using System.Net;
using NLog;

namespace ShutdownWatchdog.Feeders
{
    class HttpFeeder : IFeeder
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        HttpListener listener;

        public HttpFeeder()
        {
            var config = ConfigFile.GetClassConfig();
            int port = config.Port;

            listener = new HttpListener();
            listener.Prefixes.Add("http://*:" + port + "/");
        }

        public event FeederFoodArrivedHandler FoodArrived;

        public void Attach()
        {
            logger.Info("listening for prefix: " + listener.Prefixes.First());
            listener.Start();
            listener.BeginGetContext(HandleContext, null);
        }

        public void Detach()
        {
            listener.Stop();
        }

        protected virtual void HandleContext(IAsyncResult result)
        {
            try
            {
                HttpListenerContext context = listener.EndGetContext(result);
                logger.Info("getting feeder request:" + context.Request.Url);
                if(context.Request.Url.AbsolutePath.EndsWith("Feed"))
                {
                    double units = 1;
                    if(context.Request.QueryString.AllKeys.Contains("units"))
                    {
                        string val = context.Request.QueryString["units"];
                        if(!double.TryParse(val, out units))
                        {
                            units = 1;
                        }
                    }
                    FoodArrived?.Invoke(this, new FoodArrivedArgs { Units = units });
                }
                context.Response.Close();
                listener.BeginGetContext(HandleContext, null);
            }
            catch(ObjectDisposedException)
            {
                //will be thrown when stopping listener
            }
        }
    }
}
