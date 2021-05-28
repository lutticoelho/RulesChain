using System;
using System.Collections.Generic;
using FluentAssertions;
using Moq;
using RulesChain.Contracts;
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

        [Fact(DisplayName = "Test single rule - ShouldRun")]
        public void TestSingleRule_ShouldRun()
        {
            // Arrage
            var rule = new FakeBirthdayDiscountRule(new EndOfChainRule<ApplyDiscountContext>()); // Creates a mocked rule
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

            var shouldRunResult = rule.ShouldRun(context);

            shouldRunResult.Should().BeFalse();
        }

        [Fact(DisplayName = "Test single rule - Run")]
        public void TestSingleRule_Run()
        {
            // Arrage
            var rule = new FakeBirthdayDiscountRule(new EndOfChainRule<ApplyDiscountContext>()); // Creates a mocked rule
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

            var result = rule.Run(context);

            result.DiscountApplied.Should().Be(100);
        }

        [Fact(DisplayName = "Test single rule - Invoke")]
        public void TestSingleRule_Invoke()
        {
            // Arrage
            var rule = new FakeBirthdayDiscountRule(new EndOfChainRule<ApplyDiscountContext>()); // Creates a mocked rule
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

            var result = rule.Invoke(context);

            result.DiscountApplied.Should().Be(0);
        }
    }
}
