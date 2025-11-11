using Microsoft.EntityFrameworkCore;
using ProductManagement.Features;

namespace ProductManagement.Persistence;

public class ApplicationContext(DbContextOptions<ApplicationContext> options) : DbContext(options)
{
    public DbSet<Product> Products { get; set;}
}