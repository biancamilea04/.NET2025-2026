
using lab03.Features;
using lab03.Persistence;

public class GetAllBooksHandler( BookManagementContext dbContext ) 
{
    private readonly BookManagementContext _dbContext;

    public IEnumerable<Book> Handle(GetBooksRequest request)
    {
        int page = request.Page;
        int pageSize = request.PageSize;
        
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 10;
        var skip = (page - 1) * pageSize;
        return _dbContext.Books
            .OrderBy(b => b.Id)
            .Skip(skip)
            .Take(pageSize)
            .ToList();
    }
   
}