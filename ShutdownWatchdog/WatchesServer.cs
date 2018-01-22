using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading;
using Newtonsoft.Json.Linq;
using NLog;
using ShutdownWatchdog.Util;

namespace ShutdownWatchdog
{
    public class WatchesServer : RestartableThreadClass
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public delegate void WatchFoodArrivedHandler(FoodArrivedArgs args);

        public event WatchFoodArrivedHandler FoodArrived;

        HttpListener listener = new HttpListener();

        List<IWatch> watches = new List<IWatch>();

        public WatchesServer() : base(false)
        {
            var config = ConfigFile.GetClassConfig();
            int port = config.Port;

            listener.Prefixes.Add("http://*:" + port + "/");
        }

        public override void Start()
        {
            listener.Start();
            listener.BeginGetContext(HandleRequest, null);

            base.Start();
        }

        public override void Stop()
        {
            base.Stop();
            listener.Stop();
            watches.Clear();
        }

        private void HandleRequest(IAsyncResult res)
        {
            try
            {
                var context = listener.EndGetContext(res);
                logger.Info("getting request: " + context.Request.Url.AbsolutePath);
                if(context.Request.Url.AbsolutePath.EndsWith("AddWatch") &&
                    context.Request.QueryString.AllKeys.Contains("type") &&
                    context.Request.QueryString.AllKeys.Contains("arg"))
                {
                    string type = context.Request.QueryString["type"];
                    logger.Info("getting request for new watch of type " + type);
                    Type t = ReflectionHelper.GetType(type);
                    if(t != null)
                    {
                        JObject arg = JObject.Parse(context.Request.QueryString["arg"]);
                        var ci = t.GetConstructor(new Type[] { typeof(JObject) });
                        if(ci != null)
                        {
                            try
                            {
                                object instance = ci.Invoke(new object[] { arg });
                                try
                                {
                                    IWatch w = (IWatch)instance;
                                    lock(watches)
                                    {
                                        watches.Add(w);
                                    }
                                }
                                catch(InvalidCastException)
                                {
                                    logger.Warn(type + " has to be an IWatch, but isnt");
                                    //not an IWatch
                                }
                            }
                            catch(TargetInvocationException ex)
                            {
                                //constructor threw exception
                                logger.Warn("instantiating the watch " + type + " threw an exception");
                                logger.Warn(ex);
                            }
                        }
                        else
                        {
                            logger.Warn("cant find a constructor that takes a " + nameof(JObject) + "for type " + type);
                        }
                    }
                    else
                    {
                        logger.Warn("cant find a type " + type);
                    }
                }
                context.Response.Close();
                listener.BeginGetContext(HandleRequest, null);
            }
            catch(ObjectDisposedException)
            {
                //will be thrown when stopping listener
            }
            catch(ThreadInterruptedException)
            {
                //just end
            }
        }

        protected override void Run()
        {
            try
            {
                while(true)
                {
                    List<IWatch> toDel = new List<IWatch>();
                    lock(watches)
                    {
                        foreach(var w in watches)
                        {
                            var status = w.Watch();
                            if(status != WatcherStatus.OK)
                            {
                                toDel.Add(w);
                            }
                        }
                        foreach(var w in toDel)
                        {
                            watches.Remove(w);
                        }
                        if(watches.Count > 0)
                        {
                            FoodArrived?.Invoke(new FoodArrivedArgs());
                        }
                    }
                    Thread.Sleep(10000);
                }
            }
            catch(ThreadInterruptedException)
            {
                //end
            }
        }
    }
}