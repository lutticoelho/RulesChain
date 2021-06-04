using System.Threading.Tasks;

namespace RulesChain.Contracts
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TContext"></typeparam>
    public interface IRule<TContext>
    {
        /// <summary>
        /// Validates if the rules should be executed or not
        /// </summary>
        /// <returns><see cref="Task"/></returns>
        Task Invoke(TContext context);

        /// <summary>
        /// Executes the rule
        /// </summary>
        /// <returns><see cref="Task"/></returns>
        Task Run(TContext context);

        /// <summary>
        /// Validates if rule should run and Invokes Run or Next method according with ShouldRun result
        /// </summary>
        /// <param name="context">Rule Context</param>
        /// <returns><see cref="Task"/></returns>
        bool ShouldRun(TContext context);
    }
}