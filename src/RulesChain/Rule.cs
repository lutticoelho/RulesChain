using System.Threading.Tasks;
using RulesChain.Contracts;

namespace RulesChain
{
    /// <inheritdoc cref="IRule{TContext}"/>
    public abstract class Rule<TContext> : IRule<TContext>
    {
        protected readonly RuleHandlerDelegate<TContext> Next;

        protected Rule(RuleHandlerDelegate<TContext> next)
        {
            Next = next;
        }

        /// <inheritdoc cref="IRule{TContext}.ShouldRun"/>
        public abstract bool ShouldRun(TContext context);

        /// <inheritdoc cref="IRule{TContext}.Run"/>
        public abstract Task Run(TContext context);

        /// <inheritdoc cref="IRule{TContext}.Invoke"/>
        public Task Invoke(TContext context)
        {
            if(ShouldRun(context))
                return Run(context);
               
            return Next != null 
                    ? Next(context) 
                    : Task.CompletedTask;
        }
    }
}
