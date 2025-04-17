using Discount.GRPC.Data;
using Discount.GRPC.Models;
using Grpc.Core;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace Discount.GRPC.Services;

public class DiscountService(DiscountContext dbContext, ILogger<DiscountService> logger) : DiscountProtoService.DiscountProtoServiceBase
{
    public override async Task<CouponModel> GetDiscount(GetDiscountRequest request, ServerCallContext context)
    {
        var coupon = await dbContext.Coupons.FirstOrDefaultAsync(x => x.ProductName == request.ProductName) ?? Coupon.NoDiscountCoupon;

        logger.LogInformation("Discount was retrieved for ProductName: {productName}, Amount: {amount}", coupon.ProductName, coupon.Amount);

        return coupon.Adapt<CouponModel>();
    }

    public override async Task<CouponModel> CreateDiscount(CreateDiscountRequest request, ServerCallContext context)
    {
        var coupon = request.Coupon.Adapt<Coupon>() ?? throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid request object."));
        
        dbContext.Coupons.Add(coupon);
        
        await dbContext.SaveChangesAsync();

        logger.LogInformation("Discount was successfully created. ProductName: {productName}, Amount: {amount}", coupon.ProductName, coupon.Amount);

        return coupon.Adapt<CouponModel>();
    }

    public override async Task<CouponModel> UpdateDiscount(UpdateDiscountRequest request, ServerCallContext context)
    {
        var coupon = request.Coupon.Adapt<Coupon>() ?? throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid request object."));

        dbContext.Coupons.Update(coupon);

        await dbContext.SaveChangesAsync();

        logger.LogInformation("Discount was successfully updated. ProductName: {productName}, Amount: {amount}", coupon.ProductName, coupon.Amount);

        return coupon.Adapt<CouponModel>();
    }

    public override async Task<DeleteDiscountResponse> DeleteDiscount(DeleteDiscountRequest request, ServerCallContext context)
    {
        var coupon = await dbContext.Coupons.FirstOrDefaultAsync(x => x.ProductName == request.ProductName) 
            ?? throw new RpcException(new Status(StatusCode.NotFound, $"Discount with ProductName: {request.ProductName} was not found."));

        dbContext.Coupons.Remove(coupon);
        await dbContext.SaveChangesAsync();

        logger.LogInformation("Discount was successfully deleted. ProductName: {productName}, Amount: {amount}", coupon.ProductName, coupon.Amount);

        return new() { Success = true };
    }
}
