using System.Net;

namespace NodeCS.Interfaces
{
    public interface IRouterModule
    {
        void Handle(HttpListenerRequest request, HttpListenerResponse response);
    }
}
