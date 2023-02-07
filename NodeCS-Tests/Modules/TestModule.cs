using NodeCS.Attributes;
using NodeCS.Helpers;
using System.Net;

namespace NodeCS_Tests.Modules
{
    public class TestModule
    {
        public int counter { get; set; } = 0;

        [Endpoint("/test/:id/porcodio")]
        public void TestMethod(HttpListenerRequest request, HttpListenerResponse response, Dictionary<string, string> parameters)
        {
            response.ContentType = "application/json";
            response.Send(new Data() { param = parameters["id"], count = counter++ }.Serialize());
            response.End();
        }

        [Endpoint("/test2", httpMethod: "POST")]
        public void Test2Method(HttpListenerRequest request, HttpListenerResponse response, Dictionary<string, string> parameters)
        {
            response.Send("<html><body>HELLO TEST 2</body></html>");
            response.End();
        }
    }

    public class Data
    {
        public string param { get; set; } = "daiocen";
        public int count = 0;
    }
}
