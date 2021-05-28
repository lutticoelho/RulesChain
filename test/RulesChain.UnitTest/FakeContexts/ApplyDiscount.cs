using RulesChain.Contracts;
using System.Collections.Generic;
using RulesChain.UnitTest.FakeContexts.Models;

namespace RulesChain.UnitTest.FakeContexts
{
    public class ApplyDiscountContext : IRuleContext<ShoppingCart>
    {
        public IDictionary<string, object> Properties { get; }

        public ShoppingCart ShoppingCart { get; set; }

        public decimal DiscountApplied { get; set; }
    }
}
