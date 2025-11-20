using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace ProductManagement.Validators.Attributes;

/// <summary>
/// Validation attribute for enforcing price range constraints on decimal values.
/// Validates that a price falls within the specified minimum and maximum bounds.
/// </summary>
public class PriceRangeAttribute : ValidationAttribute
{
    private readonly decimal _min;
    private readonly decimal _max;

    /// <summary>
    /// Initializes a new instance of the PriceRangeAttribute class.
    /// </summary>
    /// <param name="min">The minimum allowed price value.</param>
    /// <param name="max">The maximum allowed price value.</param>
    /// <exception cref="ArgumentException">Thrown when min is greater than max.</exception>
    public PriceRangeAttribute(double min, double max)
    {
        if (min > max)
            throw new ArgumentException("min must be less than or equal to max", nameof(min));

        _min = Convert.ToDecimal(min);
        _max = Convert.ToDecimal(max);
    }

    /// <summary>
    /// Validates that the provided value is a valid price within the specified range.
    /// </summary>
    /// <param name="value">The price value to validate. Can be decimal, IConvertible, or string.</param>
    /// <param name="validationContext">The validation context containing metadata about the validation operation.</param>
    /// <returns>A ValidationResult indicating success or failure with appropriate error message.</returns>
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is null)
            return ValidationResult.Success;

        decimal price;
        try
        {
            if (value is decimal d)
            {
                price = d;
            }
            else if (value is IConvertible conv)
            {
                price = Convert.ToDecimal(conv, CultureInfo.InvariantCulture);
            }
            else if (value is string s &&
                     decimal.TryParse(s, NumberStyles.Number | NumberStyles.AllowCurrencySymbol, CultureInfo.CurrentCulture, out var parsed))
            {
                price = parsed;
            }
            else
            {
                 return new ValidationResult(FormatErrorMessage(validationContext?.DisplayName ?? "Value"));
            }
        }
        catch (Exception)
        {
            return new ValidationResult(FormatErrorMessage(validationContext?.DisplayName ?? "Value"));
        }

        if (price < _min || price > _max)
            return new ValidationResult(FormatErrorMessage(validationContext?.DisplayName ?? "Value"));

        return ValidationResult.Success;
    }

    /// <summary>
    /// Formats the error message with the price range bounds.
    /// </summary>
    /// <param name="name">The name of the property being validated.</param>
    /// <returns>A formatted error message with minimum and maximum price values.</returns>
    public override string FormatErrorMessage(string name)
    {
        var minStr = _min.ToString("C", CultureInfo.CurrentCulture);
        var maxStr = _max.ToString("C", CultureInfo.CurrentCulture);

        if (!string.IsNullOrEmpty(ErrorMessage))
        {
            try
            {
                return string.Format(CultureInfo.CurrentCulture, ErrorMessageString, name, minStr, maxStr);
            }
            catch
            {
                // Fall back to default if provided format is invalid
            }
        }

        return $"{name} must be between {minStr} and {maxStr}.";
    }
}