namespace LiteLang.Base.Syntax
{
    public class SyntaxProgramNode : SyntaxCommandNode
    {
        public SyntaxProgramNode(params SyntaxNode[] Children)
            : base(Children)
        {
        }

        public override SyntaxNodeType GetType()
        {
            return SyntaxNodeType.Program;
        }

        public override LiteValue Accept(IVisitor Visitor, LiteEnv Env)
        {
            return Visitor.Visit(this, Env);
        }

        public override string ToString()
        {
            return $"Program[{Children_.Count}]";
        }
    }



    public class SyntaxIdentifierNode : SyntaxTerminalNode
    {
        private readonly string Value_;

        public SyntaxIdentifierNode(Token Tok)
            : base(Tok)
        {
            Value_ = Tok.Code;
        }

        public override SyntaxNodeType GetType()
        {
            return SyntaxNodeType.Identifier;
        }

        public override LiteValue Accept(IVisitor Visitor, LiteEnv Env)
        {
            return Visitor.Visit(this, Env);
        }

        public override string ToString()
        {
            return $"Ident<{Token_.Code}>";
        }

        public string GetValue()
        {
            return Value_;
        }
    }
}