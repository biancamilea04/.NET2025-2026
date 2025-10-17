using lab03.Persistence;

namespace lab03.Features;

public class CreateBookHandler(BookManagementContext dbContext)
{
    private readonly BookManagementContext _dbContext;
    
    public async Task<IResult> Handle(CreateBookRequest request)
    {
        var book = new Book(Guid.NewGuid(), request.Title, request.Author, int.Parse(request.Year));
        _dbContext.Books.Add(book);
        await _dbContext.SaveChangesAsync();
        
        return Results.Created($"/books/{book.Id}", book);
    }
}