using NodeCS.Helpers;
using NodeCS.Services;
using System;
using System.Net;
using System.Threading;

namespace NodeCS
{
    public class Router
    {
        public int Port { get; private set; }
        public HttpListener Server { get; private set; }
        public bool IsRunning { get; private set; } = false;
        private ModuleSelector selector;

        public Router(int port, ModuleSelector selector)
        {
            if (!HttpListener.IsSupported)
                throw new Exception("HttpListener not supported");

            this.selector = selector;

            Server = new HttpListener();
            Server.Prefixes.Add($"http://*:{port}/");
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
                    var selected = selector.Select(request.RawUrl.GetPath());

                    if (selected == null)
                    {
                        response.StatusCode = 404;
                        response.Send("<html><body>404</body></html>");
                        response.End();
                        continue;
                    }

                    selected?.Handle(request, response);
                    response.End();
                }
            }).Start();
        }
    }
}
