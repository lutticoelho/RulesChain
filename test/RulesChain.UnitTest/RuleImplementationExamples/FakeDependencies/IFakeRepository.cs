using System;

namespace RulesChain.UnitTest.RuleImplementationExamples.FakeDependencies
{
    public interface IFakeRepository
    {
        Tuple<bool, decimal> IsValidCouponCode(string couponCode);
    }
}
