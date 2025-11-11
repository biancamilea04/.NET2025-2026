using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ProductManagement.Validators.Attributes;
public class ProductCategoryAttribute : ValidationAttribute
{
    private readonly HashSet<string> _allowedCategories;
    
    public ProductCategoryAttribute(params string[] allowedCategories)
    {
        if (allowedCategories == null || allowedCategories.Length == 0)
            throw new ArgumentException("At least one allowed category must be provided.", nameof(allowedCategories));

        _allowedCategories = new HashSet<string>(allowedCategories, StringComparer.OrdinalIgnoreCase);
    }

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

    public override string FormatErrorMessage(string name)
    {
        var list = string.Join(", ", _allowedCategories);
        return $"{name} must be one of the following categories: {list}.";
    }
}