using EnterpriseApp.Application.Common;
using EnterpriseApp.Application.Common.Interfaces;
using EnterpriseApp.Application.Features.Orders.Dtos;
using EnterpriseApp.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EnterpriseApp.Application.Features.Orders.Queries;

/// <summary>
/// Query to get an order by ID.
/// </summary>
public record GetOrderQuery(Guid Id) : IRequest<Result<OrderDetailDto>>;

/// <summary>
/// Handler for GetOrderQuery.
/// </summary>
public class GetOrderQueryHandler : IRequestHandler<GetOrderQuery, Result<OrderDetailDto>>
{
    private readonly IRepository<Order> _orderRepository;

    public GetOrderQueryHandler(IRepository<Order> orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<Result<OrderDetailDto>> Handle(GetOrderQuery request, CancellationToken cancellationToken)
    {
        var order = await _orderRepository
            .AsNoTracking()
            .Include(o => o.Customer)
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.Id == request.Id, cancellationToken);

        if (order == null)
        {
            return Result.Failure<OrderDetailDto>("Order not found.");
        }

        var dto = new OrderDetailDto
        {
            Id = order.Id,
            OrderNumber = order.OrderNumber,
            CustomerId = order.CustomerId,
            CustomerName = order.Customer.CompanyName ?? $"{order.Customer.FirstName} {order.Customer.LastName}",
            CustomerEmail = order.Customer.Email,
            Status = order.Status.ToString(),
            PaymentStatus = order.PaymentStatus.ToString(),
            OrderDate = order.OrderDate,
            ExpectedDeliveryDate = order.ExpectedDeliveryDate,
            DeliveredAt = order.DeliveredAt,
            Subtotal = order.Subtotal,
            DiscountAmount = order.DiscountAmount,
            TaxAmount = order.TaxAmount,
            ShippingAmount = order.ShippingAmount,
            TotalAmount = order.TotalAmount,
            Currency = order.Currency,
            PaymentMethod = order.PaymentMethod,
            PaymentReference = order.PaymentReference,
            PaidAt = order.PaidAt,
            ShippingAddress = new AddressDto
            {
                Street = order.ShippingStreet,
                City = order.ShippingCity,
                State = order.ShippingState,
                PostalCode = order.ShippingPostalCode,
                Country = order.ShippingCountry
            },
            BillingAddress = new AddressDto
            {
                Street = order.BillingStreet,
                City = order.BillingCity,
                State = order.BillingState,
                PostalCode = order.BillingPostalCode,
                Country = order.BillingCountry
            },
            Notes = order.Notes,
            CustomerNotes = order.CustomerNotes,
            TrackingNumber = order.TrackingNumber,
            ShippingCarrier = order.ShippingCarrier,
            CreatedAt = order.CreatedAt,
            ModifiedAt = order.ModifiedAt,
            Items = order.Items
                .Select(i => new OrderItemDto
                {
                    Id = i.Id,
                    ProductId = i.ProductId,
                    ProductVariantId = i.ProductVariantId,
                    ProductName = i.ProductName,
                    ProductSku = i.ProductSku,
                    VariantName = i.VariantName,
                    UnitPrice = i.UnitPrice,
                    Quantity = i.Quantity,
                    UnitDiscount = i.UnitDiscount,
                    TaxRate = i.TaxRate,
                    LineTotal = i.LineTotal,
                    TaxAmount = i.TaxAmount,
                    LineTotalWithTax = i.LineTotalWithTax,
                    Notes = i.Notes
                })
                .ToList()
        };

        return Result.Success(dto);
    }
}
