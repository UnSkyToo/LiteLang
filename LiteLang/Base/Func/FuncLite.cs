using LiteLang.Base.Syntax;

namespace LiteLang.Base.Func
{
    public class FuncLite : FuncBase
    {
        private readonly LiteEnv Env_;
        private readonly SyntaxParamListStatementNode ParamList_;
        private readonly SyntaxBlockStatementNode Block_;

        public FuncLite(string Name, LiteEnv Env, SyntaxParamListStatementNode ParamList, SyntaxBlockStatementNode Block)
            : base(Name)
        {
            this.Env_ = Env;
            this.ParamList_ = ParamList;
            this.Block_ = Block;
        }

        public SyntaxParamListStatementNode GetParamList()
        {
            return ParamList_;
        }

        public SyntaxBlockStatementNode GetBlock()
        {
            return Block_;
        }

        public LiteEnv MakeEnv()
        {
            return new LiteEnv(Env_);
        }
    }
}