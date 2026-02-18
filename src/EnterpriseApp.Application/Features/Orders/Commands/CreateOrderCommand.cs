using EnterpriseApp.Application.Common;
using EnterpriseApp.Application.Common.Interfaces;
using EnterpriseApp.Application.Features.Orders.Dtos;
using EnterpriseApp.Domain.Entities;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EnterpriseApp.Application.Features.Orders.Commands;

/// <summary>
/// Command to create a new order.
/// </summary>
public record CreateOrderCommand(
    Guid CustomerId,
    List<CreateOrderItemRequest> Items,
    AddressDto ShippingAddress,
    AddressDto BillingAddress,
    string? CustomerNotes,
    decimal ShippingAmount,
    string? ShippingCarrier
) : IRequest<Result<OrderDto>>;

/// <summary>
/// Validator for CreateOrderCommand.
/// </summary>
public class CreateOrderCommandValidator : AbstractValidator<CreateOrderCommand>
{
    public CreateOrderCommandValidator()
    {
        RuleFor(x => x.CustomerId)
            .NotEmpty().WithMessage("Customer is required.");

        RuleFor(x => x.Items)
            .NotEmpty().WithMessage("At least one item is required.");

        RuleForEach(x => x.Items).ChildRules(item =>
        {
            item.RuleFor(i => i.ProductId)
                .NotEmpty().WithMessage("Product is required.");
            item.RuleFor(i => i.Quantity)
                .GreaterThan(0).WithMessage("Quantity must be positive.");
        });

        RuleFor(x => x.ShippingAddress.Street)
            .NotEmpty().WithMessage("Shipping street is required.");
        RuleFor(x => x.ShippingAddress.City)
            .NotEmpty().WithMessage("Shipping city is required.");
        RuleFor(x => x.ShippingAddress.PostalCode)
            .NotEmpty().WithMessage("Shipping postal code is required.");
        RuleFor(x => x.ShippingAddress.Country)
            .NotEmpty().WithMessage("Shipping country is required.");

        RuleFor(x => x.BillingAddress.Street)
            .NotEmpty().WithMessage("Billing street is required.");
        RuleFor(x => x.BillingAddress.City)
            .NotEmpty().WithMessage("Billing city is required.");
        RuleFor(x => x.BillingAddress.PostalCode)
            .NotEmpty().WithMessage("Billing postal code is required.");
        RuleFor(x => x.BillingAddress.Country)
            .NotEmpty().WithMessage("Billing country is required.");

        RuleFor(x => x.ShippingAmount)
            .GreaterThanOrEqualTo(0).WithMessage("Shipping amount must be non-negative.");
    }
}

/// <summary>
/// Handler for CreateOrderCommand.
/// </summary>
public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, Result<OrderDto>>
{
    private readonly IRepository<Order> _orderRepository;
    private readonly IRepository<Customer> _customerRepository;
    private readonly IRepository<Product> _productRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateOrderCommandHandler(
        IRepository<Order> orderRepository,
        IRepository<Customer> customerRepository,
        IRepository<Product> productRepository,
        IUnitOfWork unitOfWork)
    {
        _orderRepository = orderRepository;
        _customerRepository = customerRepository;
        _productRepository = productRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<OrderDto>> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        // Verify customer exists
        var customer = await _customerRepository.GetByIdAsync(request.CustomerId, cancellationToken);
        if (customer == null)
        {
            return Result.Failure<OrderDto>("Customer not found.");
        }

        // Generate order number
        var orderCount = await _orderRepository.CountAsync(cancellationToken: cancellationToken);
        var orderNumber = $"ORD-{DateTime.UtcNow:yyyyMMdd}-{(orderCount + 1).ToString().PadLeft(5, '0')}";

        // Create order
        var order = Order.Create(orderNumber, request.CustomerId);

        // Set addresses
        order.SetShippingAddress(
            request.ShippingAddress.Street,
            request.ShippingAddress.City,
            request.ShippingAddress.State,
            request.ShippingAddress.PostalCode,
            request.ShippingAddress.Country);

        order.SetBillingAddress(
            request.BillingAddress.Street,
            request.BillingAddress.City,
            request.BillingAddress.State,
            request.BillingAddress.PostalCode,
            request.BillingAddress.Country);

        // Set shipping
        order.SetShipping(request.ShippingAmount, request.ShippingCarrier);

        // Add items
        foreach (var itemRequest in request.Items)
        {
            var product = await _productRepository
                .AsNoTracking()
                .Include(p => p.Variants)
                .FirstOrDefaultAsync(p => p.Id == itemRequest.ProductId, cancellationToken);

            if (product == null)
            {
                return Result.Failure<OrderDto>($"Product not found: {itemRequest.ProductId}");
            }

            var unitPrice = product.BasePrice;
            string? variantName = null;

            if (itemRequest.ProductVariantId.HasValue)
            {
                var variant = product.Variants.FirstOrDefault(v => v.Id == itemRequest.ProductVariantId);
                if (variant == null)
                {
                    return Result.Failure<OrderDto>($"Product variant not found: {itemRequest.ProductVariantId}");
                }
                unitPrice += variant.PriceAdjustment;
                variantName = variant.Name;
            }

            order.AddItem(
                product.Id,
                product.Name,
                product.Sku,
                unitPrice,
                itemRequest.Quantity,
                product.TaxRate,
                itemRequest.ProductVariantId,
                variantName);
        }

        await _orderRepository.AddAsync(order, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(new OrderDto
        {
            Id = order.Id,
            OrderNumber = order.OrderNumber,
            CustomerId = order.CustomerId,
            CustomerName = customer.DisplayName,
            Status = order.Status.ToString(),
            PaymentStatus = order.PaymentStatus.ToString(),
            OrderDate = order.OrderDate,
            Subtotal = order.Subtotal,
            TotalAmount = order.TotalAmount,
            ItemCount = order.Items.Count,
            CreatedAt = order.CreatedAt
        });
    }
}
