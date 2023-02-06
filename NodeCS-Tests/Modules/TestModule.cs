using NodeCS.Attributes;
using NodeCS.Helpers;
using System.Net;

namespace NodeCS_Tests.Modules
{
    public class TestModule
    {

        [Endpoint("/test")]
        public void TestMethod(HttpListenerRequest request, HttpListenerResponse response)
        {
            response.Send("<html><body>HELLO TEST</body></html>");
            response.End();
        }

        [Endpoint("/test2")]
        public void Test2Method(HttpListenerRequest request, HttpListenerResponse response)
        {
            response.Send("<html><body>HELLO TEST 2</body></html>");
            response.End();
        }
    }
}
