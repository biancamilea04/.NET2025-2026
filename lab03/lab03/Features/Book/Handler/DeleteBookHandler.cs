using lab03.Persistence;
using Microsoft.EntityFrameworkCore;

namespace lab03.Features;

public class DeleteBookHandler(BookManagementContext dbContext)
{
    private readonly BookManagementContext _dbContext;

    public async Task<IResult> Handle(DeleteBookRequest request)
    {
        var book = await _dbContext.Books.FirstOrDefaultAsync(book => book.Id == request.Id);
        if (book is null)
        {
            return Results.NotFound();
        }

        _dbContext.Books.Remove(book);
        await _dbContext.SaveChangesAsync();
        return Results.NoContent();
    }
}