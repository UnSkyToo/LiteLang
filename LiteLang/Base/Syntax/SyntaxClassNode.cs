namespace LiteLang.Base.Syntax
{
    public class SyntaxClassNode : SyntaxCommandNode
    {
        private readonly string ClassName_;

        public SyntaxClassNode(string ClassName, SyntaxClassBodyStatementNode BlockNode, SyntaxIdentifierNode BaseClassIdentNode)
            : base(BlockNode, BaseClassIdentNode)
        {
            ClassName_ = ClassName;
        }

        public override SyntaxNodeType GetType()
        {
            return SyntaxNodeType.Class;
        }

        public override LiteValue Accept(IVisitor Visitor, LiteEnv Env)
        {
            return Visitor.Visit(this, Env);
        }

        public override string ToString()
        {
            return $"class<{ClassName_}>{Children_[0]}";
        }

        public string GetClassName()
        {
            return ClassName_;
        }

        public SyntaxNode GetClassBody()
        {
            return Children_[0];
        }

        public SyntaxNode GetBaseClassIdentNode()
        {
            if (Children_.Count > 1)
            {
                return Children_[1];
            }
            return null;
        }
    }
}