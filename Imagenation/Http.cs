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
                        var path = request.QueryString.Get("path");
                        var life = double.Parse(request.QueryString.Get("life"));
                        var x = int.Parse(request.QueryString.Get("x"));
                        var y = int.Parse(request.QueryString.Get("y"));
                        var noreset = request.QueryString.Get("noreset");
                        var contents = "";
                        using (var a = new StreamReader(context.Request.InputStream))
                        {
                            contents = a.ReadToEnd();
                        }
                        try
                        {
                            var data = new DrawRequest(path, (long)(life * 1000d), noreset != null, x, y);
                            new TickScheduler(delegate
                            {
                                P.Requests.Add(data);
                            });
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
