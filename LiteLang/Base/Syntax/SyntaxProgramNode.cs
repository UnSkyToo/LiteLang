﻿namespace LiteLang.Base.Syntax
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

        public override Value Accept(IVisitor Visitor, Environment Env)
        {
            return Visitor.Visit(this, Env);
        }

        public override string ToString()
        {
            return $"Program[{Children_.Count}]";
        }
    }

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

        public override Value Accept(IVisitor Visitor, Environment Env)
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