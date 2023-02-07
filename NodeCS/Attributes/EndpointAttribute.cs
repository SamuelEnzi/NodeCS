using System;
using System.Net.Http;

namespace NodeCS.Attributes
{
    [System.AttributeUsage(AttributeTargets.Method)]
    public class EndpointAttribute : System.Attribute
    {
        public string HttpMethod { get; set; } = "GET";
        public string Path { get; set; }

        public EndpointAttribute(string path)
        {
            this.Path = path;
        }

        public EndpointAttribute(string path, string httpMethod)
        {
            this.Path = path;
            this.HttpMethod = httpMethod;
        }
    }
}
