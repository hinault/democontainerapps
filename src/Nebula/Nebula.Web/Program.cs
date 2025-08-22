using Nebula.Web.Components;

var builder = WebApplication.CreateBuilder(args);

var productsApiUrl = builder.Configuration["ApiSettings:ProductsApiUrl"];
var ordersApiUrl = builder.Configuration["ApiSettings:OrdersApiUrl"];

builder.Services.AddHttpClient("ProductsApi", client =>
{
    client.BaseAddress = new Uri(productsApiUrl!);
});

builder.Services.AddHttpClient("OrdersApi", client =>
{
    client.BaseAddress = new Uri(ordersApiUrl!);
});

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForErrors: true);

app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
