using NodeCS.Attributes;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text.RegularExpressions;

namespace NodeCS.Services
{
    public class ModuleCompiler
    {
        public List<Endpoint> Endpoints { get; set; } = new List<Endpoint>();
        private List<object> modules;

        public ModuleCompiler(List<object> modules) =>
            this.modules = modules;

        public void Compile()
        {
            foreach (var obj in modules)
            {
                var methods = obj.GetType().GetMethods();
                foreach (var method in methods)
                    foreach (var attribute in method.GetCustomAttributes())
                        if (attribute.GetType() == typeof(EndpointAttribute))
                            if (IsValidMethod(method.GetParameters()))
                                Endpoints.Add( new Endpoint(
                                        ((EndpointAttribute)attribute).Path, 
                                        obj, 
                                        method, 
                                        ((EndpointAttribute)attribute).HttpMethod)
                                    );
            }
        }

        public bool Handle(string path, HttpListenerRequest request, HttpListenerResponse response)
        {
            foreach(var endpoint in Endpoints)
                if(Regex.Match(path, endpoint.Regex, RegexOptions.IgnoreCase).Success && request.HttpMethod.ToUpper() == endpoint.HttpMethod.ToUpper())
                {
                    endpoint.Handle(path, request, response);
                    return true;
                }
            return false;
        }

        private bool IsValidMethod(ParameterInfo[] parameterInfo)
        {
            if (parameterInfo.Length != 3) return false;
            
            if ( parameterInfo[0].ParameterType == typeof(HttpListenerRequest) && 
                 parameterInfo[1].ParameterType == typeof(HttpListenerResponse) &&
                 parameterInfo[2].ParameterType == typeof(Dictionary<string, string>)) return true;

            return false;
        }
    }
    
    
    public class Endpoint
    {
        private const string ParameterReplaceRegex = "[1-9a-zA-Z]*";
        private List<(int index, string name)> parameters { get; set; } = new List<(int index, string name)>();
        private readonly MethodInfo handle;
        private readonly object parent;

        public readonly string HttpMethod;
        public string Path { get; private set; }
        public string Regex { get; private set; }
        
        public Endpoint(string path, object parent, MethodInfo handle, string httpMethod)
        {
            this.Path = path;
            this.handle = handle;
            this.parent = parent;
            this.HttpMethod = httpMethod;

            Prepare();
        }

        private void Prepare()
        {
            var sanitized = Path.Trim().Trim('/');
            var segments = sanitized.Split('/');

            List<string> regex = new List<string>();

            for(int i = 0; i < segments.Length; i++)
            {
                if (segments[i].StartsWith(":"))
                {
                    parameters.Add((i, segments[i].Substring(1)));
                    regex.Add(ParameterReplaceRegex);
                }else
                    regex.Add(segments[i]);
            }

            this.Regex = regex.Aggregate((x, y) => x + "/" + y);
        }
    
        public void Handle(string path, HttpListenerRequest request, HttpListenerResponse response)
        {
            Dictionary<string, string> parsed = new Dictionary<string, string>();
            var sanitized = path.Trim().Trim('/');
            var segments = sanitized.Split('/');
            
            foreach (var param in parameters)
                parsed.Add(param.name, segments[param.index]);

            handle.Invoke(parent, new object[] { request, response, parsed});
        }
    }
}
