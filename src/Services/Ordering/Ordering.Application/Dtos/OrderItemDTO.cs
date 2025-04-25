namespace Ordering.Application.Dtos;

public record OrderItemDTO (
    Guid OrderId, 
    Guid ProductId, 
    int Quantity, 
    decimal Price);
