using lab03.Features;
using Microsoft.EntityFrameworkCore;

namespace lab03.Persistence;

public class BookManagementContext : DbContext
{
    public BookManagementContext(DbContextOptions<BookManagementContext> options) : base(options)
    {
    }

    public DbSet<Book> Books { get; set; } = null!;
}