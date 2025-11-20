using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ProductManagement.Validators.Attributes;

/// <summary>
/// Validation attribute for validating that a value is one of the allowed product categories.
/// Enforces a whitelist of acceptable category values with case-insensitive comparison.
/// </summary>
public class ProductCategoryAttribute : ValidationAttribute
{
    private readonly HashSet<string> _allowedCategories;
    
    /// <summary>
    /// Initializes a new instance of the ProductCategoryAttribute class.
    /// </summary>
    /// <param name="allowedCategories">The array of allowed category values.</param>
    /// <exception cref="ArgumentException">Thrown when no allowed categories are provided.</exception>
    public ProductCategoryAttribute(params string[] allowedCategories)
    {
        if (allowedCategories == null || allowedCategories.Length == 0)
            throw new ArgumentException("At least one allowed category must be provided.", nameof(allowedCategories));

        _allowedCategories = new HashSet<string>(allowedCategories, StringComparer.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Validates that the provided value is one of the allowed categories.
    /// </summary>
    /// <param name="value">The category value to validate.</param>
    /// <param name="validationContext">The validation context containing metadata about the validation operation.</param>
    /// <returns>A ValidationResult indicating success or failure with appropriate error message.</returns>
    protected override ValidationResult? IsValid(object? value, ValidationContext? validationContext)
    {
        if (value is null) return ValidationResult.Success;

        var str = value as string ?? value.ToString();
        if (string.IsNullOrWhiteSpace(str)) return ValidationResult.Success;

        var candidate = str.Trim();
        if (_allowedCategories.Contains(candidate))
            return ValidationResult.Success;

        // Avoid nullable-analysis warning: check validationContext explicitly
        var displayName = validationContext != null
            ? (string.IsNullOrWhiteSpace(validationContext.DisplayName)
                ? (validationContext.ObjectType?.Name ?? "Category")
                : validationContext.DisplayName)
            : "Category";
        var message = FormatErrorMessage(displayName);
        return new ValidationResult(message);
    }

    /// <summary>
    /// Formats the error message with the list of allowed categories.
    /// </summary>
    /// <param name="name">The name of the property being validated.</param>
    /// <returns>A formatted error message listing all allowed categories.</returns>
    public override string FormatErrorMessage(string name)
    {
        var list = string.Join(", ", _allowedCategories);
        return $"{name} must be one of the following categories: {list}.";
    }
}