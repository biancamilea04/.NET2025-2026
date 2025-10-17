using lab03.Persistence;
using lab03.Middleware;

namespace lab03.Features;

public class CreateBookHandler
{
    private readonly BookManagementContext _dbContext;

    public CreateBookHandler(BookManagementContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IResult> Handle(CreateBookRequest request)
    {
        if (!int.TryParse(request.Year, out var year) || request.Year.Length != 4)
        {
            // Throw an ApiException so the middleware returns a 400 JSON response
            throw new ApiException("Year must be a valid four-digit number.", statusCode: 400);
        }

        var book = new Book(Guid.NewGuid(), request.Title, request.Author, year);
        _dbContext.Books.Add(book);
        await _dbContext.SaveChangesAsync();

        return Results.Created($"/books/{book.Id}", book);
    }
}