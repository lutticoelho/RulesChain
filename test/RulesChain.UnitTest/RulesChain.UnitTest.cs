using System;
using System.Collections.Generic;
using FluentAssertions;
using RulesChain.UnitTest.FakeContexts;
using RulesChain.UnitTest.FakeContexts.Models;
using RulesChain.UnitTest.FakeRules;
using Xunit;

namespace RulesChain.UnitTest
{
    public class RulesChainUnitTest
    {
        [Fact(DisplayName = "Should Apply higher discount")]
        public void ShouldApplyHigherDiscount()
        {
            // Arrage
            var chain = new RuleChain<ApplyDiscountContext>()
                .Use<FakeBirthdayDiscountRule>()
                .Use<MyNameIsLuttiDiscount>()
                .Build();

            var context = new ApplyDiscountContext
            {
                Context = new ShoppingCart
                {
                    CilentName = "Lutti Coelho",
                    Items = new List<ShopItem>()
                {
                    {new ShopItem() {Name = "Item 1", Price = 100}},
                    {new ShopItem() {Name = "Item 2", Price = 900}}
                },
                    ClientBirthday = new DateTime(1986, 8, 16)
                }
            };

            // Act
            context = chain.Invoke(context);

            // Assert
            context.DiscountApplied.Should().Be(500);
        }
    }
}
