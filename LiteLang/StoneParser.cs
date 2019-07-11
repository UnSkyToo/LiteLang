using System;
using System.Collections.Generic;
using LiteLang.Base;
using LiteLang.Base.Stream;
using LiteLang.Base.Syntax;

namespace LiteLang
{
    public class StoneParser
    {
        private abstract class Element
        {
            public abstract void Parse(TokenStream Lexer, List<SyntaxNode> Res);
            public abstract bool Match(TokenStream Lexer);
        }

        private class Tree : Element
        {
            private readonly StoneParser StoneParser_;

            public Tree(StoneParser P)
            {
                StoneParser_ = P;
            }

            public override void Parse(TokenStream Lexer, List<SyntaxNode> Res)
            {
                Res.Add(StoneParser_.Parse(Lexer));
            }

            public override bool Match(TokenStream Lexer)
            {
                return StoneParser_.Match(Lexer);
            }
        }

        private class OrTree : Element
        {
            private readonly List<StoneParser> Parsers_;

            public OrTree(StoneParser[] P)
            {
                Parsers_ = new List<StoneParser>(P);
            }

            public override void Parse(TokenStream Lexer, List<SyntaxNode> Res)
            {
                var P = Choose(Lexer);
                if (P == null)
                {
                    throw new Exception("parse error");
                }
                else
                {
                    Res.Add(P.Parse(Lexer));
                }
            }

            public override bool Match(TokenStream Lexer)
            {
                return Choose(Lexer) != null;
            }

            private StoneParser Choose(TokenStream Lexer)
            {
                foreach (var P in Parsers_)
                {
                    if (P.Match(Lexer))
                    {
                        return P;
                    }
                }

                return null;
            }

            public void Insert(StoneParser P)
            {
                Parsers_.Add(P);
            }
        }

        private class RepeatTree : Element
        {
            private StoneParser StoneParser_;
            private bool OnlyOnce_;

            public RepeatTree(StoneParser P, bool Once)
            {
                StoneParser_ = P;
                OnlyOnce_ = Once;
            }

            public override void Parse(TokenStream Lexer, List<SyntaxNode> Res)
            {
                while (StoneParser_.Match(Lexer))
                {
                    var T = StoneParser_.Parse(Lexer);
                    if (!(T is SyntaxCommandNode Cmd) || Cmd.GetChildrenNum() > 0)
                    {
                        Res.Add(T);
                    }

                    if (OnlyOnce_)
                    {
                        break;
                    }
                }
            }

            public override bool Match(TokenStream Lexer)
            {
                return StoneParser_.Match(Lexer);
            }
        }

        private abstract class AToken : Element
        {
            private Factory Factory_;

            public AToken(Type T)
            {
                if (T == null)
                {
                    T = typeof(SyntaxTerminalNode);
                }

                Factory_ = Factory.Get(T, typeof(Token));
            }

            public override void Parse(TokenStream Lexer, List<SyntaxNode> Res)
            {
                var Tok = Lexer.Take();
                if (Test(Tok))
                {
                    var Leaf = Factory_.Make(Tok);
                    Res.Add(Leaf);
                }
                else
                {
                    throw new Exception("parse error");
                }
            }

            public override bool Match(TokenStream Lexer)
            {
                return Test(Lexer.Peek());
            }

            public abstract bool Test(Token Tok);
        }

        private class IdToken : AToken
        {
            private HashSet<string> Reserved_;

            public IdToken(Type T, HashSet<string> R)
                : base(T)
            {
                Reserved_ = R ?? new HashSet<string>();
            }

            public override bool Test(Token Tok)
            {
                return Tok.Type == TokenType.Identifier && !Reserved_.Contains(Tok.Code);
            }
        }

        private class NumToken : AToken
        {
            public NumToken(Type T)
                : base(T)
            {
            }

            public override bool Test(Token Tok)
            {
                return Tok.Type == TokenType.Numeric;
            }
        }

        private class StrToken : AToken
        {
            public StrToken(Type T)
                : base(T)
            {
            }

            public override bool Test(Token Tok)
            {
                return Tok.Type == TokenType.String;
            }
        }

        private class Leaf : Element
        {
            private string[] Tokens_;

            public Leaf(string[] Pat)
            {
                Tokens_ = Pat;
            }

            public override void Parse(TokenStream Lexer, List<SyntaxNode> Res)
            {
                var Tok = Lexer.Take();
                if (Tok.Type == TokenType.Identifier)
                {
                    foreach (var Str in Tokens_)
                    {
                        if (Tok.Code == Str)
                        {
                            Find(Res, Tok);
                            return;
                        }
                    }
                }

                if (Tokens_.Length > 0)
                {
                    throw new Exception($"{Tokens_[0]} expected");
                }
                else
                {
                    throw new Exception("parse error");
                }
            }

