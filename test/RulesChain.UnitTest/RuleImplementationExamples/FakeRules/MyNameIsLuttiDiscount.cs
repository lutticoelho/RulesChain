using System.Linq;
using System.Threading.Tasks;
using RulesChain.Contracts;
using RulesChain.UnitTest.RuleImplementationExamples.FakeContexts;

namespace RulesChain.UnitTest.RuleImplementationExamples.FakeRules
{
    public class MyNameIsLuttiDiscount : Rule<ApplyDiscountContext>
    {
        private readonly int _year;
        public MyNameIsLuttiDiscount(RuleHandlerDelegate<ApplyDiscountContext> next) : base(next)
        {
            _year = -1;
        }

        public MyNameIsLuttiDiscount(RuleHandlerDelegate<ApplyDiscountContext> next, int year) : base(next)
        {
            _year = year;
        }

        public override async Task Run(ApplyDiscountContext context)
        {
            // Gets 50% of discount;
            var myDiscount = context.Context.Items.Sum(i => i.Price * 0.5M);
            await Next(context);

            // Only apply birthday discount if the discount applied by the other rules are smaller than this
            if (myDiscount > context.DiscountApplied)
            {
                context.DiscountApplied = myDiscount;
                context.Properties["discountType"] = "MyNameIsLuttiDiscount";
                context.Properties["year"] = _year;
            }
        }

        public override bool ShouldRun(ApplyDiscountContext context)
        {
            return context.Context.ClientName.ToUpper().Contains("LUTTI");
        }
    }
}
