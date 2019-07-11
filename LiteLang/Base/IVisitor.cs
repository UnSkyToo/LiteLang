using LiteLang.Base.Syntax;

namespace LiteLang.Base
{
    public interface IVisitor
    {
        Value Visit(SyntaxProgramNode Node, Environment Env);
        Value Visit(SyntaxAssignmentExpressionNode Node, Environment Env);
        Value Visit(SyntaxWhileStatementNode Node, Environment Env);
        Value Visit(SyntaxIfStatementNode Node, Environment Env);
        Value Visit(SyntaxBooleanLiteralNode Node, Environment Env);
        Value Visit(SyntaxNumericLiteralNode Node, Environment Env);
        Value Visit(SyntaxBinaryExpressionNode Node, Environment Env);
        Value Visit(SyntaxBlockStatementNode Node, Environment Env);
        Value Visit(SyntaxIdentifierNode Node, Environment Env);
        Value Visit(SyntaxFunctionNode Node, Environment Env);
        Value Visit(SyntaxCallFunctionExpressionNode Node, Environment Env);
        Value Visit(SyntaxReturnStatementNode Node, Environment Env);
        Value Visit(SyntaxStringLiteralNode Node, Environment Env);
    }
}