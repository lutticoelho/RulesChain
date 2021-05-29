using System.Threading.Tasks;
using RulesChain.Contracts;

namespace RulesChain
{
    public abstract class Rule<TContext> : IRule<TContext>
    {
        protected readonly RuleHandlerDelegate<TContext> Next;

        protected Rule(RuleHandlerDelegate<TContext> next)
        {
            Next = next;
        }

        /// <summary>
        /// Validates if the rules should be executed or not
        /// </summary>
        /// <returns></returns>
        public abstract bool ShouldRun(TContext context);

        /// <summary>
        /// Executes the rule
        /// </summary>
        /// <returns></returns>
        public abstract Task Run(TContext context);

        public virtual Task Invoke(TContext context)
        {
            if(ShouldRun(context))
                return Run(context);
               
            return Next != null 
                    ? Next(context) 
                    : Task.CompletedTask;
        }
    }

}
