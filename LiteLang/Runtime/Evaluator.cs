using System;
using System.Text;
using LiteLang.Base;
using LiteLang.Base.Func;
using LiteLang.Base.Log;
using LiteLang.Base.Syntax;

namespace LiteLang.Runtime
{

    public class Evaluator
    {
        private readonly LiteEnv Env_;

        public Evaluator()
        {
            Env_ = new LiteEnv();

            Register("print", Print);
            Register("time", Time);
        }

        public LiteValue Eval(SyntaxNode Node)
        {
            var V = new EvalVisitor();
            return Node.Accept(V, Env_);
        }

        public void Register(string Name, LiteLangNativeFunc Func)
        {
            Env_.SetSelf(Name, FuncTable.AddFuncEx(new FuncNative(Name, Env_, Func)));
        }

        public static bool Print(LiteEnv Env)
        {
            var ParamCount = (int)(Env.Pop().Numeric);
            if (ParamCount == 0)
            {
                Logger.DInfo(string.Empty);
            }
            else if (ParamCount == 1)
            {
                Logger.DInfo($"Print => {Env.Pop()}");
            }
            else
            {
                var Params = new LiteValue[ParamCount - 1];
                for (var Index = 0; Index < ParamCount - 1; ++Index)
                {
                    Params[Index] = Env.Pop();
                }

                var Text = new StringBuilder(Env.Pop().ToString());

                for (var Index = 0; Index < ParamCount - 1; ++Index)
                {
                    Text = Text.Replace($"{{{Index}}}", Params[Index].ToString());
                }

                Logger.DInfo($"Print => {Text}");
            }

            return false;
        }

        public static bool Time(LiteEnv Env)
        {
            var ParamCount = (int)(Env.Pop().Numeric);
            if (ParamCount != 0)
            {
                for (var Index = 0; Index < ParamCount; ++Index)
                {
                    Env.Pop();
                }
            }

            Env.Push(new LiteValue(LiteValueType.Numeric, DateTime.Now.Ticks / 10000.0d));
            return true;
        }
    }

    public class EvalVisitor : IVisitor
    {
        public EvalVisitor()
        {
        }

        public LiteValue Visit(SyntaxProgramNode Node, LiteEnv Env)
        {
            var Val = LiteValue.Nil;
            foreach (var Child in Node.GetChildren())
            {
                Val = Child.Accept(this, Env);
                //Logger.DInfo($"{Child} => {Val}");
                if (Val.IsError())
                {
                    break;
                }
            }
            return Val;
        }

        public LiteValue Visit(SyntaxAssignmentExpressionNode Node, LiteEnv Env)
        {
            var LeftNode = Node.GetLeft();

            if (LeftNode.GetType() == SyntaxNodeType.Identifier)
            {
                var Ident = (LeftNode as SyntaxIdentifierNode).GetValue();

                var Val = Node.GetRight().Accept(this, Env);
                Env.Set(Ident, Val);
                return Val;
            }
            if (LeftNode.GetType() == SyntaxNodeType.DotClassExpression)
            {
                var DotNode = LeftNode as SyntaxDotClassExpressionNode;
                var Mem = DotNode.GetCallIdentNode() as SyntaxIdentifierNode;
                var LiteObjVal = DotNode.GetClassIdentNode().Accept(this, Env);
                if (LiteObjVal.Type != LiteValueType.Object)
                {
                    Logger.DError($"bad object access : {LiteObjVal}");
                    return LiteValue.Error;
                }

                var LiteObj = ObjectTable.GetObject((int)LiteObjVal.Numeric);
                if (LiteObj == null)
                {
                    Logger.DError($"bad object access : {LiteObjVal}");
                    return LiteValue.Error;
                }

                var ExpVal = Node.GetRight().Accept(this, Env);
                if (ExpVal == LiteValue.Error)
                {
                    return ExpVal;
                }

                return LiteObj.Write(Mem.GetValue(), ExpVal);
            }

            Logger.DError($"unexpected '=' near {Node.GetLeft()}");
            return LiteValue.Error;
        }

