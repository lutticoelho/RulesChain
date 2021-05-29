namespace RulesChain.Contracts
{
    /// <summary>
    /// Defines a class that provides the mechanisms to configure an application's rules pipeline execution.
    /// </summary>
    /// <typeparam name="TContext">The context shared by all rules in the chain</typeparam>
    public interface IRuleChain<TContext>
    {
        /// <summary>
        /// Adds a rule to the application's request chain.
        /// </summary>
        /// <typeparam name="TContext"><see cref="TContext"/></typeparam>
        /// <typeparam name="TRule"></typeparam>
        /// <returns>The <see cref="IRuleChain{TContext }"/>.</returns>
        IRuleChain<TContext> Use<TRule>();

        /// <summary>
        /// Builds the delegate used by this application to process rules executions.
        /// </summary>
        /// <typeparam name="TContext"><see cref="TContext"/></typeparam>
        /// <returns>The rules handling delegate.</returns>
        RuleHandlerDelegate<TContext> Build();
    }
}