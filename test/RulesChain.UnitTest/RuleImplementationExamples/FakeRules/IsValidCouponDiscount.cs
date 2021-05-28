using System;
using System.Linq;
using System.Threading.Tasks;
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
using RulesChain.Contracts;
using RulesChain.UnitTest.RuleImplementationExamples.FakeContexts;
using RulesChain.UnitTest.RuleImplementationExamples.FakeDependencies;

namespace RulesChain.UnitTest.RuleImplementationExamples.FakeRules
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
                context.Properties["discountAppliedToProducts"] =  context.ShoppingCart.Items.Aggregate("", (result, next) => result += next.Name);
            }

            return Next(context);
        }

        public override bool ShouldRun(ApplyDiscountContext context)
        {
            return !string.IsNullOrWhiteSpace(context.ShoppingCart?.CouponCode) && context.ShoppingCart?.Items.Any() == true;
        }
    }
}
