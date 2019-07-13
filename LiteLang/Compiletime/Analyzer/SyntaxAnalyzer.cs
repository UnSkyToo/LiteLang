using LiteLang.Base;
using LiteLang.Base.Stream;
using LiteLang.Base.Syntax;

namespace LiteLang.Compiletime.Analyzer
{
    public class SyntaxAnalyzer : IAnalyzer
    {
        private readonly TokenStream TokenStream_;
        private SyntaxProgramNode ProgramNode_;
        private ExitCode ExitCode_;

        public SyntaxAnalyzer(TokenStream Tokens)
        {
            TokenStream_ = Tokens;
        }

        public ExitCode Analyzing()
        {
            ExitCode_ = ExitCode.Successful;
            ProgramNode_ = ParseProgram();
            return ExitCode_;
        }

        public SyntaxProgramNode GetProgramNode()
        {
            return ProgramNode_;
        }

        // program ::= [ class | fn | statement ] ( ";" )
        private SyntaxProgramNode ParseProgram()
        {
            var ProgramNode = new SyntaxProgramNode();

            while (!TokenStream_.IsEnd())
            {
                var Tok = TokenStream_.Peek();
                SyntaxNode Node;
                switch (Tok.Code)
                {
                    case "fn":
                        Node = ParseFunctionNode();
                        break;
                    case "class":
                        Node = ParseClassNode();
                        break;
                    default:
                        Node = ParseStatementNode();
                        break;
                }
                if (Node == null)
                {
                    return null;
                }
                ProgramNode.AddNode(Node);
                
                if (TokenStream_.PeekExpect(TokenType.Delimiter, ";"))
                {
                    TokenStream_.Take();
                }
            }

            return ProgramNode;
        }

        // statement ::= if | return | simple
        private SyntaxNode ParseStatementNode()
        {
            var Tok = TokenStream_.Peek();
            if (Tok.Type == TokenType.Keyword)
            {
                switch (Tok.Code)
                {
                    case "if":
                        return ParseIfNode();
                    case "while":
                        return ParseWhileNode();
                    case "return":
                        return ParseReturnNode();
                    default:
                        break;
                }

                ExitCode_ = new ExitUnexpectedSymbolCode(Tok);
                return null;
            }
            
            return ParseSimpleNode();
        }

        // block ::= "{" [ statement ] { ";" [ statement ] } "}"
        private SyntaxBlockStatementNode ParseBlockNode()
        {
            if (!TokenStream_.TakeExpect(TokenType.Delimiter, "{", out Token NextTok))
            {
                ExitCode_ = new ExitUnexpectedSymbolCode(NextTok);
                return null;
            }

            var BlockNode = new SyntaxBlockStatementNode();
            while (!TokenStream_.IsEnd())
            {
                var Tok = TokenStream_.Peek();
                if (Tok.Type == TokenType.Delimiter)
                {
                    TokenStream_.Take();
                    switch (Tok.Code)
                    {
                        case "}":
                            return BlockNode;
                        case ";":
                            continue;
                        default:
                            break;
                    }
                    ExitCode_ = new ExitUnexpectedSymbolCode(Tok);
                    return null;
                }

                var Node = ParseStatementNode();
                BlockNode.AddNode(Node);
            }

            ExitCode_ = new ExitFailedCode("end of file");
            return null;
        }

        // expr ::= term { <op> term }
        // op := "<" | ">" | "<=" | ">=" | "==" | "~=" | "&&" | "||" | ...... (see lang desc)
        private SyntaxNode ParseExprNode()
        {
            var Left = ParseTermNode();
            if (Left == null)
            {
                return null;
            }

            while (!TokenStream_.IsEnd())
            {
                var Tok = TokenStream_.Peek();
                switch (Tok.Type)
                {
                    case TokenType.Operator:
                        TokenStream_.Take();
                        var Right = ParseTermNode();
                        if (Right == null)
                        {
                            return null;
                        }
                        Left = new SyntaxBinaryExpressionNode(Tok, Left, Right);
                        break;
                    default:
                        return Left;
                }
            }

            return Left;
        }

