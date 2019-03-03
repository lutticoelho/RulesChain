using RulesChain.Contracts;
using System.Collections.Concurrent;
using System.Collections.Generic;
using RulesChain.UnitTest.FakeContexts.Models;

namespace RulesChain.UnitTest.FakeContexts
{
    public class ApplyDiscountContext : IRuleContext<ShoppingCart>
    {
        public ApplyDiscountContext()
        {
        }

        public ApplyDiscountContext(ShoppingCart shoppingCart)
        {
            Properties = new ConcurrentDictionary<string, object>();
            Context = shoppingCart;
        }

        public IDictionary<string, object> Properties { get; }

        public ShoppingCart Context { get; set; }

        public decimal DiscountApplied { get; set; }
    }
}
