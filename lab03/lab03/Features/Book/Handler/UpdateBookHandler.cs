using lab03.Persistence;
using Microsoft.EntityFrameworkCore;

namespace lab03.Features;

public class UpdateBookHandler( BookManagementContext dbContext )
{
    private readonly BookManagementContext _dbContext;
    
    public async Task<IResult> Handle(UpdateBookRequest updateBookRequest)
    {
        var book = await _dbContext.Books
            .FirstOrDefaultAsync( book => book.Id == updateBookRequest.Id);
        if (book is null)
        {
            return Results.NotFound();
        }

        var bookUpdated = book with
        {
            Title = updateBookRequest?.Title ?? book.Title,
            Author = updateBookRequest?.Author ?? book.Author,
            Year = updateBookRequest?.Year ?? book.Year
        };
        
        dbContext.Entry(book).CurrentValues.SetValues(bookUpdated);

        return Results.Ok(book);
    }
}