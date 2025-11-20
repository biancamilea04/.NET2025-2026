using Microsoft.EntityFrameworkCore;
using ProductManagement.Features;

namespace ProductManagement.Persistence;

/// <summary>
/// Application database context for the Product Management system.
/// Manages the database connection, entity configurations, and data persistence operations.
/// </summary>
public class ApplicationContext(DbContextOptions<ApplicationContext> options) : DbContext(options)
{
    /// <summary>
    /// Gets or sets the DbSet for Product entities.
    /// Provides access to product records in the database for CRUD operations.
    /// </summary>
    public DbSet<Product> Products { get; set;}
}