using System;
using System.Threading.Tasks;
using RulesChain.Contracts;
using RulesChain.UnitTest.FakeContexts;
using RulesChain.UnitTest.FakeDependencies;

namespace RulesChain.UnitTest.FakeRules
{
    public class IsValidCouponDiscount : Rule<ApplyDiscountContext>
    {
        private readonly IFakeRepository _repository;

        public IsValidCouponDiscount(RuleHandlerDelegate<ApplyDiscountContext> next, IFakeRepository repository) :
            base(next)
        {
            _repository = repository;
        }

        public override Task Run(ApplyDiscountContext context)
        {
            var (isValid, discount) = _repository.IsValidCouponCode(context.ShoppingCart.CouponCode);
            
            // Only apply birthday disccount if the discount applied by the other rules are smaller than this
            if (isValid && discount > context.DiscountApplied)
            {
                context.DiscountApplied = discount;
                context.Properties["discountType"] = "IsValidCouponDiscount";
            }

            return Next(context);
        }

        public override bool ShouldRun(ApplyDiscountContext context)
        {
            return !string.IsNullOrWhiteSpace(context.ShoppingCart?.CouponCode);
        }
    }
}
