using lab03.Features;
using Microsoft.EntityFrameworkCore;

namespace lab03.Persistence;

public class BookManagementContext(DbContextOptions<BookManagementContext> options) : DbContext(options)
{
    public DbSet<Book> Books { get; set; }
}