using System.Net.Mime;
using System.Text.Json;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Nebula.Web.Components;
using Nebula.Web.HealthChecks;

var builder = WebApplication.CreateBuilder(args);

var productsApiUrl = builder.Configuration["ApiSettings:ProductsApiUrl"];
var ordersApiUrl = builder.Configuration["ApiSettings:OrdersApiUrl"];

builder.Services.AddHttpClient("ProductsApi", client =>
{
    client.BaseAddress = new Uri(productsApiUrl!);
    client.Timeout = TimeSpan.FromSeconds(5);
});

builder.Services.AddHttpClient("OrdersApi", client =>
{
    client.BaseAddress = new Uri(ordersApiUrl!);
    client.Timeout = TimeSpan.FromSeconds(5);
});

// Health checks for upstream APIs
builder.Services.AddHealthChecks()
    .AddCheck<ProductsApiHealthCheck>("products_api")
    .AddCheck<OrdersApiHealthCheck>("orders_api");

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Make Kestrel listen on the ports expected by Azure Container Apps probes
builder.WebHost.ConfigureKestrel(options =>
{
    // HTTP endpoints for probes: 8080 (HTTP) and 8081 (TCP readiness)
    options.ListenAnyIP(8080);
    options.ListenAnyIP(8081);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseStatusCodePagesWithReExecute("/not-found");



// Health endpoints used by the container probes
var healthOptions = new HealthCheckOptions
{
    ResponseWriter = async (context, report) =>
    {
        context.Response.ContentType = MediaTypeNames.Application.Json;
        var result = JsonSerializer.Serialize(new
        {
            status = report.Status.ToString(),
            checks = report.Entries.Select(e => new { name = e.Key, status = e.Value.Status.ToString(), description = e.Value.Description })
        });
        await context.Response.WriteAsync(result);
    }
};

app.MapHealthChecks("/health", healthOptions);
app.MapHealthChecks("/startup", healthOptions);

app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