            public virtual void Find(List<SyntaxNode> Res, Token Tok)
            {
                //Res.Add(new SyntaxTerminalNode(Tok));
            }

            public override bool Match(TokenStream Lexer)
            {
                var Tok = Lexer.Peek();
                if (Tok.Type == TokenType.Identifier)
                {
                    foreach (var Str in Tokens_)
                    {
                        if (Tok.Code == Str)
                        {
                            return true;
                        }
                    }
                }
                return false;
            }
        }

        private class Skip : Leaf
        {
            public Skip(string[] T)
                : base(T)
            {
            }

            public override void Find(List<SyntaxNode> Res, Token Tok)
            {
            }
        }

        public class Precedence
        {
            public int Value { get; }
            public bool LeftAssoc { get; }

            public Precedence(int Value, bool Assoc)
            {
                this.Value = Value;
                this.LeftAssoc = Assoc;
            }
        }

        public class Operators : Dictionary<string, Precedence>
        {
            public static bool Left = true;
            public static bool Right = false;

            public void Add(string Name, int Prec, bool LeftAssoc)
            {
                this.Add(Name, new Precedence(Prec, LeftAssoc));
            }
        }

        private class Expr : Element
        {
            private Factory Factory_;
            private Operators Ops_;
            private StoneParser Factor_;

            public Expr(Type T, StoneParser Exp, Operators Map)
            {
                Factory_ = Factory.GetForSyntax(T);
                Ops_ = Map;
                Factor_ = Exp;
            }

            public override void Parse(TokenStream Lexer, List<SyntaxNode> Res)
            {
                var Right = Factor_.Parse(Lexer);
                Precedence Prec;
                while ((Prec = NextOperator(Lexer)) != null)
                {
                    Right = DoShift(Lexer, Right, Prec.Value);
                }

                Res.Add(Right);
            }

            private SyntaxNode DoShift(TokenStream Lexer, SyntaxNode Left, int Prec)
            {
                var List = new List<SyntaxNode>();
                List.Add(Left);
                //List.Add(new SyntaxTerminalNode(Lexer.Take()));
                var Right = Factor_.Parse(Lexer);
                Precedence Next;
                while ((Next = NextOperator(Lexer)) != null && RightIsExpr(Prec, Next))
                {
                    Right = DoShift(Lexer, Right, Next.Value);
                }

                List.Add(Right);
                return Factory_.Make(List);
            }

            private Precedence NextOperator(TokenStream Lexer)
            {
                var Tok = Lexer.Peek();
                if (Tok.Type == TokenType.Identifier)
                {
                    return Ops_[Tok.Code];
                }
                return null;
            }

            private bool RightIsExpr(int Prec, Precedence NextPrec)
            {
                if (NextPrec.LeftAssoc)
                {
                    return Prec < NextPrec.Value;
                }
                else
                {
                    return Prec <= NextPrec.Value;
                }
            }

            public override bool Match(TokenStream Lexer)
            {
                return Factor_.Match(Lexer);
            }
        }

        private class Factory
        {
            private readonly Func<object, SyntaxNode> CreateFunc_;

            private Factory(Func<object, SyntaxNode> CreateFunc)
            {
                CreateFunc_ = CreateFunc;
            }

            public SyntaxNode Make(object Arg)
            {
                return CreateFunc_(Arg);
            }

            public static Factory GetForSyntax(Type T)
            {
                var F = Get(T, typeof(List<SyntaxNode>));
                /*if (F == null)
                {
                    F = new Factory((Arg) =>
                    {
                        var Result = (List<SyntaxNode>)Arg;
                        if (Result.Count == 1)
                        {
                            return Result[0];
                        }
                        return new SyntaxCommandNode(Result.ToArray());
                    });
                }*/

                return F;
            }

            public static Factory Get(Type T, Type P)
            {
                if (T == null)
                {
                    return null;
                }

                return new Factory((Arg) =>
                {
                    var Con = T.GetConstructor(new[] {P});
                    if (Con != null)
                    {
                        return (SyntaxNode)Con.Invoke(new[] {Arg});
                    }
                    return null;
                });
            }
        }

        private List<Element> Elements_;
        private Factory Factory_;

        public StoneParser(Type T)
        {
            Reset(T);
        }

        public StoneParser(StoneParser P)
        {
            Elements_ = P.Elements_;
            Factory_ = P.Factory_;
        }

        public SyntaxNode Parse(TokenStream Lexer)
        {
            var Result = new List<SyntaxNode>();
            foreach (var Ele in Elements_)
            {
                Ele.Parse(Lexer, Result);
            }

            return Factory_.Make(Result);
        }

        public bool Match(TokenStream Lexer)
        {
            if (Elements_.Count == 0)
            {
                return true;
            }
            else
            {
                var Ele = Elements_[0];
                return Ele.Match(Lexer);
            }
        }

