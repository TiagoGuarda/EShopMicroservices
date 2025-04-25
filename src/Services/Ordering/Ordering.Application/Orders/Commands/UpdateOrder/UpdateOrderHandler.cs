using FluentValidation;

namespace Ordering.Application.Orders.Commands.UpdateOrder;

public record UpdateOrderCommand(OrderDTO Order) : ICommand<UpdateOrderResult>;
public record UpdateOrderResult(bool IsSuccess);

public class UpdateOrderCommandValidator : AbstractValidator<UpdateOrderCommand>
{
    public UpdateOrderCommandValidator()
    {
        RuleFor(x => x.Order.Id).NotEmpty().WithMessage("Id is required.");
        RuleFor(x => x.Order.OrderName).NotEmpty().WithMessage("OrderName is required.");
        RuleFor(x => x.Order.CustomerId).NotEmpty().WithMessage("CustomerId is required.");
    }
}

public class UpdateOrderHandler(IApplicationDbContext dbContext) : ICommandHandler<UpdateOrderCommand, UpdateOrderResult>
{
    public async Task<UpdateOrderResult> Handle(UpdateOrderCommand command, CancellationToken cancellationToken)
    {
        var orderId = OrderId.Of(command.Order.Id);
        var order = await dbContext.Orders.FindAsync([orderId], cancellationToken);
        
        if (order is null) throw new OrderNotFoundException(command.Order.Id);

        UpdateOrderWithNewValues(order, command.Order);

        dbContext.Orders.Update(order);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new(true);
    }

    private static void UpdateOrderWithNewValues(Order order, OrderDTO orderDTO)
    {
        var updatedShippingAddress = Address.Of(orderDTO.ShippingAddress.FirstName, orderDTO.ShippingAddress.LastName,
            orderDTO.ShippingAddress.EmailAddress, orderDTO.ShippingAddress.AddressLine, orderDTO.ShippingAddress.Country,
            orderDTO.ShippingAddress.State, orderDTO.ShippingAddress.ZipCode);
        
        var updatedBillingAddress = Address.Of(orderDTO.BillingAddress.FirstName, orderDTO.BillingAddress.LastName,
            orderDTO.BillingAddress.EmailAddress, orderDTO.BillingAddress.AddressLine, orderDTO.BillingAddress.Country,
            orderDTO.BillingAddress.State, orderDTO.BillingAddress.ZipCode);
        
        var updatedPayment = Payment.Of(orderDTO.Payment.CardName, orderDTO.Payment.CardNumber, orderDTO.Payment.Expiration,
            orderDTO.Payment.Cvv, orderDTO.Payment.PaymentMethod);
        
        order.Update(
            orderName: OrderName.Of(orderDTO.OrderName),
            shippingAddress: updatedShippingAddress,
            billingAddress: updatedBillingAddress,
            payment: updatedPayment,
            status: orderDTO.Status);
    }
}