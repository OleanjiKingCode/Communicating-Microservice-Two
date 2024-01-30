using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using System.Net.Http;
using System.Threading.Tasks;

var builder = WebApplication.CreateBuilder(args);

// Enable CORS for development (customize as needed)
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
    });
});

// Add Swagger for API documentation
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Consumer Service API", Version = "v1" });
});

var app = builder.Build();

// Middleware configuration
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Consumer Service API v1"));
}

// Enable CORS
app.UseCors();

// API endpoint for consuming products
app.MapGet("/api/consume", async context =>
{
    using (var client = new HttpClient())
    {
        try
        {
            // Simulate authentication by adding headers (customize as needed)
            client.DefaultRequestHeaders.Add("Authorization", "Bearer your_access_token");

            // Consume products API from the Product Service
            var result = await client.GetStringAsync("https://localhost:7033/api/products");
            await context.Response.WriteAsync(result);
        }
        catch (HttpRequestException ex)
        {
            context.Response.StatusCode = 500;
            await context.Response.WriteAsync($"Error: {ex.Message}");
        }
    }
});

app.Run();