        // term ::= factor { <'+' | '-'> factor }
        private SyntaxNode ParseTermNode()
        {
            var Left = ParseFactorNode();
            if (Left == null)
            {
                return null;
            }

            while (!TokenStream_.IsEnd())
            {
                var Tok = TokenStream_.Peek();
                switch (Tok.Type)
                {
                    case TokenType.Operator:
                        switch (Tok.Code)
                        {
                            case "+":
                            case "-":
                                TokenStream_.Take();
                                var Right = ParseFactorNode();
                                if (Right == null)
                                {
                                    return null;
                                }

                                Left = new SyntaxBinaryExpressionNode(Tok, Left, Right);
                                break;
                            default:
                                return Left;
                        }
                        break;
                    default:
                        return Left;
                }
            }

            return Left;
        }

        // factor ::= assignment | dot | call | primary { <'*' | '/' | '%' > primary }
        // call ::= args
        // dot ::= "." ident
        private SyntaxNode ParseFactorNode()
        {
            var Left = ParsePrimaryNode();
            if (Left == null)
            {
                return null;
            }

            while (!TokenStream_.IsEnd())
            {
                var Tok = TokenStream_.Peek();
                switch (Tok.Type)
                {
                    case TokenType.Operator:
                        switch (Tok.Code)
                        {
                            case "*":
                            case "/":
                            case "%":
                                TokenStream_.Take();
                                var Right = ParsePrimaryNode();
                                if (Right == null)
                                {
                                    return null;
                                }

                                Left = new SyntaxBinaryExpressionNode(Tok, Left, Right);
                                break;
                            case "=":
                                TokenStream_.Take();
                                var Val = ParseExprNode();
                                if (Val == null)
                                {
                                    return null;
                                }

                                Left = new SyntaxAssignmentExpressionNode(Tok, Left, Val);
                                break;
                            default:
                                return Left;
                        }
                        break;
                    case TokenType.Delimiter:
                        switch (Tok.Code)
                        {
                            case ".":
                                TokenStream_.Take();

                                if (Left.GetType() != SyntaxNodeType.Identifier)
                                {
                                    ExitCode_ = new ExitFailedCode(". must use for class ident");
                                    return null;
                                }

                                if (!TokenStream_.TakeExpect(TokenType.Identifier, out Token CallIdent))
                                {
                                    ExitCode_ = new ExitUnexpectedSymbolCode(CallIdent);
                                    return null;
                                }

                                Left = new SyntaxDotClassExpressionNode(Left as SyntaxIdentifierNode, new SyntaxIdentifierNode(CallIdent));
                                break;
                            case "(":
                                var ArgsNode = ParseArgumentListNode();
                                if (ArgsNode == null)
                                {
                                    return null;
                                }

                                Left = new SyntaxCallFunctionExpressionNode(Left, ArgsNode);
                                break;
                            case "[":
                                var IndexNode = ParseIndexNode();
                                if (IndexNode == null)
                                {
                                    return null;
                                }

                                Left = new SyntaxIndexElementsExpressionNode(Left, IndexNode);
                                break;
                            default:
                                return Left;
                        }
                        break;
                    default:
                        return Left;
                }
            }

            return Left;
        }

        // primary ::= ("fn" paramlist block | "-" primary | elements | "(" expr ")" | Number | String | Boolean | nil | Identifier ) { args }
        private SyntaxNode ParsePrimaryNode()
        {
            var Tok = TokenStream_.Take();

            switch (Tok.Type)
            {
                case TokenType.Keyword:
                    if (Tok.Code == "fn")
                    {
                        var ParamNode = ParseParamListNode();
                        if (ParamNode == null)
                        {
                            return null;
                        }

                        var BlockNode = ParseBlockNode();
                        if (BlockNode == null)
                        {
                            return null;
                        }

                        return new SyntaxFunctionNode(string.Empty, ParamNode, BlockNode);
                    }
                    break;
                case TokenType.Delimiter:
                    if (Tok.Code == "(")
                    {
                        var Expr = ParseExprNode();
                        if (!TokenStream_.TakeExpect(TokenType.Delimiter, ")", out Tok))
                        {
                            ExitCode_ = new ExitUnexpectedSymbolCode(Tok);
                            return null;
                        }
                        return Expr;
                    }

                    if (Tok.Code == "[")
                    {
                        TokenStream_.Back();
                        return ParseElementsNode();
                    }
                    break;
                case TokenType.Operator:
                    if (Tok.Code == "-")
                    {
                        var Expr = ParsePrimaryNode();
                        if (Expr != null)
                        {
                            return new SyntaxUnaryExpressionNode(Tok, Expr);
                        }
                    }
                    break;
                case TokenType.Numeric:
                    return new SyntaxNumericLiteralNode(Tok);
                case TokenType.String:
                    return new SyntaxStringLiteralNode(Tok);
                case TokenType.Boolean:
                    return new SyntaxBooleanLiteralNode(Tok);
                case TokenType.Nil:
                    return new SyntaxNilLiteralNode(Tok);
                case TokenType.Identifier:
                    return new SyntaxIdentifierNode(Tok);
                default:
                    break;
            }

            ExitCode_ = new ExitUnexpectedSymbolCode(Tok);
            return null;
        }

