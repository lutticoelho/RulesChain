using System;
using System.Collections.Generic;
using System.Linq;
using RulesChain.Contracts;

namespace RulesChain
{
    public class RuleChain<T> : IRuleChain<T>
    {
        private readonly IList<Type> _components = new List<Type>();

        public IRuleChain<T> Use<TRule>()
        {
            _components.Add(typeof(TRule));
            return this;
        }

        public IRule<T> Build()
        {
            IRule<T> app = EndOfChainRule<T>.EndOfChain();

            foreach (var component in _components.Reverse())
            {
                app = (IRule<T>)Activator.CreateInstance(component,app);
            }

            return app;
        }
    }

}
