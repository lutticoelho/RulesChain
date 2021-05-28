using System;
using System.Collections.Generic;

namespace RulesChain.UnitTest.RuleImplementationExamples.FakeContexts.Models
{
    public class ShoppingCart
    {
        public string CilentName { get; set; }
        public DateTime ClientBirthday { get; set; }
        public IEnumerable<ShopItem> Items { get; set; }
        public string CouponCode { get; set; }
    }
}
