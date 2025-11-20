using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace ProductManagement.Validators.Attributes;

/// <summary>
/// Validation attribute for validating SKU (Stock Keeping Unit) format.
/// Ensures that SKU values conform to the required pattern: 5-20 characters containing only letters, numbers, or hyphens.
/// </summary>
public class ValidSKUAttribute : ValidationAttribute, IClientModelValidator
{
    private const string Pattern = "^[A-Za-z0-9-]{5,20}$";

    /// <summary>
    /// Initializes a new instance of the ValidSKUAttribute class.
    /// Sets default error message if none is provided.
    /// </summary>
    public ValidSKUAttribute()
    {
        ErrorMessage ??= "The SKU must be 5-20 characters long and contain only letters, numbers or hyphens.";
    }
    
    /// <summary>
    /// Validates that the provided value matches the SKU format requirements.
    /// </summary>
    /// <param name="value">The SKU value to validate.</param>
    /// <param name="validationContext">The validation context containing metadata about the validation operation.</param>
    /// <returns>A ValidationResult indicating success or failure with appropriate error message.</returns>
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
    
    /// <summary>
    /// Adds client-side validation attributes to support HTML5 validation.
    /// </summary>
    /// <param name="context">The client model validation context.</param>
    public void AddValidation(ClientModelValidationContext context)
    {
        context.Attributes.Add("data-val", "true");
        context.Attributes.Add("data-val-validsku", ErrorMessage ?? "The SKU must be 5-20 characters long and contain only letters, numbers or hyphens.");
        context.Attributes.Add("data-val-validsku-pattern", Pattern);
    }
}