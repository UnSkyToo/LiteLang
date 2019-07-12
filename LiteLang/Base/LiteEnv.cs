using System.Collections.Generic;
using LiteLang.Base.Log;

namespace LiteLang.Base
{
    public class LiteEnv
    {
        private readonly Dictionary<string, LiteValue> Variable_;
        private readonly Stack<LiteValue> Stack_;
        private LiteEnv OuterEnv_;

        public LiteEnv()
            : this(null)
        {
        }

        public LiteEnv(LiteEnv OuterEnv)
        {
            Variable_ = new Dictionary<string, LiteValue>();
            Stack_ = new Stack<LiteValue>();
            OuterEnv_ = OuterEnv;
        }

        public void Push(LiteValue Val)
        {
            Stack_.Push(Val);
        }

        public LiteValue Pop()
        {
            return Stack_.Pop();
        }

        public int StackCount()
        {
            return Stack_.Count;
        }

        public void SetOuterEnv(LiteEnv OuterEnv)
        {
            OuterEnv_ = OuterEnv;
        }

        public LiteValue Get(string Name)
        {
            if (Variable_.ContainsKey(Name))
            {
                return Variable_[Name];
            }

            if (OuterEnv_ != null)
            {
                return OuterEnv_.Get(Name);
            }

            Logger.DError($"unknown var : {Name}");
            return LiteValue.Nil;
        }

        public LiteValue Get(LiteValue Name)
        {
            if (Name.Type == LiteValueType.String)
            {
                return Get(StringTable.GetString((int)Name.Numeric));
            }
            Logger.DError($"unknown var : {StringTable.GetString((int)Name.Numeric)}");
            return LiteValue.Nil;
        }

        public void Set(string Name, LiteValue Val)
        {
            var Env = Where(Name);
            if (Env == null)
            {
                Env = this;
            }

            Env.SetSelf(Name, Val);
        }

        public void Set(LiteValue Name, LiteValue Val)
        {
            if (Name.Type == LiteValueType.String)
            {
                Set(StringTable.GetString((int)Name.Numeric), Val);
            }
            else
            {
                Logger.DError($"unknown var : {StringTable.GetString((int)Name.Numeric)}");
            }
        }

        public void SetSelf(string Name, LiteValue Val)
        {
            if (!Variable_.ContainsKey(Name))
            {
                Variable_.Add(Name, Val);
            }
            else
            {
                Variable_[Name] = Val;
            }
        }

        public void SetSelf(LiteValue Name, LiteValue Val)
        {
            if (Name.Type == LiteValueType.String)
            {
                SetSelf(StringTable.GetString((int)Name.Numeric), Val);
            }
            else
            {
                Logger.DError($"unknown var : {StringTable.GetString((int)Name.Numeric)}");
            }
        }

        public LiteEnv Where(string Name)
        {
            if (Variable_.ContainsKey(Name))
            {
                return this;
            }

            if (OuterEnv_ != null)
            {
                return OuterEnv_.Where(Name);
            }

            return null;
        }
    }
}