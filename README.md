# NodeCS

This Library can be used to create HTTP Servers.
It was inspired by [NodeJS](https://nodejs.org/en/docs/) and can be used to create RESTFUL apis or other server services.

To create a server, you must first define a router. This handles all connections and resolves the path to the correct endpoint.

```csharp
var router = 
    new Router(3000,
    new List<object>
    {
        new TestModule()
    });
```

In the constructor of the class the port and the modules containing endpoints are specified.
To start the Server call the `Listen` method:

```csharp
router.Listen((req, res) =>
{
    Console.WriteLine($"[{req.RemoteEndPoint.Address}]  ::  {req.HttpMethod.ToUpper()} -> {req.LocalEndPoint.Address}{req.GetPath()}");
    return true;
});
```

The structure of a module is simple. Here is an example:

```csharp
public class TestModule
{
    public int counter { get; set; } = 0;

    //endpoint only defining a path setting it to GET by default
    //path defines a parameter called 'id'. it will be parsed and passed in the variable parameters when calling the endpoint method.
    [Endpoint("/test/:id/porcodio")]
    public void TestMethod(HttpListenerRequest request, HttpListenerResponse response, Dictionary<string, string> parameters)
    {
        response.ContentType = "application/json";
        response.Send(new Data() { param = parameters["id"], count = counter++ }.Serialize());
        response.End();
    }
    
    //endpoint defining a path and a HttpMethod
    [Endpoint("/test2", HttpMethod = "POST")]
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
```

In this class 2 endpoints are defined. Endpoint methods are extended with the `EndpointAttribute`. The path and the `HttpMethod` of the endpoint can be specified.

if a segment of a path starts with `:` it is treated as a parameter. All parameters are parsed as string and passed in the `Dictionary parameters`. 

## IMPORTANT
Make sure to create a firewall rule for external access and:
- Run the server using administrator privileges. 

   **OR**
- See [this](https://learn.microsoft.com/en-us/windows/win32/http/add-urlacl) for more info.
