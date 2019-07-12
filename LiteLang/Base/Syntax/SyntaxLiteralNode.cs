namespace LiteLang.Base.Syntax
{
    public class SyntaxLiteralNode : SyntaxTerminalNode
    {
        protected LiteValue Value_;

        public SyntaxLiteralNode(Token Tok)
            : base(Tok)
        {
            Value_ = LiteValue.Nil;
        }

        public override SyntaxNodeType GetType()
        {
            return SyntaxNodeType.Literal;
        }

        public override string ToString()
        {
            return $"{Token_.Type}:{Token_.Code}";
        }

        public LiteValue GetValue()
        {
            return Value_;
        }
    }

    public class SyntaxNilLiteralNode : SyntaxLiteralNode
    {
        public SyntaxNilLiteralNode(Token Tok)
            : base(Tok)
        {
            Value_ = LiteValue.Nil;
        }

        public override SyntaxNodeType GetType()
        {
            return SyntaxNodeType.Nil;
        }

        public override string ToString()
        {
            return "nil";
        }
    }

    public class SyntaxBooleanLiteralNode : SyntaxLiteralNode
    {
        public SyntaxBooleanLiteralNode(Token Tok)
            : base(Tok)
        {
            Value_ = new LiteValue(LiteValueType.Boolean, bool.Parse(Tok.Code) ? 1 : 0);
        }

        public override SyntaxNodeType GetType()
        {
            return SyntaxNodeType.Boolean;
        }

        public override LiteValue Accept(IVisitor Visitor, LiteEnv Env)
        {
            return Visitor.Visit(this, Env);
        }

        public override string ToString()
        {
            return $"{Value_}";
        }
    }

    public class SyntaxNumericLiteralNode : SyntaxLiteralNode
    {
        public SyntaxNumericLiteralNode(Token Tok)
            : base(Tok)
        {
            Value_ = new LiteValue(LiteValueType.Numeric, float.Parse(Tok.Code));
        }

        public override SyntaxNodeType GetType()
        {
            return SyntaxNodeType.Numeric;
        }

        public override LiteValue Accept(IVisitor Visitor, LiteEnv Env)
        {
            return Visitor.Visit(this, Env);
        }

        public override string ToString()
        {
            return $"{Value_}";
        }
    }

    public class SyntaxStringLiteralNode : SyntaxLiteralNode
    {
        public SyntaxStringLiteralNode(Token Tok)
            : base(Tok)
        {
            Value_ = new LiteValue(LiteValueType.String, StringTable.AddString(Tok.Code));
        }

        public override SyntaxNodeType GetType()
        {
            return SyntaxNodeType.String;
        }

        public override LiteValue Accept(IVisitor Visitor, LiteEnv Env)
        {
            return Visitor.Visit(this, Env);
        }

        public override string ToString()
        {
            return Token_.Code;
        }
    }
}