using System.Threading.Tasks;
using Moq;
using Xunit;

namespace RulesChain.UnitTest
{
    public class RuleUnitTest
    {
        [Theory]
        [InlineData(true, 1, 1)]
        [InlineData(false, 1, 0)]
        public void Rule_Invoke_ShouldOnlyCallRun_When_ShouldRunReturns_True(bool shouldRunResult, int expectedShouldRunExecutions, int expectedRunExecutions)
        {
            // Arrange
            var rule = new Mock<Rule<object>>(MockBehavior.Loose, null) {CallBase = true};
            rule.Setup(_ => _.ShouldRun(It.IsAny<object>())).Returns(shouldRunResult);
            rule.Setup(_ => _.Run(It.IsAny<object>())).Returns(Task.CompletedTask);

            // Act
            rule.Object.Invoke(new object());

            // Assert
            rule.Verify(_ => _.ShouldRun(It.IsAny<object>()), Times.Exactly(expectedShouldRunExecutions));
            rule.Verify(_ => _.Run(It.IsAny<object>()), Times.Exactly(expectedRunExecutions));
        }
    }
}
