namespace ProductManagement.Features;

/// <summary>
/// Represents a product in the Product Management system.
/// Contains all essential product information including pricing, inventory, and categorization.
/// </summary>
public class Product
{
    /// <summary>
    /// Gets or sets the unique identifier for the product.
    /// </summary>
    public Guid Id { get; set; } 
    
    /// <summary>
    /// Gets or sets the name of the product.
    /// Required field that must be unique within the system.
    /// </summary>
    public required string Name { get; set; }
    
    /// <summary>
    /// Gets or sets the brand or manufacturer of the product.
    /// Required field that identifies the product's source.
    /// </summary>
    public required string Brand { get; set; } 
    
    /// <summary>
    /// Gets or sets the Stock Keeping Unit (SKU) - a unique identifier for inventory tracking.
    /// Required field that must follow the format: 8 uppercase alphanumeric characters.
    /// </summary>
    public required string SKU { get; set; } 
    
    /// <summary>
    /// Gets or sets the product category classification.
    /// Determines which category the product belongs to (Electronics, Clothing, Books, Home).
    /// </summary>
    public ProductCategory Category { get; set; } 
    
    /// <summary>
    /// Gets or sets the price of the product in decimal format.
    /// Should be greater than zero and less than 10,000.
    /// </summary>
    public decimal Price { get; set; } 
    
    /// <summary>
    /// Gets or sets the URL pointing to the product's image.
    /// Optional field; may be null for certain product categories like Home.
    /// </summary>
    public string? ImageUrl { get; set; } 
    
    /// <summary>
    /// Gets or sets a value indicating whether the product is currently available for purchase.
    /// Derived from the stock quantity; true if stock is greater than zero.
    /// </summary>
    public bool IsAvailable { get; set; } 
    
    /// <summary>
    /// Gets or sets the current quantity of the product in stock.
    /// Must be positive and not exceed 100,000 units.
    /// </summary>
    public int StockQuantity { get; set; } 
    
    /// <summary>
    /// Gets or sets the date when the product was first released or made available.
    /// Used to calculate product age and apply category-specific rules.
    /// </summary>
    public DateTime ReleaseDate { get; set; } 
    
    /// <summary>
    /// Gets or sets the timestamp when the product was created in the system.
    /// Automatically set to UTC time at creation.
    /// </summary>
    public DateTime CreatedAt { get; set; }
    
    /// <summary>
    /// Gets or sets the timestamp when the product was last updated.
    /// Tracks the most recent modification to the product record.
    /// </summary>
    public DateTime UpdatedAt { get; set; }
}