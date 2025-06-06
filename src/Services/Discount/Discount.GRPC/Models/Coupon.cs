﻿namespace Discount.GRPC.Models;

public class Coupon
{
    public int Id { get; set; }
    public string ProductName { get; set; } = default!;
    public string Description { get; set; } = default!;
    public int Amount { get; set; }

    public static Coupon NoDiscountCoupon => new()
    {
        Id = 0,
        ProductName = "No Discount",
        Description = "No Discount",
        Amount = 0
    };
}
