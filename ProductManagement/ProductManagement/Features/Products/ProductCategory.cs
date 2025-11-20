namespace ProductManagement.Features;

/// <summary>
/// Defines the available product categories in the Product Management system.
/// Used to classify products and apply category-specific business rules and validations.
/// </summary>
public enum ProductCategory
{
    /// <summary>
    /// Electronics and technology products including smart devices, computers, and gadgets.
    /// Subject to minimum price of $50.00 and must contain technology-related keywords.
    /// </summary>
    Electronics = 0,
    
    /// <summary>
    /// Clothing and fashion products including apparel, footwear, and accessories.
    /// Requires brand name to be at least 3 characters long.
    /// </summary>
    Clothing = 1, 
    
    /// <summary>
    /// Books, media, and literature products.
    /// Subject to standard validation rules without category-specific restrictions.
    /// </summary>
    Books = 2, 
    
    /// <summary>
    /// Home and garden products including furniture, decor, and household items.
    /// Limited to maximum price of $200.00 and image URLs are filtered for privacy.
    /// Product names must not contain inappropriate content.
    /// </summary>
    Home = 3
}