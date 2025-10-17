using lab03.Persistence;
using Microsoft.EntityFrameworkCore;

namespace lab03.Features;

public class GetBookSortedByTitleOrYearHandler(BookManagementContext dbContext)
{
    private readonly BookManagementContext _dbContext;
    
    public async Task<IResult> Handle(GetBookSortedByTitleOrYearRequest request)
    {
        List<Book> books = null;

        if (request.toOrderBy == "Title")
        {
            books = await _dbContext.Books.OrderBy(b => b.Title).ToListAsync();
        } else if (request.toOrderBy == "Year")
        {
            books = await _dbContext.Books.OrderBy(b => b.Year).ToListAsync();
        }
        else
        {
            throw new ArgumentException("Invalid request");
        }

        return books is not null ? Results.Ok(books) : Results.BadRequest("Invalid sort parameter");
    }
}