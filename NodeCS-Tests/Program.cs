using NodeCS;
using NodeCS_Tests.Modules;
using NodeCS.Helpers;

var router = 
    new Router(3000,
    new List<object>
    {
        new TestModule()
    });

router.PathNotFound += (req, res) =>
{
    res.StatusCode = 404;
    res.Send("<html><body>DER GRO?E BAUER <img src=\"https://scontent-mxp2-1.xx.fbcdn.net/v/t39.30808-6/308627891_194131116312733_1266746255693589805_n.jpg?_nc_cat=105&ccb=1-7&_nc_sid=09cbfe&_nc_ohc=hktr7vNPSDsAX_i9PdW&_nc_ht=scontent-mxp2-1.xx&oh=00_AfDwmFr76gUSx2Ji7oMGKXfNTDGtybyNoWg63TJGWll-pw&oe=63E74084\"/></body></html>");
};

router.Listen((req, res) =>
{
    Console.WriteLine($"[{req.RemoteEndPoint.Address}]  ::  {req.HttpMethod.ToUpper()} -> {req.LocalEndPoint.Address}{req.GetPath()}");
    return true;
});

Console.WriteLine($"listening on localhost:3000");
Console.ReadLine();