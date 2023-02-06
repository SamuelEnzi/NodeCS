using NodeCS.Interfaces;
using System.Net;

namespace NodeCS.Services
{
    public class RouterModule : IRouterModule
    {
        public string Path { get; private set; }

        public RouterModule(string path) =>
            this.Path = path;

        public virtual void Handle(HttpListenerRequest request, HttpListenerResponse response)
        {

        }
    }
}
