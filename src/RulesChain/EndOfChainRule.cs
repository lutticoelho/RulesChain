namespace RulesChain
{
    internal class EndOfChainRule<T> : Rule<T>
    {
        public EndOfChainRule(Rule<T> next) : base(next)
        {
        }

        public override bool ShouldRun(T context)
        {
            return true;
        }

        public override T Run(T context)
        {
            return context;
        }

        internal static EndOfChainRule<T> EndOfChain()
        {
            return new EndOfChainRule<T>(null);
        }

    }

}
