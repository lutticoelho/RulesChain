using RulesChain.Contracts;
using System.Collections.Generic;
using RulesChain.UnitTest.RuleImplementationExamples.FakeContexts.Models;

namespace RulesChain.UnitTest.RuleImplementationExamples.FakeContexts
{
    public class ApplyDiscountContext : IRuleContext<ShoppingCart>
    {
        public IDictionary<string, object> Properties { get; } = new Dictionary<string, object>();

        public ShoppingCart Context { get; set; }

        public decimal DiscountApplied { get; set; }
    }
}
