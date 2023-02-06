using NodeCS.Attributes;
using NodeCS.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;
using System.Threading;

namespace NodeCS
{
    public class Router
    {
        public int Port { get; private set; }
        public HttpListener Server { get; private set; }
        public bool IsRunning { get; private set; } = false;
        public List<object> Modules = new List<object>();


        public Router(int port, List<object> modules)
        {
            if (!HttpListener.IsSupported)
                throw new Exception("HttpListener not supported");

            this.Modules = modules;

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
                    var selected = Handle(request.RawUrl.GetPath(), request, response);

                    if (!selected)
                    {
                        response.StatusCode = 404;
                        response.Send("<html><body>404</body></html>");
                        response.End();
                        continue;
                    }

                    response.End();
                }
            }).Start();
        }

        private bool Handle(string path, HttpListenerRequest request, HttpListenerResponse response)
        {
            foreach (var obj in Modules)
            {
                var methods = obj.GetType().GetMethods();
                foreach (var method in methods)
                    foreach (var attribute in method.GetCustomAttributes())
                        if (attribute.GetType() == typeof(EndpointAttribute))
                            if(((EndpointAttribute)attribute).Path == path)
                                if (IsValidMethod(method.GetParameters()))
                                {
                                    method.Invoke(obj, new object[] { request, response });
                                    return true;
                                }
            }
            return false;
        }


        private bool IsValidMethod(ParameterInfo[] parameterInfo)
        {
            if (parameterInfo.Length != 2) return false;
            if (parameterInfo[0].ParameterType == typeof(HttpListenerRequest) && parameterInfo[1].ParameterType == typeof(HttpListenerResponse)) return true;
            return false;
        }
    }
}
