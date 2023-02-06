using NodeCS.Services;
using NodeCS.Helpers;
using System.Net;

namespace node.server.Modules
{
    public class TestModule : RouterModule
    {
        public int Counter { get; set; } = 0;
        public TestModule(string path) : base(path)
        {

        }

        public override void Handle(HttpListenerRequest request, HttpListenerResponse response)
        {
            response.ContentType = "application/json";
            response.Send((new Models.DataModels() {TestData=$"penis {Counter++}", Param = request.GetQueryData().Serialize() }).Serialize());
            response.End();
        }
    }
}