        // args ::= "(" [ expr { "," expr } ] ")"
        private SyntaxArgumentListStatementNode ParseArgumentListNode()
        {
            if (!TokenStream_.TakeExpect(TokenType.Delimiter, "(", out Token Tok))
            {
                ExitCode_ = new ExitUnexpectedSymbolCode(Tok);
                return null;
            }

            var Node = new SyntaxArgumentListStatementNode();

            if (TokenStream_.PeekExpect(TokenType.Delimiter, ")"))
            {
                TokenStream_.Take();
                return Node;
            }

            while (!TokenStream_.IsEnd())
            {
                var Arg = ParseExprNode();
                if (Arg == null)
                {
                    ExitCode_ = new ExitUnexpectedSymbolCode(TokenStream_.Peek());
                    return null;
                }

                Node.AddNode(Arg);

                if (!TokenStream_.TakeExpect(TokenType.Delimiter, out Tok))
                {
                    ExitCode_ = new ExitUnexpectedSymbolCode(Tok);
                    return null;
                }

                if (Tok.Code == ")")
                {
                    return Node;
                }
                if (Tok.Code == ",")
                {
                    continue;
                }

                ExitCode_ = new ExitUnexpectedSymbolCode(Tok);
            }

            ExitCode_ = new ExitUnexpectedSymbolCode(Tok);
            return null;
        }

        // elements ::= "[" [ expr { "," expr } ] "]"
        private SyntaxElementsStatementNode ParseElementsNode()
        {
            if (!TokenStream_.TakeExpect(TokenType.Delimiter, "[", out Token Tok))
            {
                ExitCode_ = new ExitUnexpectedSymbolCode(Tok);
                return null;
            }

            var Node = new SyntaxElementsStatementNode();

            if (TokenStream_.PeekExpect(TokenType.Delimiter, "]"))
            {
                TokenStream_.Take();
                return Node;
            }

            while (!TokenStream_.IsEnd())
            {
                var Arg = ParseExprNode();
                if (Arg == null)
                {
                    ExitCode_ = new ExitUnexpectedSymbolCode(TokenStream_.Peek());
                    return null;
                }

                Node.AddNode(Arg);

                if (!TokenStream_.TakeExpect(TokenType.Delimiter, out Tok))
                {
                    ExitCode_ = new ExitUnexpectedSymbolCode(Tok);
                    return null;
                }

                if (Tok.Code == "]")
                {
                    return Node;
                }
                if (Tok.Code == ",")
                {
                    continue;
                }

                ExitCode_ = new ExitUnexpectedSymbolCode(Tok);
            }

            ExitCode_ = new ExitUnexpectedSymbolCode(Tok);
            return null;
        }

        // simple ::= expr
        private SyntaxNode ParseSimpleNode()
        {
            return ParseExprNode();
        }

        // if ::= "if" "(" expr ")" block [ "else" block ]
        private SyntaxIfStatementNode ParseIfNode()
        {
            // skip "if"
            if (!TokenStream_.TakeExpect(TokenType.Keyword, "if", out Token Tok))
            {
                ExitCode_ = new ExitUnexpectedSymbolCode(Tok);
                return null;
            }

            // skip "("
            if (!TokenStream_.TakeExpect(TokenType.Delimiter, "(", out Tok))
            {
                ExitCode_ = new ExitUnexpectedSymbolCode(Tok);
                return null;
            }

            var ExprNode = ParseExprNode();
            if (ExprNode == null)
            {
                return null;
            }

            // skip ")"
            if (!TokenStream_.TakeExpect(TokenType.Delimiter, ")", out Tok))
            {
                ExitCode_ = new ExitUnexpectedSymbolCode(Tok);
                return null;
            }

            var BlockNode = ParseBlockNode();
            if (BlockNode == null)
            {
                return null;
            }

            if (!TokenStream_.PeekExpect(TokenType.Keyword, "else"))
            {
                return new SyntaxIfStatementNode(ExprNode, BlockNode, null);
            }

            // skip "else"
            TokenStream_.Take();
            SyntaxNode ElseNode;
            if (TokenStream_.PeekExpect(TokenType.Keyword, "if"))
            {
                ElseNode = ParseIfNode();
            }
            else
            {
                ElseNode = ParseBlockNode();
            }

            if (ElseNode == null)
            {
                return null;
            }
            return new SyntaxIfStatementNode(ExprNode, BlockNode, ElseNode);
        }

