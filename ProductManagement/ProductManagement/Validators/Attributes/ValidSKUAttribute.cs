using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace ProductManagement.Validators.Attributes;

public class ValidSKUAttribute : ValidationAttribute, IClientModelValidator
{
    private const string Pattern = "^[A-Za-z0-9-]{5,20}$";

    public ValidSKUAttribute()
    {
        ErrorMessage ??= "The SKU must be 5-20 characters long and contain only letters, numbers or hyphens.";
    }
    
    protected override ValidationResult? IsValid(object? value, ValidationContext? validationContext)
    {
        if (value is null) return ValidationResult.Success;

        var str = value as string ?? value.ToString();
        if (string.IsNullOrWhiteSpace(str)) return ValidationResult.Success; 
        
        var normalized = str.Replace(" ", string.Empty);

        if (Regex.IsMatch(normalized, Pattern))
            return ValidationResult.Success;

        var displayName = validationContext?.DisplayName ?? validationContext?.ObjectType?.Name ?? "SKU";
        var message = FormatErrorMessage(displayName);
        return new ValidationResult(message);
    }
    
    public void AddValidation(ClientModelValidationContext context)
    {
        context.Attributes.Add("data-val", "true");
        context.Attributes.Add("data-val-validsku", ErrorMessage ?? "The SKU must be 5-20 characters long and contain only letters, numbers or hyphens.");
        context.Attributes.Add("data-val-validsku-pattern", Pattern);
    }
}