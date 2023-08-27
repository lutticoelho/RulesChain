using System;
using System.Collections.Generic;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using RulesChain;
using RulesChain.UnitTest.RuleImplementationExamples.FakeContexts;
using RulesChain.UnitTest.RuleImplementationExamples.FakeContexts.Models;
using RulesChain.UnitTest.RuleImplementationExamples.FakeDependencies;
using RulesChain.UnitTest.RuleImplementationExamples.FakeRules;
using Xunit;

namespace RulesChain.UnitTest.RuleImplementationExamples
{
    public class RulesChainUnitTest
    {
        [Theory(DisplayName = "Should Apply higher discount")]
        [InlineData(100, 900, 0.1, "Lutti Coelho", 500)]
        [InlineData(100, 900, 600, "Lutti Coelho", 600)]
        [InlineData(100, 900, 0.1, "John Doe", 100)]
        [InlineData(100, 900, 0, "John Doe", 100)]
        public void ShouldApplyHigherDiscount(decimal priceItemOne, decimal priceItemTwo, decimal couponDiscount, string clientName, decimal expectedDiscount)
        {
            // Arrange
            var fakeRepository = new Mock<IFakeRepository>();
            fakeRepository.Setup(_ => _.IsValidCouponCode(It.IsAny<string>()))
                .Returns(new Tuple<bool, decimal>(true, couponDiscount));

            var services = new ServiceCollection()
                .AddSingleton(fakeRepository.Object)
                .BuildServiceProvider();

            var chain = new RuleChain<ApplyDiscountContext>(services)
                .Use<FakeBirthdayDiscountRule>()
                .Use<MyNameIsLuttiDiscount>()
                .Use<IsValidCouponDiscount>()
                .Build();

            var context = new ApplyDiscountContext
            {
                Context = new ShoppingCart
                {
                    ClientName = clientName,
                    Items = new List<ShopItem>()
                    {
                        new ShopItem {Name = "Item 1", Price = priceItemOne},
                        new ShopItem {Name = "Item 2", Price = priceItemTwo}
                    },
                    ClientBirthday = DateTime.UtcNow,
                    CouponCode = "TestCoupon005"
                }
            };

            // Act
            chain(context);

            // Assert
            context.DiscountApplied.Should().Be(expectedDiscount);
        }

        [Fact(DisplayName = "RuleChain should only be built once")]
        public void RuleChainShouldOnlyBeBuiltOnce()
        {
            // Arrange
            var fakeRepository = new Mock<IFakeRepository>();
            fakeRepository.Setup(_ => _.IsValidCouponCode(It.IsAny<string>()))
                .Returns(new Tuple<bool, decimal>(true, 0.1M));

            var services = new ServiceCollection()
                .AddSingleton(fakeRepository.Object)
                .BuildServiceProvider();

            var chain = new RuleChain<ApplyDiscountContext>(services)
                .Use<FakeBirthdayDiscountRule>()
                .Use<IsValidCouponDiscount>();

            var builtChain = chain.Build();

            // Act & Assert
            var ex = Assert.Throws<InvalidOperationException>(() => chain.Build()); //Force error calling .Build twice
            Assert.Equal("Chain can only be built once", ex.Message);
        }

        [Fact(DisplayName = "RuleChain should use minimal constructor possible for each Rule")]
        public void RuleChainShouldUseMinimalConstructorPossible()
        {
            // Arrange
            var services = new ServiceCollection().BuildServiceProvider();

            var chain = new RuleChain<ApplyDiscountContext>(services)
                .Use<MyNameIsLuttiDiscount>()
                .Build();

            var context = new ApplyDiscountContext
            {
                Context = new ShoppingCart
                {
                    ClientName = "Lutti Coelho",
                    Items = new List<ShopItem>()
                    {
                        new ShopItem {Name = "Item 1", Price = 100M},
                        new ShopItem {Name = "Item 2", Price = 900M}
                    },
                    ClientBirthday = DateTime.UtcNow,
                    CouponCode = "TestCoupon005"
                }
            };

            // Act
            chain(context);

            // Assert
            context.DiscountApplied.Should().Be(500);
            context.Properties["discountType"].Should().Be("MyNameIsLuttiDiscount");
            context.Properties["year"].Should().Be(0); // Validates that was used the constructor with less parameters
        }

        [Theory(DisplayName = "RuleChain should not accept a rule that don't implements IRule interface")]
        [InlineData(typeof(InvalidRule), "Missing invoke method")]
        [InlineData(typeof(InvalidRule2), "invalid invoke return type")]
        [InlineData(typeof(InvalidRule3), "invalid invoke parameter type")]
        public void RuleChainShouldNotAcceptAruleThatDontImplementsIRuleInterface_MustHaveInvokeMethod(Type t, string expectedErrorMessage)
        {
            // Arrange
            var services = new ServiceCollection().BuildServiceProvider();

            var chain = new RuleChain<ApplyDiscountContext>(services);

            // Act & Assert
            var ex = Assert.Throws<InvalidOperationException>(() => chain.GetValidInvokeMethodInfo(t));
            Assert.Equal(expectedErrorMessage, ex.Message);
        }
    }
}
