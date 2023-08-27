using System.Runtime.CompilerServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using RulesChain.Contracts;

namespace RulesChain
{
    /// <inheritdoc cref="IRuleChain{TContext}"/>
    public class RuleChain<TContext> : IRuleChain<TContext>
    {
        private readonly IServiceProvider _services;
        private readonly Stack<Func<RuleHandlerDelegate<TContext>, RuleHandlerDelegate<TContext>>> _components =
            new Stack<Func<RuleHandlerDelegate<TContext>, RuleHandlerDelegate<TContext>>>();

        private bool _built;

        public RuleChain(IServiceProvider services) => _services = services;

        /// <summary>
        /// Gets services from <see cref="IServiceProvider"/>
        /// </summary>
        /// <param name="type">Type of the service that should be build</param>
        /// <param name="args">Constructor parameters</param>
        /// <returns>A concrete instance o the requested service.</returns>
        protected virtual object GetService(Type type, params object[] args)
        {
            return args == null || args.Length == 0
                ? _services.GetService(type)
                : Activator.CreateInstance(type, args);
        }

        /// <inheritdoc cref="IRuleChain{TContext}.Build"/>
        public RuleHandlerDelegate<TContext> Build()
        {
            if (_built) throw new InvalidOperationException("Chain can only be built once");
            var next = new RuleHandlerDelegate<TContext>(context => Task.CompletedTask);
            while (_components.Any())
            {
                // Stryker disable once block
                var component = _components.Pop();
                next = component(next);
            }
            _built = true;
            return next;
        }
        
        /// <inheritdoc cref="IRuleChain{TContext}.Build"/>
        public IRuleChain<TContext> Use<TRule>() where TRule : IRule<TContext>
        {
            _components.Push(CreateDelegate<TRule>);
            return this;
        }

        private RuleHandlerDelegate<TContext> CreateDelegate<TRule>(RuleHandlerDelegate<TContext> next)
        {
            var ruleType = typeof(TRule);
            MethodInfo methodInfo = GetValidInvokeMethodInfo(ruleType);

            //Constructor parameters
            var constructorArguments = new object[] { next };
            var dependencies = GetDependencies(ruleType, GetService);
            if (dependencies.Any())
                constructorArguments = constructorArguments.Concat(dependencies).ToArray();

            //Create the rule instance using the constructor arguments (including dependencies)
            var rule = GetService(ruleType, constructorArguments);

            //return the delegate for the rule
            return (RuleHandlerDelegate<TContext>)methodInfo
                .CreateDelegate(typeof(RuleHandlerDelegate<TContext>), rule);
        }

        internal MethodInfo GetValidInvokeMethodInfo(Type type)
        {
            //Must have public method named Invoke or InvokeAsync.
            var methodInfo = type.GetMethod("Invoke");
            if (methodInfo == null)
                throw new InvalidOperationException("Missing invoke method");

            //This method must: Return a Task.
            if (!typeof(Task).IsAssignableFrom(methodInfo.ReturnType))
                throw new InvalidOperationException("invalid invoke return type");
            
            //and accept a first parameter of type TContext.
            ParameterInfo[] parameters = methodInfo.GetParameters();
            if (parameters.Length != 1 || parameters[0].ParameterType != typeof(TContext))
                throw new InvalidOperationException("invalid invoke parameter type");

            return methodInfo;
        }

        private object[] GetDependencies(Type middlewareType, Func<Type, object[], object> factory)
        {
            var constructors = middlewareType.GetConstructors().Where(c => c.IsPublic).ToArray();
            var constructor = constructors.Length == 1 ? constructors[0]
                : constructors.OrderByDescending(c => c.GetParameters().Length).FirstOrDefault();

            if (constructor == null)
                return Array.Empty<object>();

            var ctorArgsTypes = constructor.GetParameters().Select(p => p.ParameterType).ToArray();
            return ctorArgsTypes
                .Skip(1) //Skipping first argument since it is suppose to be next delegate
                .Select(parameter => factory(parameter, null)) //resolve other parameters
                .ToArray();
        }
    }
}
