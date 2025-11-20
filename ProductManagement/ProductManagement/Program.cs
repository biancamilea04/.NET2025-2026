using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using ProductManagement.Common.Mapping;
using ProductManagement.Persistence;
using ProductManagement.Common.Middelware;
using ProductManagement.Features.Products;
using ProductManagement.Features.Request;
using ProductManagement.Features.DTOs;
using FluentValidation;
using FluentValidation.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc(
            "v1",
            new OpenApiInfo()
            {
                Title = "Product Management API",
                Version = "v1",
                Description = "API for managing products in the Product Management system."
            }
            );
    }
);

builder.Services.AddDbContext<ApplicationContext>(options =>
    options.UseSqlite("Data Source=productmanagement.db"));

builder.Services.AddAutoMapper(cfg => cfg.AddProfile<AdvancedProductMappingProfile>(), typeof(AdvancedProductMappingProfile));

builder.Services.AddScoped<CreateProductHandler>();
builder.Services.AddScoped<GetByIdProductHandler>();
builder.Services.AddScoped<GetAllProductsHandler>();
builder.Services.AddScoped<GetProductMetricsHandler>();

builder.Services.AddMemoryCache();

builder.Services.AddValidatorsFromAssemblyContaining<CreateProductProfileRequest>();
builder.Services.AddFluentValidationAutoValidation();

builder.Services.AddLogging(config =>
{
    config.AddConsole();
    config.AddDebug();
});

var app = builder.Build();

app.UseMiddleware<CorrelationMiddleware>();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationContext>();
    context.Database.EnsureCreated();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(
        c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "Product Management API v1");
            c.RoutePrefix = string.Empty;
            c.DisplayRequestDuration();
        });
    
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapPost("/products", async ( CreateProductProfileRequest req, CreateProductHandler handler ) =>
    await handler.Handle(req));
app.MapGet("/products/{id:guid}", async ( Guid id, GetByIdProductHandler handler ) =>
    await handler.Handle(new GetByIdProductRequest(id) ));
app.MapGet("/products", async (GetAllProductsHandler handler) =>
    await handler.Handle());
app.MapGet("/products/metrics/dashboard", async (GetProductMetricsHandler handler, CancellationToken ct) =>
    await handler.Handle(ct))
    .WithName("GetProductMetrics")
    .WithOpenApi()
    .Produces<ProductMetricsDto>(statusCode: 200)
    .WithTags("Product Metrics")
    .WithSummary("Get Product Metrics Dashboard")
    .WithDescription("Returns aggregated product metrics including inventory value, stock status breakdown, category distribution, and top products by price");

await app.RunAsync();
