
namespace RulesChain.Contracts
{
    /// <summary>
    /// Defines a class that provides the mechanisms to configure an application's rules pipeline execution.
    /// </summary>
    /// <typeparam name="T">The context shared by all rules in the chain</typeparam>
    public interface IRuleChain<T>
    {

        /// <summary>
        /// Adds a rule to the application's request chain.
        /// </summary>
        /// <returns>The <see cref="IRuleChain{T}"/>.</returns>
        IRuleChain<T> Use<TRule>();

        /// <summary>
        /// Builds the delegate used by this application to process rules executions.
        /// </summary>
        /// <returns>The rules handling delegate.</returns>
        IRule<T> Build();
    }
}