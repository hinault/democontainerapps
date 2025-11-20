var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

var orders = new List<dynamic>
{
    new { Id = 1, ProductId = 1, Quantity = 2, Total = 3000 },
    new { Id = 2, ProductId = 2, Quantity = 1, Total = 899 }
};

// Endpoints
app.MapGet("/orders", () => orders);
app.MapPost("/orders", (dynamic order) =>
{
    order.Id = orders.Count + 1;
    orders.Add(order);
    return Results.Created($"/orders/{order.Id}", order);
});

app.Run();


