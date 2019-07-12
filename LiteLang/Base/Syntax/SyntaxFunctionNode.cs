namespace LiteLang.Base.Syntax
{
    public class SyntaxFunctionNode : SyntaxCommandNode
    {
        private readonly string FuncName_;

        public SyntaxFunctionNode(string FuncName, SyntaxParamListStatementNode ParamListNode, SyntaxBlockStatementNode BlockNode)
            : base(ParamListNode, BlockNode)
        {
            FuncName_ = FuncName;
        }

        public override SyntaxNodeType GetType()
        {
            return SyntaxNodeType.Function;
        }

        public override LiteValue Accept(IVisitor Visitor, LiteEnv Env)
        {
            return Visitor.Visit(this, Env);
        }

        public override string ToString()
        {
            return $"fn({Children_[0]})[{Children_[1]}]";
        }

        public string GetFuncName()
        {
            return FuncName_;
        }

        public SyntaxNode GetParamList()
        {
            return Children_[0];
        }

        public SyntaxNode GetBlock()
        {
            return Children_[1];
        }
    }
}