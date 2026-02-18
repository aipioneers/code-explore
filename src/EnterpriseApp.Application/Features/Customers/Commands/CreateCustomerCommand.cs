using EnterpriseApp.Application.Common;
using EnterpriseApp.Application.Common.Interfaces;
using EnterpriseApp.Application.Features.Customers.Dtos;
using EnterpriseApp.Domain.Entities;
using FluentValidation;
using MediatR;

namespace EnterpriseApp.Application.Features.Customers.Commands;

/// <summary>
/// Command to create a new customer.
/// </summary>
public record CreateCustomerCommand(
    string? CompanyName,
    string FirstName,
    string LastName,
    string Email,
    string? Phone,
    string? Mobile,
    string? TaxId,
    string? Notes,
    decimal? CreditLimit,
    int PaymentTermsDays
) : IRequest<Result<CustomerDto>>;

/// <summary>
/// Validator for CreateCustomerCommand.
/// </summary>
public class CreateCustomerCommandValidator : AbstractValidator<CreateCustomerCommand>
{
    public CreateCustomerCommandValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First name is required.")
            .MaximumLength(100).WithMessage("First name must not exceed 100 characters.");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Last name is required.")
            .MaximumLength(100).WithMessage("Last name must not exceed 100 characters.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("A valid email address is required.")
            .MaximumLength(256).WithMessage("Email must not exceed 256 characters.");

        RuleFor(x => x.CompanyName)
            .MaximumLength(200).WithMessage("Company name must not exceed 200 characters.")
            .When(x => !string.IsNullOrEmpty(x.CompanyName));

        RuleFor(x => x.Phone)
            .MaximumLength(30).WithMessage("Phone must not exceed 30 characters.")
            .When(x => !string.IsNullOrEmpty(x.Phone));

        RuleFor(x => x.CreditLimit)
            .GreaterThanOrEqualTo(0).WithMessage("Credit limit must be positive.")
            .When(x => x.CreditLimit.HasValue);

        RuleFor(x => x.PaymentTermsDays)
            .InclusiveBetween(0, 365).WithMessage("Payment terms must be between 0 and 365 days.");
    }
}

/// <summary>
/// Handler for CreateCustomerCommand.
/// </summary>
public class CreateCustomerCommandHandler : IRequestHandler<CreateCustomerCommand, Result<CustomerDto>>
{
    private readonly IRepository<Customer> _customerRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateCustomerCommandHandler(
        IRepository<Customer> customerRepository,
        IUnitOfWork unitOfWork)
    {
        _customerRepository = customerRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<CustomerDto>> Handle(CreateCustomerCommand request, CancellationToken cancellationToken)
    {
        // Check for duplicate email
        var existingCustomer = await _customerRepository.FirstOrDefaultAsync(
            c => c.Email.ToLower() == request.Email.ToLower(),
            cancellationToken);

        if (existingCustomer != null)
        {
            return Result.Failure<CustomerDto>("A customer with this email already exists.");
        }

        // Generate customer number
        var count = await _customerRepository.CountAsync(cancellationToken: cancellationToken);
        var customerNumber = $"CUST-{(count + 1).ToString().PadLeft(5, '0')}";

        // Create customer
        var customer = Customer.Create(
            customerNumber,
            request.FirstName,
            request.LastName,
            request.Email,
            request.CompanyName,
            request.Phone);

        customer.Update(
            request.FirstName,
            request.LastName,
            request.Email,
            request.CompanyName,
            request.Phone,
            request.Mobile,
            null,
            null,
            request.TaxId,
            request.Notes,
            request.CreditLimit,
            request.PaymentTermsDays);

        await _customerRepository.AddAsync(customer, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(new CustomerDto
        {
            Id = customer.Id,
            CustomerNumber = customer.CustomerNumber,
            DisplayName = customer.DisplayName,
            CompanyName = customer.CompanyName,
            FullName = customer.FullName,
            Email = customer.Email,
            Phone = customer.Phone,
            Status = customer.Status.ToString(),
            CreatedAt = customer.CreatedAt
        });
    }
}
