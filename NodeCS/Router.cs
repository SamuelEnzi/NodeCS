using NodeCS.Helpers;
using NodeCS.Services;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;

namespace NodeCS
{
    public class Router
    {
        public delegate void PathNotFoundEventHandler(HttpListenerRequest request, HttpListenerResponse response);
        public delegate void HandleExceptionEventHandler(HttpListenerRequest request, HttpListenerResponse response, Exception exception);

        public event PathNotFoundEventHandler PathNotFound;
        public event HandleExceptionEventHandler HandleException;


        public int Port { get; private set; }
        public HttpListener Server { get; private set; }
        public bool IsRunning { get; private set; } = false;
        private Func<HttpListenerRequest, HttpListenerResponse, bool> Callback;
        private ModuleCompiler moduleCompiler;

        public Router(int port, List<object> modules)
        {
            if (!HttpListener.IsSupported)
                throw new Exception("HttpListener not supported");

            Log.WriteLine($"setting up router");

            this.Port = port;
            moduleCompiler = new ModuleCompiler(modules);

            Compile();
        }

        private void Compile()
        {
            Log.WriteLine($"compileing endpoints");
            moduleCompiler.Compile();
            Log.WriteLine($"\nendpoints compiled", ConsoleColor.Green);
            foreach (var endpoint in moduleCompiler.Endpoints)
                Log.WriteLine($"    [{endpoint.HttpMethod.ToUpper()}] {endpoint.Path}");
            Log.WriteLine();
        }

        public void Listen(Func<HttpListenerRequest, HttpListenerResponse, bool> callback = null)
        {
            Log.WriteLine($"starting server");
            this.Callback = callback;
            Server = new HttpListener();
            Server.Prefixes.Add($"http://*:{Port}/");
            Start();
            Log.WriteLine($"listening on {$"http://*:{Port}/"}", ConsoleColor.Green);
        }

        private void Start()
        {
            if (IsRunning) return;

            Server.Start();
            IsRunning = true;
            new Thread(() =>
            {
                while (true)
                {
                    var context = Server.GetContext();
                    var request = context.Request;
                    var response = context.Response;

                    try
                    {
                        var cont = Callback?.Invoke(request, response);
                        if (cont == false)
                        {
                            response.End();
                            continue;
                        }

                        var selected = moduleCompiler.Handle(request.GetPath(), request, response);

                        if (!selected)
                        {
                            PathNotFound?.Invoke(request, response);
                            response.End();
                            continue;
                        }
                    }
                    catch (Exception ex){ HandleException?.Invoke(request, response, ex); }
                    finally
                    {
                        response.End();
                    }
                }
            }).Start();
        }
    }
}