        // while ::= "while" "(" expr ")" block
        private SyntaxWhileStatementNode ParseWhileNode()
        {
            // skip "while"
            if (!TokenStream_.TakeExpect(TokenType.Keyword, "while", out Token Tok))
            {
                ExitCode_ = new ExitUnexpectedSymbolCode(Tok);
                return null;
            }

            // skip "("
            if (!TokenStream_.TakeExpect(TokenType.Delimiter, "(", out Tok))
            {
                ExitCode_ = new ExitUnexpectedSymbolCode(Tok);
                return null;
            }

            var ExprNode = ParseExprNode();
            if (ExprNode == null)
            {
                return null;
            }

            // skip ")"
            if (!TokenStream_.TakeExpect(TokenType.Delimiter, ")", out Tok))
            {
                ExitCode_ = new ExitUnexpectedSymbolCode(Tok);
                return null;
            }

            var BlockNode = ParseBlockNode();
            if (BlockNode == null)
            {
                return null;
            }

            return new SyntaxWhileStatementNode(ExprNode, BlockNode);
        }

        // return ::= "return" < ";" | expr >
        private SyntaxReturnStatementNode ParseReturnNode()
        {
            if (!TokenStream_.TakeExpect(TokenType.Keyword, "return", out Token Tok))
            {
                ExitCode_ = new ExitUnexpectedSymbolCode(Tok);
                return null;
            }

            if (TokenStream_.PeekExpect(TokenType.Delimiter, ";"))
            {
                TokenStream_.Take();
                return new SyntaxReturnStatementNode(null);
            }
            else
            {
                var Val = ParseExprNode();
                return new SyntaxReturnStatementNode(Val);
            }
        }

        // assignment ::= ident "=" expr
        private SyntaxAssignmentExpressionNode ParseAssignmentNode()
        {
            if (!TokenStream_.TakeExpect(TokenType.Identifier, out Token Ident))
            {
                ExitCode_ = new ExitUnexpectedSymbolCode(Ident);
                return null;
            }

            // skip "="
            if (!TokenStream_.TakeExpect(TokenType.Operator, "=", out Token Op))
            {
                ExitCode_ = new ExitUnexpectedSymbolCode(Op);
                return null;
            }

            var Expr = ParseExprNode();
            if (Expr == null)
            {
                return null;
            }

            return new SyntaxAssignmentExpressionNode(Op, new SyntaxIdentifierNode(Ident), Expr);
        }

        // fn ::= "fn" ident paramlist block
        private SyntaxFunctionNode ParseFunctionNode()
        {
            // skip "fn"
            if (!TokenStream_.TakeExpect(TokenType.Keyword, "fn", out Token Tok))
            {
                ExitCode_ = new ExitUnexpectedSymbolCode(Tok);
                return null;
            }

            if (!TokenStream_.TakeExpect(TokenType.Identifier, out Token FuncName))
            {
                ExitCode_ = new ExitUnexpectedSymbolCode(FuncName);
                return null;
            }

            var ParamNode = ParseParamListNode();
            if (ParamNode == null)
            {
                return null;
            }

            var BlockNode = ParseBlockNode();
            if (BlockNode == null)
            {
                return null;
            }

            return new SyntaxFunctionNode(FuncName.Code, ParamNode, BlockNode);
        }

