using NodeCS;
using NodeCS_Tests.Modules;

new Router(3000,
    new List<object>
    {
        new TestModule()
    });

Console.WriteLine($"listening on localhost:3000");
Console.ReadLine();