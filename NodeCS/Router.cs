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
        public event PathNotFoundEventHandler PathNotFound;

        public int Port { get; private set; }
        public HttpListener Server { get; private set; }
        public bool IsRunning { get; private set; } = false;
        private Func<HttpListenerRequest, HttpListenerResponse, bool> Callback;
        private ModuleCompiler moduleCompiler;

        public Router(int port, List<object> modules)
        {
            if (!HttpListener.IsSupported)
                throw new Exception("HttpListener not supported");

            this.Port = port;
            moduleCompiler = new ModuleCompiler(modules);

            Compile();
        }

        private void Compile()
        {
            moduleCompiler.Compile();
        }

        public void Listen(Func<HttpListenerRequest, HttpListenerResponse, bool> callback = null)
        {
            this.Callback = callback;
            Server = new HttpListener();
            Server.Prefixes.Add($"http://*:{Port}/");
            Start();
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

                    var cont = Callback?.Invoke(request, response);
                    if(cont == false)
                    {
                        response.End();
                        continue;
                    }

                    var selected = moduleCompiler.Handle(request.GetPath(), request, response);

                    if (!selected)
                    {
                        PathNotFound?.Invoke(request,response);
                        response.End();
                        continue;
                    }

                    response.End();
                }
            }).Start();
        }
    }
}
