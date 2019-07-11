using System.Collections.Generic;
using LiteLang.Base;
using LiteLang.Base.Log;
using LiteLang.Base.Stream;
using LiteLang.Compiletime.Description;

namespace LiteLang.Compiletime.Analyzer
{
    public class LexicalAnalyzer : IAnalyzer
    {
        internal enum LexerStateType
        {
            Begin = 0,
            Integer = 1,
            Float = 2,
            String = 3,
            Identity = 4,
            Operator = 5,
            End = 255,
        }

        private readonly CharacterStream CharStream_;
        private readonly ILanguageDescription LangDesc_;
        private TokenStream TokenStream_;

        private LexerStateType CurrentType_ = LexerStateType.End;
        private readonly StringStream CurrentText_;
        private int CurrentLine_ = 1;

        public LexicalAnalyzer(CharacterStream CharStream, ILanguageDescription LangDesc)
        {
            CharStream_ = CharStream;
            CurrentText_ = new StringStream();
            LangDesc_ = LangDesc;
        }

        public TokenStream GetTokenStream()
        {
            return TokenStream_;
        }

        public ExitCode Analyzing()
        {
            CharStream_.Reset();
            CurrentLine_ = 1;
            var Tokens = new List<Token>();

            while (!CharStream_.IsEnd())
            {
                var Tok = ParseToken();
                if (Tok.Type == TokenType.None)
                {
                    continue;
                }
                if (Tok.Type == TokenType.Error)
                {
                    return new ExitUnexpectedSymbolCode(Tok);
                }
                Tokens.Add(Tok);
            }

            TokenStream_ = new TokenStream(Tokens.ToArray());
            return ExitCode.Successful;
        }

        private Token ParseToken()
        {
            var TokType = TokenType.Error;
            CurrentType_ = LexerStateType.Begin;
            CurrentText_.Reset();

            while (!CharStream_.IsEnd() && CurrentType_ != LexerStateType.End)
            {
                var Ch = CharStream_.Take();
                CurrentText_.Push(Ch);

                switch (CurrentType_)
                {
                    #region Begin
                    case LexerStateType.Begin:
                        if (LangDesc_.IsWhitespaceChar(Ch))
                        {
                            TokType = TokenType.None;
                            CurrentType_ = LexerStateType.End;
                        }
                        else if (LangDesc_.IsEndOfLineChar(Ch))
                        {
                            CurrentLine_++;

                            TokType = TokenType.None;
                            CurrentType_ = LexerStateType.End;
                        }
                        else if (LangDesc_.IsDelimiterChar(Ch))
                        {
                            TokType = TokenType.Delimiter;
                            CurrentType_ = LexerStateType.End;
                        }
                        else if (LangDesc_.IsDigitChar(Ch))
                        {
                            CurrentType_ = LexerStateType.Integer;
                        }
                        else if (LangDesc_.IsQuoteChar(Ch))
                        {
                            CurrentType_ = LexerStateType.String;
                        }
                        else if (LangDesc_.IsIdentityChar(Ch))
                        {
                            CurrentType_ = LexerStateType.Identity;
                        }
                        else if (LangDesc_.IsOperatorChar(Ch))
                        {
                            CurrentType_ = LexerStateType.Operator;
                        }
                        break;
                    #endregion
                    #region Integer
                    case LexerStateType.Integer:
                        if (Ch == '.')
                        {
                            CurrentType_ = LexerStateType.Float;
                        }
                        else if (!LangDesc_.IsDigitChar(Ch))
                        {
                            CurrentText_.Pop();
                            CharStream_.Back();

                            if (LangDesc_.IsWhitespaceChar(Ch) ||
                                LangDesc_.IsOperatorChar(Ch) ||
                                LangDesc_.IsDelimiterChar(Ch) ||
                                LangDesc_.IsEndOfLineChar(Ch))
                            {
                                TokType = TokenType.Numeric;
                            }
                            else
                            {
                                TokType = TokenType.Error;
                            }

                            CurrentType_ = LexerStateType.End;
                        }
                        break;
                    #endregion
                    #region Float
                    case LexerStateType.Float:
                        if (!LangDesc_.IsDigitChar(Ch))
                        {
                            CurrentText_.Pop();
                            CharStream_.Back();

                            if (LangDesc_.IsWhitespaceChar(Ch) || LangDesc_.IsDelimiterChar(Ch))
                            {
                                TokType = TokenType.Numeric;
                            }
                            else
                            {
                                TokType = TokenType.Error;
                            }

                            CurrentType_ = LexerStateType.End;
                        }
                        break;
                    #endregion
                    #region String
                    case LexerStateType.String:
                        if (Ch == '\\')
                        {
                            CurrentText_.Pop();
                            CurrentText_.Push(CharStream_.Take());
                        }
                        else if (LangDesc_.IsQuoteChar(Ch) && CurrentText_.Index(0) == Ch)
                        {
                            CurrentText_.Pop();
                            CurrentText_.Remove(0);

                            TokType = TokenType.String;
                            CurrentType_ = LexerStateType.End;
                        }
                        else if (LangDesc_.IsEndOfLineChar(Ch))
                        {
                            CurrentText_.Pop();
                            CharStream_.Back();

                            TokType = TokenType.Error;
                            CurrentType_ = LexerStateType.End;
                        }
                        break;
                    #endregion
                    #region Identity
                    case LexerStateType.Identity:
                        if (!LangDesc_.IsIdentityChar(Ch) && !LangDesc_.IsDigitChar(Ch))
                        {
                            CurrentText_.Pop();
                            CharStream_.Back();

                            var TokCode = CurrentText_.ToString();
                            if (LangDesc_.IsNilString(TokCode))
                            {
                                TokType = TokenType.Nil;
                            }
                            else if (LangDesc_.IsBooleanString(TokCode))
                            {
                                TokType = TokenType.Boolean;
                            }
                            else if (LangDesc_.IsKeywordString(TokCode))
                            {
                                TokType = TokenType.Keyword;
                            }
                            else
                            {
                                TokType = TokenType.Identifier;
                            }
                            CurrentType_ = LexerStateType.End;
                        }
                        break;
                    #endregion
                    #region Operator
                    case LexerStateType.Operator:
                        if (!LangDesc_.IsOperatorChar(Ch))
                        {
                            CurrentText_.Pop();
                            CharStream_.Back();

                            if (LangDesc_.IsOperatorString(CurrentText_.ToString()))
                            {
                                TokType = TokenType.Operator;
                            }
                            else
                            {
                                TokType = TokenType.Error;
                            }

                            CurrentType_ = LexerStateType.End;
                        }
                        break;
                        #endregion
                }
            }

            return new Token(TokType, CurrentText_.ToString(), CurrentLine_);
        }

        public void Display()
        {
            if (TokenStream_ == null)
            {
                return;
            }

            TokenStream_.Reset();

            while (!TokenStream_.IsEnd())
            {
                var Tok = TokenStream_.Take();
                Logger.DInfo($"[{Tok.Line}] {Tok.Type} : {Tok.Code}");
            }

            TokenStream_.Reset();
        }
    }
}