        public static StoneParser Rule()
        {
            return Rule(null);
        }

        public static StoneParser Rule(Type T)
        {
            return new StoneParser(T);
        }

        public StoneParser Reset()
        {
            Elements_ = new List<Element>();
            return this;
        }

        public StoneParser Reset(Type T)
        {
            Elements_ = new List<Element>();
            Factory_ = Factory.GetForSyntax(T);
            return this;
        }

        public StoneParser Number()
        {
            return Number(null);
        }

        public StoneParser Number(Type T)
        {
            Elements_.Add(new NumToken(T));
            return this;
        }

        public StoneParser Identifier(HashSet<string> Reserved)
        {
            return Identifier(null, Reserved);
        }

        public StoneParser Identifier(Type T, HashSet<string> Reserved)
        {
            Elements_.Add(new IdToken(T, Reserved));
            return this;
        }

        public StoneParser String()
        {
            return String(null);
        }

        public StoneParser String(Type T)
        {
            Elements_.Add(new StrToken(T));
            return this;
        }

        public StoneParser Token(params string[] Pat)
        {
            Elements_.Add(new Leaf(Pat));
            return this;
        }

        public StoneParser Sep(params string[] Pat)
        {
            Elements_.Add(new Skip(Pat));
            return this;
        }

        public StoneParser Ast(StoneParser P)
        {
            Elements_.Add(new Tree(P));
            return this;
        }

        public StoneParser Or(params StoneParser[] P)
        {
            Elements_.Add(new OrTree(P));
            return this;
        }

        public StoneParser Maybe(StoneParser P)
        {
            var P2 = new StoneParser(P);
            P2.Reset();
            Elements_.Add(new OrTree(new []{ P, P2 }));
            return this;
        }

        public StoneParser Option(StoneParser P)
        {
            Elements_.Add(new RepeatTree(P, true));
            return this;
        }

        public StoneParser Repeat(StoneParser P)
        {
            Elements_.Add(new RepeatTree(P, false));
            return this;
        }

        public StoneParser Expression(StoneParser SubExp, Operators Ops)
        {
            Elements_.Add(new Expr(null, SubExp, Ops));
            return this;
        }

        public StoneParser Expression(Type T, StoneParser SubExp, Operators Ops)
        {
            Elements_.Add(new Expr(T, SubExp, Ops));
            return this;
        }

        public StoneParser InsertChoice(StoneParser P)
        {
            var Ele = Elements_[0];
            if (Ele is OrTree OrT)
            {
                OrT.Insert(P);
            }
            else
            {
                var Otherwise = new StoneParser(this);
                Reset(null);
                Or(P, Otherwise);
            }

            return this;
        }
    }

    public class BasicStoneParser
    {
        private static HashSet<string> Reserved_ = new HashSet<string>();
        private static StoneParser.Operators Ops_ = new StoneParser.Operators();
        private static StoneParser Expr0_ = StoneParser.Rule();

        private static StoneParser Primary_ = StoneParser.Rule(typeof(SyntaxBinaryExpressionNode)).Or(
            StoneParser.Rule().Sep("(").Ast(Expr0_).Sep(")"),
            StoneParser.Rule().Number(typeof(SyntaxNumericLiteralNode)),
            StoneParser.Rule().Identifier(typeof(SyntaxIdentifierNode), Reserved_),
            StoneParser.Rule().String(typeof(SyntaxStringLiteralNode)));

        private static StoneParser Factor_ = StoneParser.Rule().Or(StoneParser.Rule(typeof(SyntaxUnaryExpressionNode)).Sep("-").Ast(Primary_), Primary_);
        private static StoneParser Expr_ = Expr0_.Expression(typeof(SyntaxBinaryExpressionNode), Factor_, Ops_);
        private static StoneParser Statement0_ = StoneParser.Rule();
        private static StoneParser Block_ = StoneParser.Rule();

        public BasicStoneParser()
        {
            Reserved_.Add(";");
            Reserved_.Add("}");
            Reserved_.Add("\n");

            Ops_.Add("=", 1, StoneParser.Operators.Right);
            Ops_.Add("==", 2, StoneParser.Operators.Left);
            Ops_.Add(">", 2, StoneParser.Operators.Left);
            Ops_.Add("<", 2, StoneParser.Operators.Left);
            Ops_.Add("+", 3, StoneParser.Operators.Left);
            Ops_.Add("-", 3, StoneParser.Operators.Left);
            Ops_.Add("*", 4, StoneParser.Operators.Left);
            Ops_.Add("/", 4, StoneParser.Operators.Left);
            Ops_.Add("%", 4, StoneParser.Operators.Left);
        }

        public SyntaxNode Parse(TokenStream Lexer)
        {
            return Expr_.Parse(Lexer);
        }
    }
}
