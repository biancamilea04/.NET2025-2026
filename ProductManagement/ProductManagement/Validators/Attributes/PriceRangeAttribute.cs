using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace ProductManagement.Validators.Attributes;

public class PriceRangeAttribute : ValidationAttribute
{
    private readonly decimal _min;
    private readonly decimal _max;

    public PriceRangeAttribute(double min, double max)
    {
        if (min > max)
            throw new ArgumentException("min must be less than or equal to max", nameof(min));

        _min = Convert.ToDecimal(min);
        _max = Convert.ToDecimal(max);
    }

   protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
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