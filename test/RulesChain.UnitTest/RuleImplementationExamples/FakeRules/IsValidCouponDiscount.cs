using System;
using System.Linq;
using System.Threading.Tasks;
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
            var (isValid, discount) = _repository.IsValidCouponCode(context.Context.CouponCode);

            // Only apply birthday discount if the discount applied by the other rules are smaller than this
            if (isValid && discount > context.DiscountApplied)
            {
                context.DiscountApplied = discount;
                context.Properties["discountType"] = "IsValidCouponDiscount";
                context.Properties["discountAppliedToProducts"] =  context.Context.Items.Aggregate("", (result, next) => result += next.Name);
            }

            return Next(context);
        }

        public override bool ShouldRun(ApplyDiscountContext context)
        {
            return !string.IsNullOrWhiteSpace(context.Context?.CouponCode) && context.Context?.Items.Any() == true;
        }
    }
}
