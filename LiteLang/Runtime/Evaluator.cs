using LiteLang.Base;
using LiteLang.Base.Log;
using LiteLang.Base.Syntax;

namespace LiteLang.Runtime
{
    public class Evaluator
    {
        private readonly Environment Env_;

        public Evaluator()
        {
            Env_ = new Environment();
            Env_.SetSelf("print", new Value(ValueType.Function, 
                FuncTable.AddFunc(new Function("print", Env_, null, null))));
        }

        public Value Eval(SyntaxNode Node)
        {
            var V = new EvalVisitor();
            return Node.Accept(V, Env_);
        }
    }

    public class EvalVisitor : IVisitor
    {
        public EvalVisitor()
        {
        }

        public Value Visit(SyntaxProgramNode Node, Environment Env)
        {
            var Val = Value.Nil;
            foreach (var Child in Node.GetChildren())
            {
                Val = Child.Accept(this, Env);
                //Logger.DInfo($"{Child} => {Val}");
            }
            return Val;
        }

        public Value Visit(SyntaxAssignmentExpressionNode Node, Environment Env)
        {
            var Ident = Node.GetLeft() as SyntaxIdentifierNode;
            var Val = Node.GetRight().Accept(this, Env);
            Env.Set(Ident.GetValue(), Val);
            return Val;
        }

        public Value Visit(SyntaxWhileStatementNode Node, Environment Env)
        {
            var Result = Value.Nil;
            while (true)
            {
                var Val = Node.GetExpressionNode().Accept(this, Env);

                switch (Val.Type)
                {
                    case ValueType.Nil:
                        return Result;
                    case ValueType.Boolean:
                    case ValueType.Numeric:
                        if (Val.IsZero())
                        {
                            return Result;
                        }
                        break;
                }

                Result = Node.GetBlockNode().Accept(this, Env);
            }
        }

        public Value Visit(SyntaxIfStatementNode Node, Environment Env)
        {
            var Val = Node.GetExpressionNode().Accept(this, Env);
            if (!Val.IsZero())
            {
                return Node.GetBlockNode().Accept(this, Env);
            }
            else
            {
                var ElseNode = Node.GetElseBlockNode();
                if (ElseNode == null)
                {
                    return Value.Nil;
                }

                return ElseNode.Accept(this, Env);
            }
        }

        public Value Visit(SyntaxBooleanLiteralNode Node, Environment Env)
        {
            return Node.GetValue();
        }

        public Value Visit(SyntaxNumericLiteralNode Node, Environment Env)
        {
            return Node.GetValue();
        }

        public Value Visit(SyntaxBinaryExpressionNode Node, Environment Env)
        {
            var ValLeft = Node.GetLeft().Accept(this, Env);
            var ValRight = Node.GetRight().Accept(this, Env);
            var Op = Node.GetOperator().Code;

            switch (Op)
            {
                case "<":
                    return ValLeft < ValRight ? Value.True : Value.False;
                case "<=":
                    return ValLeft <= ValRight ? Value.True : Value.False;
                case ">":
                    return ValLeft > ValRight ? Value.True : Value.False;
                case ">=":
                    return ValLeft >= ValRight ? Value.True : Value.False;
                case "==":
                    return ValLeft == ValRight ? Value.True : Value.False;
                case "~=":
                    return ValLeft != ValRight ? Value.True : Value.False;
                case "+":
                    return ValLeft + ValRight;
                case "-":
                    return ValLeft - ValRight;
                case "*":
                    return ValLeft * ValRight;
                case "/":
                    return ValLeft / ValRight;
                case "%":
                    return ValLeft % ValRight;
                default:
                    return Value.False;
            }
        }

        public Value Visit(SyntaxBlockStatementNode Node, Environment Env)
        {
            var Val = Value.Nil;
            foreach (var Child in Node.GetChildren())
            {
                Val = Child.Accept(this, Env);
                //Logger.DInfo($"{Child} => {Val}");
                if (Child.GetType() == SyntaxNodeType.ReturnStatement)
                {
                    return Val;
                }
            }
            return Val;
        }

        public Value Visit(SyntaxIdentifierNode Node, Environment Env)
        {
            return Env.Get(Node.GetValue());
        }

        public Value Visit(SyntaxFunctionNode Node, Environment Env)
        {
            var FuncIndex = FuncTable.AddFunc(new Function(Node.GetFuncName(), Env,
                Node.GetParamList() as SyntaxParamListStatementNode, Node.GetBlock() as SyntaxBlockStatementNode));
            var FuncValue = new Value(ValueType.Function, FuncIndex);
            Env.SetSelf(Node.GetFuncName(), FuncValue);
            return FuncValue;
        }

        public Value Visit(SyntaxCallFunctionExpressionNode Node, Environment Env)
        {
            var FuncIndex = Value.Nil;
            var FuncName = Node.GetFuncIdentNode();
            if (FuncName.GetType() == SyntaxNodeType.CallFunctionExpression)
            {
                FuncIndex = Visit(FuncName as SyntaxCallFunctionExpressionNode, Env);
            }
            else if (FuncName.GetType() == SyntaxNodeType.Identifier)
            {
                FuncIndex = Env.Get((FuncName as SyntaxIdentifierNode).GetValue());
            }

            if (FuncIndex == Value.Nil || FuncIndex.Type != ValueType.Function)
            {
                Logger.DError($"unknown function : {FuncName}");
                return Value.Nil;
            }

            var Func = FuncTable.GetFunc((int)FuncIndex.Numeric);
            if (Func == null)
            {
                Logger.DError($"=> unknown fn name : {FuncIndex.Numeric}");
                return Value.Nil;
            }

            var NewEnv = Func.MakeEnv();
            var ParamList = Func.GetParamList();
            var ArgList = Node.GetArgumentListNode() as SyntaxArgumentListStatementNode;

            if (Func.GetName() == "print")
            {
                foreach (var Arg in ArgList.GetChildren())
                {
                    Logger.DInfo($"print => {Arg.Accept(this, Env)}");

                }
                return Value.Nil;
            }

            for (var Index = 0; Index < ParamList.GetChildrenNum(); ++Index)
            {
                var ArgName = ParamList.GetChild<SyntaxIdentifierNode>(Index).GetValue();
                var ArgValue = Index >= ArgList.GetChildrenNum() ? Value.Nil : ArgList.GetChild(Index).Accept(this, Env);
                NewEnv.SetSelf(ArgName, ArgValue);
            }

            var Val = Func.GetBlock().Accept(this, NewEnv);
            //Logger.DInfo($"=> call [{Func.GetName()}] = {Val}");
            return Val;
        }

        public Value Visit(SyntaxReturnStatementNode Node, Environment Env)
        {
            if (Node.GetChildrenNum() > 0)
            {
                return Node.GetChild(0).Accept(this, Env);
            }
            return Value.Nil;
        }

        public Value Visit(SyntaxStringLiteralNode Node, Environment Env)
        {
            return Node.GetValue();
        }
    }
}