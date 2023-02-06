using NodeCS;
using node.server.Modules;

new Router(
    3000,
    new NodeCS.Services.ModuleSelector(
        new List<NodeCS.Services.RouterModule>()
        {
            new TestModule("/test")
        })
    );

Console.ReadLine();