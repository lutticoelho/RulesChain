using System;
using RulesChain.Contracts;

namespace RulesChain
{
    public abstract class Rule<T> : IRule<T>
    {
        protected readonly Rule<T> _next;

        protected Rule(Rule<T> next)
        {
            _next = next;
        }

        /// <summary>
        /// Valides if the rules should be executed or not
        /// </summary>
        /// <returns></returns>
        public abstract bool ShouldRun(T context);

        /// <summary>
        /// Executes the rule
        /// </summary>
        /// <returns></returns>
        public abstract T Run(T context);

        public virtual T Invoke(T context)
        {
            if(ShouldRun(context))
                return Run(context);
            else
               return _next != null 
                    ? _next.Invoke(context) 
                    : context;
        }
    }

}
