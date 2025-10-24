using FluentValidation;
using ProductManagement.Features.Product;

namespace ProductManagement.Validator;

public class CreateProductProfileValidator : AbstractValidator<CreateProductProfileRequest>
{
    private ILogger<CreateProductHandler> _logger;
    public CreateProductProfileValidator( ILogger<CreateProductHandler> logger)
    {
        _logger = logger;
        
        _logger.LogDebug("CreateProductProfileValidator constructed");
         
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Product name is required.")
            .MaximumLength(100).WithMessage("Product name cannot exceed 100 characters.");

        RuleFor(x => x.Brand)
            .NotEmpty().WithMessage("Brand is required.")
            .MaximumLength(50).WithMessage("Brand cannot exceed 50 characters.");

        RuleFor(x => x.SKU)
            .NotEmpty().WithMessage("SKU is required.")
            .MaximumLength(30).WithMessage("SKU cannot exceed 30 characters.");

        RuleFor(x => x.Category)
            .IsInEnum().WithMessage("Invalid product category.");

        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("Price must be greater than zero.");

        RuleFor(x => x.ReleaseDate)
            .LessThanOrEqualTo(DateTime.Now).WithMessage("Release date cannot be in the future.");

        RuleFor(x => x.ImageUrl)
            .MaximumLength(200).WithMessage("Image URL cannot exceed 200 characters.")
            .When(x => !string.IsNullOrEmpty(x.ImageUrl));

        RuleFor(x => x.StockQuantity)
            .GreaterThanOrEqualTo(0).WithMessage("Stock quantity cannot be negative.");
    }
}