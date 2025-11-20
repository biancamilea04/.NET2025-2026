using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using ProductManagement.Common.Mapping;
using ProductManagement.Persistence;
using ProductManagement.Common.Middelware;
using ProductManagement.Features.Products;
using ProductManagement.Features.Request;
using FluentValidation;
using FluentValidation.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

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

builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddDbContext<ApplicationContext>(options =>
    options.UseSqlite("Data Source=productmanagement.db"));

builder.Services.AddScoped<CreateProductHandler>();
builder.Services.AddScoped<GetByIdProductHandler>();
builder.Services.AddScoped<GetAllProductsHandler>();

builder.Services.AddAutoMapper(cfg => cfg.AddProfile<AdvancedProductMappingProfile>(), typeof(AdvancedProductMappingProfile));

builder.Services.AddValidatorsFromAssemblyContaining<CreateProductProfileRequest>();
builder.Services.AddFluentValidationAutoValidation();

var app = builder.Build();

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

app.UseMiddleware<CorrerationMiddleware>();

app.UseHttpsRedirection();

app.MapPost("/products", async ( CreateProductProfileRequest req, CreateProductHandler handler ) =>
    await handler.Handle(req));
app.MapGet("/products/{id:guid}", async ( Guid id, GetByIdProductHandler handler ) =>
    await handler.Handle(new GetByIdProductRequest(id) ));
app.MapGet("/products", async (GetAllProductsHandler handler) =>
    await handler.Handle());

await app.RunAsync();
