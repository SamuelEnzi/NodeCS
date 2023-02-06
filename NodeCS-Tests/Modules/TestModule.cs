using NodeCS.Attributes;
using NodeCS.Helpers;
using System.Net;

namespace NodeCS_Tests.Modules
{
    public class TestModule
    {
        public int counter { get; set; } = 0;

        [Endpoint("/test")]
        public void TestMethod(HttpListenerRequest request, HttpListenerResponse response)
        {
            response.ContentType = "application/json";
            response.Send(new Data() { count = counter++ }.Serialize());
            response.End();
        }

        [Endpoint("/test2")]
        public void Test2Method(HttpListenerRequest request, HttpListenerResponse response)
        {
            response.Send("<html><body>HELLO TEST 2</body></html>");
            response.End();
        }
    }

    public class Data
    {
        public string base64 { get; set; } = "daiocen";
        public int count = 0;
    }
}
