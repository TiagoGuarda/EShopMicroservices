using FluentValidation;

namespace Ordering.Application.Orders.Commands.CreateOrder;

public record CreateOrderCommand(OrderDTO Order) : ICommand<CreateOrderResult>;
public record CreateOrderResult(Guid Id);

public class CreateOrderCommandValidator : AbstractValidator<CreateOrderCommand>
{
    public CreateOrderCommandValidator()
    {
        RuleFor(x => x.Order.OrderName).NotEmpty().WithMessage("OrderName is required.");
        RuleFor(x => x.Order.CustomerId).NotEmpty().WithMessage("CustomerId is required.");
        RuleFor(x => x.Order.OrderItems).NotEmpty().WithMessage("OrderItems should not be empty.");
    }
}

public class CreateOrderHandler(IApplicationDbContext dbContext) : ICommandHandler<CreateOrderCommand, CreateOrderResult>
{
    public async Task<CreateOrderResult> Handle(CreateOrderCommand command, CancellationToken cancellationToken)
    {
        var order = CreateNewOrder(command.Order);

        dbContext.Orders.Add(order);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new(order.Id.Value);
    }

    private static Order CreateNewOrder(OrderDTO orderDTO)
    {
        var shippingAddress = Address.Of(orderDTO.ShippingAddress.FirstName, orderDTO.ShippingAddress.LastName,
            orderDTO.ShippingAddress.EmailAddress, orderDTO.ShippingAddress.AddressLine, orderDTO.ShippingAddress.Country,
            orderDTO.ShippingAddress.State, orderDTO.ShippingAddress.ZipCode);

        var billingAddress = Address.Of(orderDTO.BillingAddress.FirstName, orderDTO.BillingAddress.LastName, 
            orderDTO.BillingAddress.EmailAddress, orderDTO.BillingAddress.AddressLine, orderDTO.BillingAddress.Country, 
            orderDTO.BillingAddress.State, orderDTO.BillingAddress.ZipCode);

        var payment = Payment.Of(orderDTO.Payment.CardName, orderDTO.Payment.CardNumber, orderDTO.Payment.Expiration, 
            orderDTO.Payment.Cvv, orderDTO.Payment.PaymentMethod);

        var newOrder = Order.Create(
            id: OrderId.Of(Guid.NewGuid()),
            customerId: CustomerId.Of(orderDTO.CustomerId),
            orderName: OrderName.Of(orderDTO.OrderName),
            shippingAddress: shippingAddress,
            billingAddress: billingAddress,
            payment: payment);

        foreach (var orderItemDTO in orderDTO.OrderItems)
            newOrder.Add(
                productId: ProductId.Of(orderItemDTO.ProductId),
                quantity: orderItemDTO.Quantity,
                price: orderItemDTO.Price);

        return newOrder;
    }
}
