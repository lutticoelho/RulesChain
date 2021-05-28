using System;
using System.Collections.Generic;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Moq;
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
    }
}
