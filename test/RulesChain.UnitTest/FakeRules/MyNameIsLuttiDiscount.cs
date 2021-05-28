using System.Linq;
using System.Threading.Tasks;
using RulesChain.Contracts;
using RulesChain.UnitTest.FakeContexts;

namespace RulesChain.UnitTest.FakeRules
{
    public class MyNameIsLuttiDiscount : Rule<ApplyDiscountContext>
    {
        public MyNameIsLuttiDiscount(RuleHandlerDelegate<ApplyDiscountContext> next) : base(next)
        {}

        public override async Task Run(ApplyDiscountContext context)
        {
            // Gets 50% of discount;
            var myDiscount = context.ShoppingCart.Items.Sum(i => i.Price * 0.5M);
            await Next.Invoke(context);

            // Only apply birthday disccount if the discount applied by the other rules are smaller than this
            if (myDiscount > context.DiscountApplied)
            {
                context.DiscountApplied = myDiscount;
                context.Properties["discountType"] = "MyNameIsLuttiDiscount";
            }
        }

        public override bool ShouldRun(ApplyDiscountContext context)
        {
            return context.ShoppingCart.CilentName.ToUpper().Contains("LUTTI");
        }
    }
}
