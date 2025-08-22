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

var rnd = new Random();

var products = new[]
{
    new { Id = 1, Name = "Laptop Nebula X1", Price = 1500, Stock = rnd.Next(1, 101) },
    new { Id = 2, Name = "Smartphone Nebula S", Price = 899, Stock = rnd.Next(1, 101) },
    new { Id = 3, Name = "Casque VR Nebula Vision", Price = 499, Stock = rnd.Next(1, 101) }
};

// Endpoints
app.MapGet("/products", () => products);
app.MapGet("/products/{id}", (int id) =>
    products.FirstOrDefault(p => p.Id == id) is var product && product != null
        ? Results.Ok(product)
        : Results.NotFound());

app.Run();

