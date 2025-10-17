using lab03.Persistence;
using Microsoft.EntityFrameworkCore;
using lab03.Middleware;

namespace lab03.Features;

public class DeleteBookHandler
{
    private readonly BookManagementContext _dbContext;

    public DeleteBookHandler(BookManagementContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IResult> Handle(DeleteBookRequest request)
    {
        var book = await _dbContext.Books.FirstOrDefaultAsync(book => book.Id == request.Id);
        if (book is null)
        {
            // Throw ApiException with 404 to be formatted by middleware
            throw new ApiException("Book not found.", statusCode: 404);
        }

        _dbContext.Books.Remove(book);
        await _dbContext.SaveChangesAsync();
        return Results.NoContent();
    }
}