namespace LiteLang.Base.Syntax
{
    public class SyntaxLiteralNode : SyntaxTerminalNode
    {
        protected Value Value_;

        public SyntaxLiteralNode(Token Tok)
            : base(Tok)
        {
            Value_ = Value.Nil;
        }

        public override SyntaxNodeType GetType()
        {
            return SyntaxNodeType.Literal;
        }

        public override string ToString()
        {
            return $"{Token_.Type}:{Token_.Code}";
        }

        public Value GetValue()
        {
            return Value_;
        }
    }

    public class SyntaxNilLiteralNode : SyntaxLiteralNode
    {
        public SyntaxNilLiteralNode(Token Tok)
            : base(Tok)
        {
            Value_ = Value.Nil;
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
            Value_ = new Value(ValueType.Boolean, bool.Parse(Tok.Code) ? 1 : 0);
        }

        public override SyntaxNodeType GetType()
        {
            return SyntaxNodeType.Boolean;
        }

        public override Value Accept(IVisitor Visitor, Environment Env)
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
            Value_ = new Value(ValueType.Numeric, float.Parse(Tok.Code));
        }

        public override SyntaxNodeType GetType()
        {
            return SyntaxNodeType.Numeric;
        }

        public override Value Accept(IVisitor Visitor, Environment Env)
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
            Value_ = new Value(ValueType.String, StringTable.AddString(Tok.Code));
        }

        public override SyntaxNodeType GetType()
        {
            return SyntaxNodeType.String;
        }

        public override Value Accept(IVisitor Visitor, Environment Env)
        {
            return Visitor.Visit(this, Env);
        }

        public override string ToString()
        {
            return Token_.Code;
        }
    }

    public class SyntaxIdentifierNode : SyntaxLiteralNode
    {
        public SyntaxIdentifierNode(Token Tok)
            : base(Tok)
        {
            Value_ = new Value(ValueType.String, StringTable.AddString(Tok.Code));
        }

        public override SyntaxNodeType GetType()
        {
            return SyntaxNodeType.Identifier;
        }

        public override Value Accept(IVisitor Visitor, Environment Env)
        {
            return Visitor.Visit(this, Env);
        }

        public override string ToString()
        {
            return Token_.Code;
        }
    }
}