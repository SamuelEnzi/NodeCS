using System;

namespace NodeCS.Attributes
{
    [System.AttributeUsage(AttributeTargets.Method)]
    public class EndpointAttribute : System.Attribute
    {
        public string Path { get; set; }

        public EndpointAttribute(string path)
        {
            this.Path = path;
        }
    }
}
