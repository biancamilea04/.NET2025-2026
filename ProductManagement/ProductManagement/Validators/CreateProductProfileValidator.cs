using System.Text.RegularExpressions;
using FluentValidation;
using ProductManagement.Features;
using ProductManagement.Persistence;
using Microsoft.EntityFrameworkCore;
using ProductManagement.Features.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

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

       RuleFor(x => x.Brand)
            .MinimumLength(3).When(x => x.Category == ProductCategory.Clothing)
            .WithMessage("For Clothing products the brand name must be at least 3 characters.");
        
        RuleFor( x => x.SKU)
            .NotEmpty().WithMessage("Product SKU is required.")
            .Must(BeValidSKU).WithMessage("Product SKU must be exactly 8 uppercase alphanumeric characters.")
            .Must(BeUniqueSKU).WithMessage("Product SKU must be unique.");
        
        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("Product price must be greater than zero.")
            .LessThan(10000).WithMessage("Product price must be less than or equal to 10,000.");

        RuleFor(x => x.Price)
            .LessThanOrEqualTo(200m).When(x => x.Category == ProductCategory.Home)
            .WithMessage("Home products must have a price of at most $200.00.");

        RuleFor(x => x.StockQuantity)
            .GreaterThan(0).WithMessage("Product stock quantity must be positive.")
            .LessThanOrEqualTo(100000).WithMessage("Product stock quantity must be less than or equal to 100,000.");

        RuleFor(x => x)
            .Must(request => request.Price <= 100m || request.StockQuantity <= 20)
            .WithMessage("Expensive products (price > $100) must have stock quantity of 20 units or less.");

        RuleFor(x => x.ImageUrl)
            .Must(BeValidImageUrl).WithMessage("Product image URL is not valid or has unsupported format.");
        
        RuleFor(x => x)
            .Must(request => request.Category != ProductCategory.Electronics || ContainTechnologyKeywords(request.Name))
            .WithMessage("Electronics product names must contain technology-related keywords (e.g. 'smart', 'wireless', 'bluetooth').");

       RuleFor(x => x.Name)
            .Must(BeAppropriateForHome).When(x => x.Category == ProductCategory.Home)
            .WithMessage("Home product name contains inappropriate words.");

        RuleFor( x => x)
            .MustAsync( async (request, _) => await PassBusinessRules(request))
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

    private bool ContainTechnologyKeywords(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) return false;
        var techKeywords = new List<string>
        {
            "smart","wireless","bluetooth","4k","oled","led","processor","chip","ssd","hdd","ram","gps","nfc","touch","battery","camera","hd","ultra","tablet","laptop","phone","charger","usb","hdmi","wifi","sensor","display","keyboard","monitor","speaker","microphone"
        };

        var tokens = name.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).Select(t => t.ToLower());
        return tokens.Any(t => techKeywords.Any(k => t.Contains(k)));
    }

    private bool BeValidSKU(string sku)
    {
        Regex skuPattern = new Regex("^[A-Z0-9]{8}$");
        
        if (string.IsNullOrWhiteSpace(sku))
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

    private bool BeAppropriateForHome(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) return false;
        var inappropriate = new List<string> { "porn", "sex", "drugs", "explicit", "banned", "illegal" };
        var tokens = name.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).Select(t => t.ToLower());
        return !tokens.Any(t => inappropriate.Contains(t));
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
             
             if (request.Category == ProductCategory.Electronics)
             {
                 if (request.Price < 50.00m)
                 {
                     logger.LogWarning("BusinessRule[ElectronicsMinPrice] failed for SKU={SKU}. Category=Electronics, Price={Price} < 50.00", request.SKU, request.Price);
                     return false;
                 }
                 logger.LogInformation("BusinessRule[ElectronicsMinPrice] passed for SKU={SKU}. Price={Price}", request.SKU, request.Price);

                 var fiveYearsAgo = DateTime.UtcNow.AddYears(-5);
                 if (request.ReleaseDate < fiveYearsAgo)
                 {
                     logger.LogWarning("BusinessRule[ElectronicsRecentRelease] failed for SKU={SKU}. ReleaseDate={ReleaseDate} is older than 5 years", request.SKU, request.ReleaseDate);
                     return false;
                 }
                 logger.LogInformation("BusinessRule[ElectronicsRecentRelease] passed for SKU={SKU}. ReleaseDate={ReleaseDate}", request.SKU, request.ReleaseDate);

                 if (!ContainTechnologyKeywords(request.Name))
                 {
                     logger.LogWarning("BusinessRule[ElectronicsNameKeywords] failed for SKU={SKU}. Name lacks technology keywords", request.SKU);
                     return false;
                 }
                 logger.LogInformation("BusinessRule[ElectronicsNameKeywords] passed for SKU={SKU}.", request.SKU);
             }
             
             if (request.Category == ProductCategory.Home)
             {
                 if (request.Price > 200.00m)
                 {
                     logger.LogWarning("BusinessRule[HomeMaxPrice] failed for SKU={SKU}. Price={Price} > 200.00", request.SKU, request.Price);
                     return false;
                 }
                 logger.LogInformation("BusinessRule[HomeMaxPrice] passed for SKU={SKU}. Price={Price}", request.SKU, request.Price);

                 if (!BeAppropriateForHome(request.Name))
                 {
                     logger.LogWarning("BusinessRule[HomeAppropriateName] failed for SKU={SKU}. Name contains inappropriate words", request.SKU);
                     return false;
                 }
                 logger.LogInformation("BusinessRule[HomeAppropriateName] passed for SKU={SKU}.", request.SKU);
             }
             
             if (request.Category == ProductCategory.Clothing)
             {
                 if (string.IsNullOrWhiteSpace(request.Brand) || request.Brand.Length < 3)
                 {
                     logger.LogWarning("BusinessRule[ClothingBrandLength] failed for SKU={SKU}. Brand length={Length}", request.SKU, request.Brand?.Length ?? 0);
                     return false;
                 }
                 logger.LogInformation("BusinessRule[ClothingBrandLength] passed for SKU={SKU}. Brand={Brand}", request.SKU, request.Brand);
             }
             
             if (request.Price > 100.00m && request.StockQuantity > 20)
             {
                 logger.LogWarning("BusinessRule[ExpensiveStockLimit] failed for SKU={SKU}. Price={Price} > 100.00 and StockQuantity={Stock} > 20", request.SKU, request.Price, request.StockQuantity);
                 return false;
             }
             logger.LogInformation("BusinessRule[ExpensiveStockLimit] passed for SKU={SKU}. Price={Price}, StockQuantity={Stock}", request.SKU, request.Price, request.StockQuantity);
             
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
