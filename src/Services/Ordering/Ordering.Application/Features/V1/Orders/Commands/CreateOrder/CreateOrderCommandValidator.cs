using FluentValidation;

namespace Ordering.Application.Features.V1.Orders;

public class CreateOrderCommandValidator : AbstractValidator<CreateOrderCommand>
{
    public CreateOrderCommandValidator()
    {
        RuleFor(p => p.Username)
            .NotEmpty().WithMessage("{Username} is required.")
            .NotNull()
            .MaximumLength(50).WithMessage("{Username} must not exceed 50 characters.");
    }
}