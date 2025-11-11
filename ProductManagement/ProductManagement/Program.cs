using Microsoft.EntityFrameworkCore;
using ProductManagement.Persistence;
using ProductManagement.Common.Middelware;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddDbContext<ApplicationContext>(options =>
    options.UseSqlite("Data Source=productmanagement.db"));

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseMiddleware<CollerationMiddleware>();

app.UseHttpsRedirection();

await app.RunAsync();
