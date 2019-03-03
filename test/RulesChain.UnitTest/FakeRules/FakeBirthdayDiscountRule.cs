using System;
using System.Linq;
using System.Text;
using RulesChain.UnitTest.FakeContexts;

namespace RulesChain.UnitTest.FakeRules
{
    public class FakeBirthdayDiscountRule : Rule<ApplyDiscountContext>
    {
        public FakeBirthdayDiscountRule(Rule<ApplyDiscountContext> next) : base(next)
        {}

        public override ApplyDiscountContext Run(ApplyDiscountContext context)
        {
            // Gets 10% of discount;
            var birthDayDiscount = context.Context.Items.Sum(i => i.Price * 0.1M);
            context = _next.Invoke(context);

            // Only apply birthday disccount if the discount applied by the other rules are smaller than this
            if (birthDayDiscount > context.DiscountApplied)
                context.DiscountApplied = birthDayDiscount;

            return context;
        }

        public override bool ShouldRun(ApplyDiscountContext context)
        {
            var dayAndMonth = context.Context.ClientBirthday.ToString("ddMM");
            var todayDayAndMonth = DateTime.Now.ToString("ddMM"); // TODO: Learn how to mock date.
            return dayAndMonth == todayDayAndMonth;
        }
    }
}
