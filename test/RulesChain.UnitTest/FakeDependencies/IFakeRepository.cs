using System;

namespace RulesChain.UnitTest.FakeDependencies
{
    public interface IFakeRepository
    {
        Tuple<bool, decimal> IsValidCouponCode(string couponCode);
    }
}
