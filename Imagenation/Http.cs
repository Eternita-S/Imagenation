using ECommons.Logging;
using ECommons.Schedulers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Imagenation
{
    internal class Http
    {
        HttpListener listener;
        internal Http()
        {
            listener = new HttpListener()
            {
                Prefixes = { "http://127.0.0.1:43434/" }
            };
            listener.Start();
            new Thread((ThreadStart)delegate
            {
                while (listener != null && listener.IsListening)
                {
                    try
                    {
                        List<string> status = new List<string>();
                        HttpListenerContext context = listener.GetContext();
                        HttpListenerRequest request = context.Request;
                        try
                        {
                            var path = request.QueryString.Get("path");
                            if (request.QueryString.Get("destroy") != null)
                            {
                                new TickScheduler(delegate
                                {
                                    P.Requests.RemoveAll(x => x.Path == path);
                                    status.Add($"Removed {path}");
                                });
                            }
                            else if (request.QueryString.Get("destroyAll") != null)
                            {
                                new TickScheduler(delegate
                                {
                                    P.Requests.Clear();
                                    status.Add($"Removed all");
                                });
                            }
                            else
                            {
                                if(path == null)
                                {
                                    throw new InvalidDataException("Path can not be null");
                                }
                                if (!double.TryParse(request.QueryString.Get("life"), out var life))
                                {
                                    life = 99999;
                                    status.Add("Life is set to default 99999 seconds");
                                };

                                if (!int.TryParse(request.QueryString.Get("x"), out var x))
                                {
                                    x = 0;
                                    status.Add("x is set to default 0");
                                };
                                if (!int.TryParse(request.QueryString.Get("y"), out var y))
                                {
                                    x = 0;
                                    status.Add("y is set to default 0");
                                };
                                var noreset = request.QueryString.Get("noreset");
                                var contents = "";
                                using (var a = new StreamReader(context.Request.InputStream))
                                {
                                    contents = a.ReadToEnd();
                                }
                                var data = new DrawRequest(path, (long)(life * 1000d), noreset != null, x, y);
                                new TickScheduler(delegate
                                {
                                    P.Requests.Add(data);
                                    PluginLog.Information($"Displaying image {data.Path}");
                                    status.Add($"Displaying image {data.Path}");
                                });
                            }
                        }
                        catch (Exception e)
                        {
                            status.Add("Error:");
                            status.Add(e.Message);
                            status.Add(e.StackTrace);
                        }
                        HttpListenerResponse response = context.Response;
                        response.AppendHeader("Access-Control-Allow-Origin", "*");
                        string responseString = string.Join("\n", status);
                        byte[] buffer = Encoding.UTF8.GetBytes(responseString);
                        response.ContentLength64 = buffer.Length;
                        Stream output = response.OutputStream;
                        output.Write(buffer, 0, buffer.Length);
                        output.Close();
                    }
                    catch (Exception e)
                    {
                        new TickScheduler(delegate { PluginLog.Error("Error: " + e + "\n" + e.StackTrace); });
                    }
                }
            }).Start();
        }

        public void Dispose()
        {
            listener.Abort();
            listener = null;
        }
    }
}