        public LiteValue Visit(SyntaxWhileStatementNode Node, LiteEnv Env)
        {
            var Result = LiteValue.Nil;
            while (true)
            {
                var Val = Node.GetExpressionNode().Accept(this, Env);

                switch (Val.Type)
                {
                    case LiteValueType.Nil:
                        return Result;
                    case LiteValueType.Boolean:
                    case LiteValueType.Numeric:
                        if (Val.IsZero())
                        {
                            return Result;
                        }
                        break;
                }

                Result = Node.GetBlockNode().Accept(this, Env);
            }
        }

        public LiteValue Visit(SyntaxIfStatementNode Node, LiteEnv Env)
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
                    return LiteValue.Nil;
                }

                return ElseNode.Accept(this, Env);
            }
        }

        public LiteValue Visit(SyntaxBooleanLiteralNode Node, LiteEnv Env)
        {
            return Node.GetValue();
        }

        public LiteValue Visit(SyntaxNumericLiteralNode Node, LiteEnv Env)
        {
            return Node.GetValue();
        }

        public LiteValue Visit(SyntaxBinaryExpressionNode Node, LiteEnv Env)
        {
            var ValLeft = Node.GetLeft().Accept(this, Env);
            var ValRight = Node.GetRight().Accept(this, Env);
            var Op = Node.GetOperator().Code;

            switch (Op)
            {
                case "<":
                    return ValLeft < ValRight ? LiteValue.True : LiteValue.False;
                case "<=":
                    return ValLeft <= ValRight ? LiteValue.True : LiteValue.False;
                case ">":
                    return ValLeft > ValRight ? LiteValue.True : LiteValue.False;
                case ">=":
                    return ValLeft >= ValRight ? LiteValue.True : LiteValue.False;
                case "==":
                    return ValLeft == ValRight ? LiteValue.True : LiteValue.False;
                case "~=":
                    return ValLeft != ValRight ? LiteValue.True : LiteValue.False;
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
                    Logger.DError($"unknown op : {Op}");
                    return LiteValue.Error;
            }
        }

        public LiteValue Visit(SyntaxBlockStatementNode Node, LiteEnv Env)
        {
            var Val = LiteValue.Nil;
            foreach (var Child in Node.GetChildren())
            {
                Val = Child.Accept(this, Env);
                //Logger.DInfo($"{Child} => {Val}");
                if (Child.GetType() == SyntaxNodeType.ReturnStatement)
                {
                    return Val;
                }
                if (Val.IsError())
                {
                    break;
                }
            }
            return Val;
        }

        public LiteValue Visit(SyntaxIdentifierNode Node, LiteEnv Env)
        {
            return Env.Get(Node.GetValue());
        }

        public LiteValue Visit(SyntaxFunctionNode Node, LiteEnv Env)
        {
            var FuncValue = FuncTable.AddFuncEx(new FuncLite(Node.GetFuncName(), Env,
                Node.GetParamList() as SyntaxParamListStatementNode, Node.GetBlock() as SyntaxBlockStatementNode));
            Env.SetSelf(Node.GetFuncName(), FuncValue);
            return FuncValue;
        }

        public LiteValue Visit(SyntaxCallFunctionExpressionNode Node, LiteEnv Env)
        {
            var FuncIndex = LiteValue.Nil;
            var FuncName = Node.GetFuncIdentNode();
            if (FuncName.GetType() == SyntaxNodeType.CallFunctionExpression)
            {
                FuncIndex = Visit(FuncName as SyntaxCallFunctionExpressionNode, Env);
            }
            else if (FuncName.GetType() == SyntaxNodeType.DotClassExpression)
            {
                var DotNode = FuncName as SyntaxDotClassExpressionNode;
                FuncIndex = Visit(DotNode, Env);

                if ((DotNode.GetCallIdentNode() as SyntaxIdentifierNode).GetValue() == "New")
                {
                    return FuncIndex;
                }
            }
            else if (FuncName.GetType() == SyntaxNodeType.Identifier)
            {
                FuncIndex = Env.Get((FuncName as SyntaxIdentifierNode).GetValue());
            }

            if (FuncIndex == LiteValue.Nil || FuncIndex.Type != LiteValueType.Function)
            {
                Logger.DError($"unknown function : {FuncName}");
                return LiteValue.Error;
            }

            var Func = FuncTable.GetFunc((int)FuncIndex.Numeric);
            if (Func == null)
            {
                Logger.DError($"=> unknown fn name : {FuncIndex.Numeric}");
                return LiteValue.Error;
            }
            return CallFunc(Func, Node.GetArgumentListNode() as SyntaxArgumentListStatementNode, Env);
        }

        private LiteValue CallFunc(FuncBase Func, SyntaxArgumentListStatementNode ArgList, LiteEnv Env)
        {
            if (Func is FuncNative FN)
            {
                for (var Index = 0; Index < ArgList.GetChildrenNum(); ++Index)
                {
                    var ArgValue = Index >= ArgList.GetChildrenNum()
                        ? LiteValue.Nil
                        : ArgList.GetChild(Index).Accept(this, Env);
                    FN.Push(ArgValue);
                }
                FN.Push(ArgList.GetChildrenNum());
                return FN.Invoke();
            }
            else if (Func is FuncLite FL)
            {
                var NewEnv = FL.MakeEnv();
                var ParamList = FL.GetParamList();

                for (var Index = 0; Index < ParamList.GetChildrenNum(); ++Index)
                {
                    var ArgName = ParamList.GetChild<SyntaxIdentifierNode>(Index).GetValue();
                    var ArgValue = Index >= ArgList.GetChildrenNum()
                        ? LiteValue.Nil
                        : ArgList.GetChild(Index).Accept(this, Env);
                    NewEnv.SetSelf(ArgName, ArgValue);
                }

                var Val = FL.GetBlock().Accept(this, NewEnv);
                //Logger.DInfo($"=> call [{Func.GetName()}] = {Val}");
                return Val;
            }

            return LiteValue.Nil;
        }

        public LiteValue Visit(SyntaxReturnStatementNode Node, LiteEnv Env)
        {
            if (Node.GetChildrenNum() > 0)
            {
                return Node.GetChild(0).Accept(this, Env);
            }
            return LiteValue.Nil;
        }

        public LiteValue Visit(SyntaxStringLiteralNode Node, LiteEnv Env)
        {
            return Node.GetValue();
        }

        public LiteValue Visit(SyntaxClassNode Node, LiteEnv Env)
        {
            ClassInfo BaseCls = null;
            if (Node.GetBaseClassIdentNode() is SyntaxIdentifierNode BaseIdent)
            {
                var Val = Env.Get(BaseIdent.GetValue());
                if (Val.Type == LiteValueType.Class)
                {
                    BaseCls = ClassTable.GetClass((int)Val.Numeric);
                }

                if (BaseCls == null)
                {
                    Logger.DError($"error base class : {BaseIdent.GetValue()}");
                    return LiteValue.Error;
                }
            }

            var ClsValue = ClassTable.AddClassEx(new ClassInfo(Node.GetClassName(), Env,
                Node.GetClassBody() as SyntaxClassBodyStatementNode,
                BaseCls));
            Env.SetSelf(Node.GetClassName(), ClsValue);
            return ClsValue;
        }

        public LiteValue Visit(SyntaxClassBodyStatementNode Node, LiteEnv Env)
        {
            var Val = LiteValue.Nil;
            foreach (var Child in Node.GetChildren())
            {
                Val = Child.Accept(this, Env);
                if (Val.IsError())
                {
                    break;
                }
            }
            return Val;
        }

        public LiteValue Visit(SyntaxDotClassExpressionNode Node, LiteEnv Env)
        {
            var Val = Node.GetClassIdentNode().Accept(this, Env);
            var Mem = (Node.GetCallIdentNode() as SyntaxIdentifierNode).GetValue();
            if (Val.Type == LiteValueType.Class)
            {
                if (Mem == "New")
                {
                    var Cls = ClassTable.GetClass((int)Val.Numeric);
                    var ObjEnv = Cls.MakeEnv();
                    var LiteObj = new LiteObject(ObjEnv);
                    var Obj = ObjectTable.AddObjectEx(LiteObj);
                    ObjEnv.SetSelf("this", Obj);
                    LiteObj.InitObject(this, Cls, ObjEnv);
                    return Obj;
                }
            }
            else if (Val.Type == LiteValueType.Object)
            {
                var LiteObj = ObjectTable.GetObject((int)Val.Numeric);
                if (LiteObj == null)
                {
                    Logger.DError($"bad member access : {Val}");
                    return LiteValue.Error;
                }

                return LiteObj.Read(Mem);
            }
            else
            {
                Logger.DError($"unknown class type : {Val}");
            }

            return LiteValue.Error;
        }
    }
}