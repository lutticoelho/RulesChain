using System;
using System.Collections.Generic;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using RulesChain.UnitTest.FakeContexts;
using RulesChain.UnitTest.FakeContexts.Models;
using RulesChain.UnitTest.FakeDependencies;
using RulesChain.UnitTest.FakeRules;
using Xunit;

namespace RulesChain.UnitTest
{
    public class RulesChainUnitTest
    {
        [Theory(DisplayName = "Should Apply higher discount")]
        [InlineData(100, 900, 0.1, 500)]
        [InlineData(100, 900, 600, 500)]
        public void ShouldApplyHigherDiscount(decimal priceItemOne, decimal priceItemTwo, decimal couponDiscount, decimal expectedDiscount)
        {
            // Arrage
            var fakeRepository = new Mock<IFakeRepository>();
            fakeRepository.Setup(_ => _.IsValidCouponCode(It.IsAny<string>()))
                .Returns(new Tuple<bool, decimal>(true, couponDiscount));

            var services = new ServiceCollection()
                .AddSingleton(fakeRepository.Object)
                .BuildServiceProvider();

            var chain = new RuleChain<ApplyDiscountContext>(services)
                .Use<FakeBirthdayDiscountRule>()      //0.1
                .Use<MyNameIsLuttiDiscount>()              //0.5
                .Use<IsValidCouponDiscount>()    //Mock Value
                .Build();

            var context = new ApplyDiscountContext
            {
                ShoppingCart = new ShoppingCart
                {
                    CilentName = "Lutti Coelho",
                    Items = new List<ShopItem>()
                    {
                        new ShopItem() {Name = "Item 1", Price = priceItemOne},
                        new ShopItem() {Name = "Item 2", Price = priceItemTwo}
                    },
                    ClientBirthday = DateTime.UtcNow
                }
            };

            // Act
            chain.Invoke(context);

            // Assert
            context.DiscountApplied.Should().Be(expectedDiscount);
        }
    }
}
