using System.Linq;
using RulesChain.UnitTest.FakeContexts;

namespace RulesChain.UnitTest.FakeRules
{
    public class MyNameIsLuttiDiscount : Rule<ApplyDiscountContext>
    {
        public MyNameIsLuttiDiscount(Rule<ApplyDiscountContext> next) : base(next)
        {}

        public override ApplyDiscountContext Run(ApplyDiscountContext context)
        {
            // Gets 50% of discount;
            var myDiscount = context.Context.Items.Sum(i => i.Price * 0.5M);
            context = _next.Invoke(context) ?? context;

            // Only apply birthday disccount if the discount applied by the other rules are smaller than this
            if (myDiscount > context.DiscountApplied)
                context.DiscountApplied = myDiscount;

            return context;
        }

        public override bool ShouldRun(ApplyDiscountContext context)
        {
            return context.Context.CilentName.ToUpper().Contains("LUTTI");
        }
    }
}
