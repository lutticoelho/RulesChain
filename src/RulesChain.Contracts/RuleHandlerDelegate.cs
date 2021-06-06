using System.Threading.Tasks;

namespace RulesChain.Contracts
{
    /// <summary>
    /// A function that can process a the context dependent rule.
    /// </summary>
    /// <typeparam name="TContext">Type of the context</typeparam>
    /// <param name="context">Context shared by all rules in a chain</param>
    /// <returns>A task that represents the completion of rule processing</returns>
    public delegate Task RuleHandlerDelegate<TContext>(TContext context);
}
