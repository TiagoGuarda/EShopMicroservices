using Ordering.Domain.Enums;

namespace Ordering.Application.Dtos;

public record OrderDTO(
    Guid Id,
    Guid CustomerId,
    string OrderName,
    AddressDTO ShippingAddress,
    AddressDTO BillingAddress,
    PaymentDTO Payment,
    OrderStatus Status,
    List<OrderItemDTO> OrderItems);