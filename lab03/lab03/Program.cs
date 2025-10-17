using lab03.Features;
using lab03.Persistence;
using lab03.Validators;
using Microsoft.EntityFrameworkCore;
using FluentValidation;
using lab03.Middleware;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddDbContext<BookManagementContext>(options =>
{
    options.UseSqlite("Data Source=bookmanagement.db");
});
builder.Services.AddScoped<CreateBookHandler>();
builder.Services.AddScoped<ReadBookHandler>();
builder.Services.AddScoped<DeleteBookHandler>();
builder.Services.AddScoped<UpdateBookHandler>();
builder.Services.AddScoped<GetBooksByAuthorHandler>();
builder.Services.AddScoped<GetBookSortedByTitleOrYearHandler>();

builder.Services.AddValidatorsFromAssemblyContaining<CreateBookValidator>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<BookManagementContext>();
    dbContext.Database.EnsureCreated();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseCustomExceptionHandler();

app.UseHttpsRedirection();

app.MapPost("/books", async ([FromBody] CreateBookRequest request, [FromServices] CreateBookHandler handler) =>
{
    return await handler.Handle(request);
});

app.MapPost("/books/query", async ([FromBody] ReadBookRequest request, [FromServices] ReadBookHandler handler) =>
{
    return await handler.Handle(request);
});

app.MapGet("books/all", async (BookManagementContext dbContext) =>
{
    var books = await dbContext.Books.ToListAsync();
    return Results.Ok(books);
});

app.MapDelete("/books/{id:guid}", async (Guid id, [FromServices] DeleteBookHandler handler) =>
{
    return await handler.Handle(new DeleteBookRequest(id));
});

app.MapPut("/books", async ([FromBody] UpdateBookRequest request, [FromServices] UpdateBookHandler handler) =>
{
    return await handler.Handle(request);
});

app.MapPost("/books/author", async ([FromBody] GetBooksByAuthorRequest request, [FromServices] GetBooksByAuthorHandler handler) =>
{
    return await handler.Handle(request);
});

app.MapPost("/books/sorted", async ([FromBody] GetBookSortedByTitleOrYearRequest request, [FromServices] GetBookSortedByTitleOrYearHandler handler) =>
{
    return await handler.Handle(request);
});

app.Run();

