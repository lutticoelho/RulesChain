using System.Threading.Tasks;

namespace RulesChain.Contracts
{
    /// <summary>
    /// A function that can process a <see cref="TContext"/> dependent rule.
    /// </summary>
    /// <typeparam name="TContext"><see cref="TContext"/></typeparam>
    /// <param name="context"></param>
    /// <returns>A task that represents the completion of rule processing</returns>
    public delegate Task RuleHandlerDelegate<TContext>(TContext context);
}
