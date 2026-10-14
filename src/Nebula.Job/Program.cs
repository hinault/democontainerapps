using System.Net.Http;

var products = Environment.GetEnvironmentVariable("PRODUCTS_API_URL") ?? "http://nebula-products-api:80";
var orders = Environment.GetEnvironmentVariable("ORDERS_API_URL") ?? "http://nebula-orders-api:80";

Console.WriteLine("Nebula.Job starting...");
Console.WriteLine($"Products API: {products}");
Console.WriteLine($"Orders API: {orders}");

using var http = new HttpClient();

try
{
    Console.WriteLine("Checking upstream APIs...");

    var pResp = await SafeGet(http, new Uri(new Uri(products), "/health"));
    Console.WriteLine($"Products API health: {pResp}");

    var oResp = await SafeGet(http, new Uri(new Uri(orders), "/health"));
    Console.WriteLine($"Orders API health: {oResp}");

    // Simulate job work
    Console.WriteLine("Processing job items...");
    await Task.Delay(TimeSpan.FromSeconds(3));
    Console.WriteLine("Job completed successfully.");
    Environment.ExitCode = 0;
}
catch (Exception ex)
{
    Console.Error.WriteLine($"Job failed: {ex}");
    Environment.ExitCode = 2;
}

static async Task<string> SafeGet(HttpClient http, Uri uri)
{
    try
    {
        using var r = await http.GetAsync(uri, HttpCompletionOption.ResponseHeadersRead);
        return r.IsSuccessStatusCode ? "Healthy" : $"Unhealthy ({(int)r.StatusCode})";
    }
    catch (Exception e)
    {
        return $"Error: {e.Message}";
    }
}
