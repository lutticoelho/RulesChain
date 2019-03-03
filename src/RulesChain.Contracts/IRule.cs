namespace RulesChain.Contracts
{
    public interface IRule<T>
    {
        T Invoke(T context);
        T Run(T context);
        bool ShouldRun(T context);
    }
}