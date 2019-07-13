namespace LiteLang.Base.Syntax
{
    public class SyntaxStatementNode : SyntaxCommandNode
    {
        public SyntaxStatementNode(params SyntaxNode[] Children)
            : base(Children)
        {
        }

        public override SyntaxNodeType GetType()
        {
            return SyntaxNodeType.Statement;
        }
    }

    public class SyntaxBlockStatementNode : SyntaxStatementNode
    {
        public SyntaxBlockStatementNode()
            : base(null)
        {
        }

        public override SyntaxNodeType GetType()
        {
            return SyntaxNodeType.BlockStatement;
        }

        public override LiteValue Accept(IVisitor Visitor, LiteEnv Env)
        {
            return Visitor.Visit(this, Env);
        }

        public override string ToString()
        {
            return $"Block {Children_.Count}";
        }
    }

    public class SyntaxIfStatementNode : SyntaxStatementNode
    {
        public SyntaxIfStatementNode(SyntaxNode ExprNode, SyntaxBlockStatementNode BlockNode, SyntaxNode ElseNode)
            : base(ExprNode, BlockNode, ElseNode)
        {
        }

        public override SyntaxNodeType GetType()
        {
            return SyntaxNodeType.IfStatement;
        }

        public override LiteValue Accept(IVisitor Visitor, LiteEnv Env)
        {
            return Visitor.Visit(this, Env);
        }

        public override string ToString()
        {
            return $"if ({Children_[0]})";
        }

        public SyntaxNode GetExpressionNode()
        {
            return Children_[0];
        }

        public SyntaxNode GetBlockNode()
        {
            return Children_[1];
        }

        public SyntaxNode GetElseBlockNode()
        {
            return Children_.Count > 1 ? Children_[2] : null;
        }
    }

    public class SyntaxWhileStatementNode : SyntaxStatementNode
    {
        public SyntaxWhileStatementNode(SyntaxNode ExprNode, SyntaxBlockStatementNode BlockNode)
            : base(ExprNode, BlockNode)
        {
        }

        public override SyntaxNodeType GetType()
        {
            return SyntaxNodeType.WhileStatement;
        }

        public override LiteValue Accept(IVisitor Visitor, LiteEnv Env)
        {
            return Visitor.Visit(this, Env);
        }

        public override string ToString()
        {
            return $"while ({Children_[0]})";
        }

        public SyntaxNode GetExpressionNode()
        {
            return Children_[0];
        }

        public SyntaxNode GetBlockNode()
        {
            return Children_[1];
        }
    }

    public class SyntaxReturnStatementNode : SyntaxStatementNode
    {
        public SyntaxReturnStatementNode(SyntaxNode ValNode)
            : base(ValNode)
        {
        }

        public override SyntaxNodeType GetType()
        {
            return SyntaxNodeType.ReturnStatement;
        }

        public override LiteValue Accept(IVisitor Visitor, LiteEnv Env)
        {
            return Visitor.Visit(this, Env);
        }

        public override string ToString()
        {
            return $"return({Children_.Count})";
        }
    }

    public class SyntaxParamListStatementNode : SyntaxStatementNode
    {
        public SyntaxParamListStatementNode(params SyntaxNode[] Children)
            : base(Children)
        {
        }

        public override SyntaxNodeType GetType()
        {
            return SyntaxNodeType.ParamListStatement;
        }

        public override string ToString()
        {
            return $"Params[{Children_.Count}]";
        }
    }

    public class SyntaxArgumentListStatementNode : SyntaxStatementNode
    {
        public SyntaxArgumentListStatementNode(params SyntaxNode[] Children)
            : base(Children)
        {
        }

        public override SyntaxNodeType GetType()
        {
            return SyntaxNodeType.ArgumentListStatement;
        }

        public override string ToString()
        {
            return $"Args[{Children_.Count}]";
        }
    }

    public class SyntaxClassBodyStatementNode : SyntaxStatementNode
    {
        public SyntaxClassBodyStatementNode(params SyntaxNode[] Children)
            : base(Children)
        {
        }

        public override SyntaxNodeType GetType()
        {
            return SyntaxNodeType.ClassBodyStatement;
        }

        public override LiteValue Accept(IVisitor Visitor, LiteEnv Env)
        {
            return Visitor.Visit(this, Env);
        }

        public override string ToString()
        {
            return $"class body[{Children_.Count}]";
        }
    }

    public class SyntaxElementsStatementNode : SyntaxStatementNode
    {
        public SyntaxElementsStatementNode(params SyntaxNode[] Children)
            : base(Children)
        {
        }

        public override SyntaxNodeType GetType()
        {
            return SyntaxNodeType.ElementsStatement;
        }

        public override LiteValue Accept(IVisitor Visitor, LiteEnv Env)
        {
            return Visitor.Visit(this, Env);
        }

        public override string ToString()
        {
            return $"Elements[{Children_.Count}]";
        }
    }
}