using System.Collections.Generic;

namespace LiteLang.Base.Syntax
{
    public enum SyntaxNodeType : short
    {
        CommandNode = -100,
        Terminal = -99,
        // Other
        Program = 0,
        Literal = 1,
        Nil = 2,
        Boolean = 3,
        Numeric = 4,
        String = 5,
        Identifier = 6,
        Function = 7,
        Class = 8,
        // Declaration
        Declaration = 10,
        VariableDeclaration = 11,
        // Statement
        Statement = 100,
        IfStatement = 101,
        ForStatement = 102,
        ForInStatement = 103,
        BlockStatement = 104,
        BreakStatement = 105,
        WhileStatement = 106,
        ReturnStatement = 107,
        ContinueStatement = 108,
        ParamListStatement = 109,
        ArgumentListStatement = 110,
        ClassBodyStatement = 111,
        // Expression
        Expression = 200,
        UnaryExpression = 201,
        BinaryExpression = 203,
        AssignmentExpression = 207,
        CallFunctionExpression = 208,
        DotClassExpression = 209,
    }

    public abstract class SyntaxNode
    {
        public new abstract SyntaxNodeType GetType();
        public abstract LiteValue Accept(IVisitor Visitor, LiteEnv Env);
    }

    public class SyntaxCommandNode : SyntaxNode
    {
        protected readonly List<SyntaxNode> Children_;

        protected SyntaxCommandNode(params SyntaxNode[] Children)
        {
            Children_ = new List<SyntaxNode>();
            if (Children != null)
            {
                Children_.AddRange(Children);
            }
        }

        public override SyntaxNodeType GetType()
        {
            return SyntaxNodeType.CommandNode;
        }

        public override LiteValue Accept(IVisitor Visitor, LiteEnv Env)
        {
            return LiteValue.Nil;
        }

        public List<SyntaxNode> GetChildren()
        {
            return Children_;
        }

        public SyntaxNode GetChild(int Index)
        {
            if (Index < 0 || Index >= Children_.Count)
            {
                return null;
            }

            return Children_[Index];
        }

        public T GetChild<T>(int Index) where T : SyntaxNode
        {
            return GetChild(Index) as T;
        }

        public int GetChildrenNum()
        {
            return Children_.Count;
        }

        public void AddNode(SyntaxNode Node)
        {
            Children_.Add(Node);
        }
    }

    public class SyntaxTerminalNode : SyntaxNode
    {
        protected readonly Token Token_;

        protected SyntaxTerminalNode(Token Tok)
        {
            Token_ = Tok;
        }

        public Token GetToken()
        {
            return Token_;
        }

        public override SyntaxNodeType GetType()
        {
            return SyntaxNodeType.Terminal;
        }

        public override LiteValue Accept(IVisitor Visitor, LiteEnv Env)
        {
            return LiteValue.Nil;
        }
    }
}