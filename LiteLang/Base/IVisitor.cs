using LiteLang.Base.Syntax;

namespace LiteLang.Base
{
    public interface IVisitor
    {
        LiteValue Visit(SyntaxProgramNode Node, LiteEnv Env);
        LiteValue Visit(SyntaxAssignmentExpressionNode Node, LiteEnv Env);
        LiteValue Visit(SyntaxWhileStatementNode Node, LiteEnv Env);
        LiteValue Visit(SyntaxIfStatementNode Node, LiteEnv Env);
        LiteValue Visit(SyntaxBooleanLiteralNode Node, LiteEnv Env);
        LiteValue Visit(SyntaxNumericLiteralNode Node, LiteEnv Env);
        LiteValue Visit(SyntaxBinaryExpressionNode Node, LiteEnv Env);
        LiteValue Visit(SyntaxBlockStatementNode Node, LiteEnv Env);
        LiteValue Visit(SyntaxIdentifierNode Node, LiteEnv Env);
        LiteValue Visit(SyntaxFunctionNode Node, LiteEnv Env);
        LiteValue Visit(SyntaxCallFunctionExpressionNode Node, LiteEnv Env);
        LiteValue Visit(SyntaxReturnStatementNode Node, LiteEnv Env);
        LiteValue Visit(SyntaxStringLiteralNode Node, LiteEnv Env);
        LiteValue Visit(SyntaxClassNode Node, LiteEnv Env);
        LiteValue Visit(SyntaxClassBodyStatementNode Node, LiteEnv Env);
        LiteValue Visit(SyntaxDotClassExpressionNode Node, LiteEnv Env);
        LiteValue Visit(SyntaxElementsStatementNode Node, LiteEnv Env);
        LiteValue Visit(SyntaxIndexElementsExpressionNode Node, LiteEnv Env);
    }
}