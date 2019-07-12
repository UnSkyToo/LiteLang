using LiteLang.Base.Syntax;

namespace LiteLang.Base
{
    public class ClassInfo
    {
        private readonly string Name_;
        private readonly ClassInfo BaseClass_;
        private readonly LiteEnv Env_;
        private readonly SyntaxClassBodyStatementNode Body_;

        public ClassInfo(string Name, LiteEnv Env, SyntaxClassBodyStatementNode BodyNode, ClassInfo BaseClass)
        {
            this.Name_ = Name;
            this.BaseClass_ = BaseClass;
            this.Env_ = Env;
            this.Body_ = BodyNode;
        }

        public string GetName()
        {
            return Name_;
        }

        public ClassInfo GetBaseClass()
        {
            return BaseClass_;
        }

        public SyntaxClassBodyStatementNode GetBody()
        {
            return Body_;
        }

        public LiteEnv MakeEnv()
        {
            return new LiteEnv(Env_);
        }

        public LiteEnv GetEnv()
        {
            return Env_;
        }
    }
}