using System;
using System.Threading.Tasks;

namespace Ex3
{
    public interface ICouponProvider
    {
        Task<Coupon> Retrieve(Guid couponId);
    }
}