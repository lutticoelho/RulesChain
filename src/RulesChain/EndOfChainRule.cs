namespace RulesChain
{
    public class EndOfChainRule<T> : Rule<T>
    {
        public EndOfChainRule() : base(null){}
        
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
            return new EndOfChainRule<T>();
        }

    }

}
