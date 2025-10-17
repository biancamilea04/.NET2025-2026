using lab03.Persistence;

namespace lab03.Features;

public class GetBooksByAuthorHandler( BookManagementContext dbContext )
{
    private readonly BookManagementContext _dbContext;
    
    public async Task<IResult> Handle( GetBooksByAuthorRequest request )
    {
        var books = _dbContext.Books.Where( book => book.Author == request.Author );
        return Results.Ok( books );
    }
}