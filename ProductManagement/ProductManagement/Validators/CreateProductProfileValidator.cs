using System.Text.RegularExpressions;
using AutoMapper;
using FluentValidation;
using ProductManagement.Features;
using ProductManagement.Persistence;
using Microsoft.EntityFrameworkCore;
using ProductManagement.Features.Request;

namespace ProductManagement.Validators;

public class CreateProductProfileValidator : AbstractValidator<CreateProductProfileRequest>
{
    private readonly ApplicationContext context;
    private readonly ILogger<CreateProductProfileValidator> logger;

    public CreateProductProfileValidator(ApplicationContext context, ILogger<CreateProductProfileValidator> logger)
    {
        this.context = context;
        this.logger = logger;
        
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Product name is required.")
            .Length(1, 200).WithMessage("Product name must be between 1 and 200 characters.")
            .Must(BeValidName).WithMessage("Product name contains invalid words or is empty.")
            .Must(BeUniqueName).WithMessage("Product name must be unique.");
        
        RuleFor(x => x.Brand)
            .NotEmpty().WithMessage("Product brand is required.")
            .Length(2,100).WithMessage("Product brand must be between 2 and 100 characters.")
            .Must(BeValidBrandName).WithMessage("Product brand contains invalid characters.");
        
        RuleFor( x => x.SKU)
            .NotEmpty().WithMessage("Product SKU is required.")
            .Must(BeValidSKU).WithMessage("Product SKU must be exactly 8 uppercase alphanumeric characters.")
            .Must(BeUniqueSKU).WithMessage("Product SKU must be unique.");
        
        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("Product price must be greater than zero.")
            .LessThan(10000).WithMessage("Product price must be less than or equal to 10,000.");
        
        RuleFor(x => x.StockQuantity)
            .GreaterThan(0).WithMessage("Product stock quantity must be positive.")
            .LessThanOrEqualTo(100000).WithMessage("Product stock quantity must be less than or equal to 100,000.");
        
        RuleFor(x => x.ImageUrl)
            .Must(BeValidImageUrl).WithMessage("Product image URL is not valid or has unsupported format.");
        
        RuleFor( x => x)
            .MustAsync( async (request, cancellation) => await PassBusinessRules(request))
            .WithMessage("Product does not comply with business rules.");
    }

    private bool BeValidName(string name)
    {
        
        List<string> invalidNames = new List<string>() { "inval1", "inval2", "inval3" };
        if (string.IsNullOrWhiteSpace(name))
        {
            return false;
        }
        
        if (invalidNames.Contains(name.ToLower()))
        {
            return false;
        }
        
        var wordList = name.Split( ' ', StringSplitOptions.RemoveEmptyEntries);
        foreach (var word in wordList)
        {
            if (invalidNames.Contains(word.ToLower()))
            {
                return false;
            }
        }
        
        return true;
    }
    
    private bool BeUniqueName(string name)
    {
        var existingProduct = context.Products.FirstOrDefault(p => p.Name.ToLower() == name.ToLower());
        return existingProduct == null;
    }
    
    private bool BeValidBrandName(string brand)
    {
        Regex brandPattern = new Regex("^[A-Za-z0-9 &.-]+$");
        
        if (string.IsNullOrWhiteSpace(brand))
        {
            return false;
        }
        
        return brandPattern.IsMatch(brand);
    }

    private bool BeValidSKU(string sku)
    {
        Regex skuPattern = new Regex("^[A-Z0-9]{5,20}$");
        
        if (string.IsNullOrWhiteSpace(skuPattern.ToString()))
        {
            return false;
        }
        
        return skuPattern.IsMatch(sku);
    }
    
    private bool BeUniqueSKU(string sku)
    {
        var existingProduct = context.Products.FirstOrDefault(p => p.SKU.ToUpper() == sku.ToUpper());
        return existingProduct == null;
    }
    
    private bool BeValidImageUrl(string? imageUrl)
    {
        List<string> allowedImageExtensions = new List<string>()
        {
            ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".webp"
        };

        if (string.IsNullOrWhiteSpace(imageUrl))
        {
            return true;
        }

        Uri uriResult;
        bool result = Uri.TryCreate(imageUrl, UriKind.Absolute, out uriResult)
                      && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);

        if (!result)
        {
            return false;
        }

        string fileExtension = Path.GetExtension(uriResult.AbsolutePath).ToLower();
        if (!allowedImageExtensions.Contains(fileExtension)) 
        {
            return false;
        }

        return result;
    }
    
    public async Task<bool> PassBusinessRules(CreateProductProfileRequest request)
     {
         try
         {
             var start = DateTime.UtcNow;
             
             var dayStart = DateTime.UtcNow.Date;
             var dayEnd = dayStart.AddDays(1);
             var todaysCount = await context.Products
                 .Where(p => p.CreatedAt >= dayStart && p.CreatedAt < dayEnd)
                 .CountAsync();

             if (todaysCount >= 500)
             {
                 logger.LogWarning("BusinessRule[DailyLimit] failed for SKU={SKU}. Today's added products={Count} (limit=500)", request.SKU, todaysCount);
                 return false;
             }
             logger.LogInformation("BusinessRule[DailyLimit] passed for SKU={SKU}. Today's added products={Count}", request.SKU, todaysCount);

             if (request.Category == ProductCategory.Electronics && request.Price < 50.00m)
             {
                 logger.LogWarning("BusinessRule[ElectronicsMinPrice] failed for SKU={SKU}. Category=Electronics, Price={Price} < 50.00", request.SKU, request.Price);
                 return false;
             }
             logger.LogInformation("BusinessRule[ElectronicsMinPrice] passed for SKU={SKU}. Price={Price}", request.SKU, request.Price);
             
             if (request.Category == ProductCategory.Home)
             {
                 var restrictedWords = new List<string> { "restricted", "banned", "prohibited", "explicit" };
                 var nameTokens = request.Name?.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries) ?? Array.Empty<string>();
                 var matches = nameTokens.Select(t => t.ToLower()).Intersect(restrictedWords).ToList();
                 if (matches.Any())
                 {
                     logger.LogWarning("BusinessRule[HomeContentRestrictions] failed for SKU={SKU}. Found restricted words: {Words}", request.SKU, string.Join(',', matches));
                     return false;
                 }
                 logger.LogInformation("BusinessRule[HomeContentRestrictions] passed for SKU={SKU}.", request.SKU);
             }

             if (request.Price > 500.00m && request.StockQuantity > 10)
             {
                 logger.LogWarning("BusinessRule[HighValueStockLimit] failed for SKU={SKU}. Price={Price} > 500 and StockQuantity={Stock} > 10", request.SKU, request.Price, request.StockQuantity);
                 return false;
             }
             logger.LogInformation("BusinessRule[HighValueStockLimit] passed for SKU={SKU}. Price={Price}, StockQuantity={Stock}", request.SKU, request.Price, request.StockQuantity);

             var duration = DateTime.UtcNow - start;
             logger.LogInformation("PassBusinessRules completed for SKU={SKU} in {Duration}ms - result=Success", request.SKU, duration.TotalMilliseconds);

             return true;
         }
         catch (Exception ex)
         {
             logger.LogError(ex, "PassBusinessRules encountered an exception for SKU={SKU}", request?.SKU);
             return false;
         }
     }
 }