        // paramlist ::= "(" [ param { "," param } ] ")"
        // param ::= ident
        private SyntaxParamListStatementNode ParseParamListNode()
        {
            if (!TokenStream_.TakeExpect(TokenType.Delimiter, "(", out Token Tok))
            {
                ExitCode_ = new ExitUnexpectedSymbolCode(Tok);
                return null;
            }

            var Node = new SyntaxParamListStatementNode();

            if (TokenStream_.PeekExpect(TokenType.Delimiter, ")"))
            {
                TokenStream_.Take();
                return Node;
            }

            while (!TokenStream_.IsEnd())
            {
                if (!TokenStream_.TakeExpect(TokenType.Identifier, out Token Param))
                {
                    ExitCode_ = new ExitUnexpectedSymbolCode(Param);
                    return null;
                }
                Node.AddNode(new SyntaxIdentifierNode(Param));

                if (!TokenStream_.TakeExpect(TokenType.Delimiter, out Tok))
                {
                    ExitCode_ = new ExitUnexpectedSymbolCode(Tok);
                    return null;
                }

                if (Tok.Code == ")")
                {
                    return Node;
                }
                if (Tok.Code == ",")
                {
                    continue;
                }

                ExitCode_ = new ExitUnexpectedSymbolCode(Tok);
            }

            ExitCode_ = new ExitUnexpectedSymbolCode(Tok);
            return null;
        }
        
        // class ::= "class" ident [ ":" ident ] class_block
        private SyntaxClassNode ParseClassNode()
        {
            if (!TokenStream_.TakeExpect(TokenType.Keyword, "class", out Token Tok))
            {
                ExitCode_ = new ExitUnexpectedSymbolCode(Tok);
                return null;
            }

            if (!TokenStream_.TakeExpect(TokenType.Identifier, out Token ClassName))
            {
                ExitCode_ = new ExitUnexpectedSymbolCode(ClassName);
                return null;
            }

            SyntaxIdentifierNode BaseClassIdentNode = null;
            if (TokenStream_.PeekExpect(TokenType.Delimiter, ":"))
            {
                // skip ":"
                TokenStream_.Take();
                if (!TokenStream_.TakeExpect(TokenType.Identifier, out Token BaseName))
                {
                    ExitCode_ = new ExitUnexpectedSymbolCode(BaseName);
                    return null;
                }

                BaseClassIdentNode = new SyntaxIdentifierNode(BaseName);
            }

            var ClassBodyNode = ParseClassBodyNode();
            if (ClassBodyNode == null)
            {
                return null;
            }

            return new SyntaxClassNode(ClassName.Code, ClassBodyNode, BaseClassIdentNode);
        }

        // class_block ::= "{" [ member ] { [ member ] } "}"
        private SyntaxClassBodyStatementNode ParseClassBodyNode()
        {
            if (!TokenStream_.TakeExpect(TokenType.Delimiter, "{", out Token Tok))
            {
                ExitCode_ = new ExitUnexpectedSymbolCode(Tok);
                return null;
            }

            var Node = new SyntaxClassBodyStatementNode();

            if (TokenStream_.PeekExpect(TokenType.Delimiter, "}"))
            {
                TokenStream_.Take();
                return Node;
            }

            while (!TokenStream_.IsEnd())
            {
                var Member = ParseClassMemberNode();
                if (Member == null)
                {
                    return null;
                }
                Node.AddNode(Member);

                if (TokenStream_.PeekExpect(TokenType.Delimiter, "}"))
                {
                    TokenStream_.Take();
                    return Node;
                }
            }

            ExitCode_ = new ExitUnexpectedSymbolCode(Tok);
            return null;
        }

        // member ::= fn | simple
        private SyntaxNode ParseClassMemberNode()
        {
            if (TokenStream_.PeekExpect(TokenType.Keyword, "fn"))
            {
                return ParseFunctionNode();
            }

            return ParseSimpleNode();
        }

        // index ::= "[" expr "]"
        private SyntaxNode ParseIndexNode()
        {
            if (!TokenStream_.TakeExpect(TokenType.Delimiter, "[", out Token Tok))
            {
                ExitCode_ = new ExitUnexpectedSymbolCode(Tok);
                return null;
            }

            var Ident = ParseExprNode();
            if (Ident == null)
            {
                return null;
            }

            if (!TokenStream_.TakeExpect(TokenType.Delimiter, "]", out Tok))
            {
                ExitCode_ = new ExitUnexpectedSymbolCode(Tok);
                return null;
            }

            return Ident;
        }
    }
}