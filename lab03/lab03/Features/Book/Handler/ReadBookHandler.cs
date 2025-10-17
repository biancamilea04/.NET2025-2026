using lab03.Persistence;
using Microsoft.EntityFrameworkCore;

namespace lab03.Features;

public class ReadBookHandler(BookManagementContext dbContext)
{
    private readonly BookManagementContext _dbContext;

    public async Task<IResult> Handle(ReadBookRequest readBookRequest)
    {
        var book = await _dbContext.Books
            .FirstOrDefaultAsync( book => book.Title == readBookRequest.Title && book.Author == readBookRequest.Author);
        return book is not null ? Results.Ok(book) : Results.NotFound();
    }  
}