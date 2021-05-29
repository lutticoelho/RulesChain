using System;
using System.Linq;
using System.Threading.Tasks;
using RulesChain.Contracts;
using RulesChain.UnitTest.RuleImplementationExamples.FakeContexts;

namespace RulesChain.UnitTest.RuleImplementationExamples.FakeRules
{
    public class FakeBirthdayDiscountRule : Rule<ApplyDiscountContext>
    {
        public FakeBirthdayDiscountRule(RuleHandlerDelegate<ApplyDiscountContext> next) : base(next)
        { }

        public override async Task Run(ApplyDiscountContext context)
        {
            // Gets 10% of discount;
            var birthDayDiscount = context.Context.Items.Sum(i => i.Price * 0.1M);
            await Next(context);

            // Only apply birthday discount if the discount applied by the other rules are smaller than this
            if (birthDayDiscount > context.DiscountApplied)
            {
                context.DiscountApplied = birthDayDiscount;
                context.Properties["discountType"] = "FakeBirthdayDiscountRule";
            }
        }

        public override bool ShouldRun(ApplyDiscountContext context)
        {
            var dayAndMonth = context.Context.ClientBirthday.ToString("ddMM");
            var todayDayAndMonth = DateTime.UtcNow.ToString("ddMM");
            return dayAndMonth == todayDayAndMonth;
        }
    }
}
