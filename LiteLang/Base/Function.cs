using LiteLang.Base.Syntax;

namespace LiteLang.Base
{
    public class Function
    {
        private readonly string Name_;
        private readonly Environment Env_;
        private readonly SyntaxParamListStatementNode ParamList_;
        private readonly SyntaxBlockStatementNode Block_;

        public Function(string Name, Environment Env, SyntaxParamListStatementNode ParamList, SyntaxBlockStatementNode Block)
        {
            this.Name_ = Name;
            this.Env_ = Env;
            this.ParamList_ = ParamList;
            this.Block_ = Block;
        }

        public string GetName()
        {
            return Name_;
        }

        public SyntaxParamListStatementNode GetParamList()
        {
            return ParamList_;
        }

        public SyntaxBlockStatementNode GetBlock()
        {
            return Block_;
        }

        public Environment MakeEnv()
        {
            return new Environment(Env_);
        }
    }
}