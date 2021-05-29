using System.Threading.Tasks;

namespace RulesChain.Contracts
{
    public interface IRule<TContext>
    {
        Task Invoke(TContext context);
        Task Run(TContext context);
        bool ShouldRun(TContext context);
    }
}