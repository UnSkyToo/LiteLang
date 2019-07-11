namespace LiteLang.Base.Syntax
{
    public class SyntaxExpressionNode : SyntaxCommandNode
    {
        protected readonly Token Operator_;

        protected SyntaxExpressionNode(Token Operator, params SyntaxNode[] Children)
            : base(Children)
        {
            Operator_ = Operator;
        }

        public override SyntaxNodeType GetType()
        {
            return SyntaxNodeType.Expression;
        }

        public Token GetOperator()
        {
            return Operator_;
        }
    }

    public class SyntaxUnaryExpressionNode : SyntaxExpressionNode
    {
        public SyntaxUnaryExpressionNode(Token Operator, SyntaxNode Argument)
            : base(Operator, Argument)
        {
        }

        public override SyntaxNodeType GetType()
        {
            return SyntaxNodeType.UnaryExpression;
        }

        public override string ToString()
        {
            return $"UnaryExp ({Operator_.Code}{Children_[0]})";
        }

        public SyntaxNode GetArgument()
        {
            return Children_[0];
        }
    }

    public class SyntaxBinaryExpressionNode : SyntaxExpressionNode
    {
        public SyntaxBinaryExpressionNode(Token Operator, SyntaxNode Left, SyntaxNode Right)
            : base(Operator, Left, Right)
        {
        }

        public override SyntaxNodeType GetType()
        {
            return SyntaxNodeType.BinaryExpression;
        }

        public override Value Accept(IVisitor Visitor, Environment Env)
        {
            return Visitor.Visit(this, Env);
        }

        public override string ToString()
        {
            return $"BinaryExp ({Children_[0]} {Operator_.Code} {Children_[1]})";
        }

        public SyntaxNode GetLeft()
        {
            return Children_[0];
        }

        public SyntaxNode GetRight()
        {
            return Children_[1];
        }
    }

    public class SyntaxAssignmentExpressionNode : SyntaxExpressionNode
    {
        public SyntaxAssignmentExpressionNode(Token Operator, SyntaxNode Left, SyntaxNode Right)
            : base(Operator, Left, Right)
        {
        }

        public override SyntaxNodeType GetType()
        {
            return SyntaxNodeType.AssignmentExpression;
        }

        public override Value Accept(IVisitor Visitor, Environment Env)
        {
            return Visitor.Visit(this, Env);
        }

        public override string ToString()
        {
            return $"AssignmentExp ({Children_[0]} {Operator_.Code} {Children_[1]})";
        }

        public SyntaxNode GetLeft()
        {
            return Children_[0];
        }

        public SyntaxNode GetRight()
        {
            return Children_[1];
        }
    }

    public class SyntaxCallFunctionExpressionNode : SyntaxExpressionNode
    {
        public SyntaxCallFunctionExpressionNode(SyntaxNode FuncIdentNode, SyntaxArgumentListStatementNode ArgumentListNode)
            : base(new Token(TokenType.Operator, "()"), FuncIdentNode, ArgumentListNode)
        {
        }

        public override SyntaxNodeType GetType()
        {
            return SyntaxNodeType.CallFunctionExpression;
        }

        public override Value Accept(IVisitor Visitor, Environment Env)
        {
            return Visitor.Visit(this, Env);
        }

        public override string ToString()
        {
            return $"Call({Children_[1]})";
        }

        public SyntaxNode GetFuncIdentNode()
        {
            return Children_[0];
        }

        public SyntaxNode GetArgumentListNode()
        {
            return Children_[1];
        }
    }
}