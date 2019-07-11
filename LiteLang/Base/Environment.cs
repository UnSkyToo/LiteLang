using System.Collections.Generic;
using LiteLang.Base.Log;

namespace LiteLang.Base
{
    public class Environment
    {
        private readonly Dictionary<string, Value> Variable_;
        private Environment OuterEnv_;

        public Environment()
            : this(null)
        {
        }

        public Environment(Environment OuterEnv)
        {
            Variable_ = new Dictionary<string, Value>();
            OuterEnv_ = OuterEnv;
        }

        public void SetOuterEnv(Environment OuterEnv)
        {
            OuterEnv_ = OuterEnv;
        }

        public Value Get(string Name)
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
            return Value.Nil;
        }

        public Value Get(Value Name)
        {
            if (Name.Type == ValueType.Ident || Name.Type == ValueType.String)
            {
                return Get(StringTable.GetString((int)Name.Numeric));
            }
            Logger.DError($"unknown var : {StringTable.GetString((int)Name.Numeric)}");
            return Value.Nil;
        }

        public void Set(string Name, Value Val)
        {
            var Env = Where(Name);
            if (Env == null)
            {
                Env = this;
            }

            Env.SetSelf(Name, Val);
        }

        public void Set(Value Name, Value Val)
        {
            if (Name.Type == ValueType.Ident || Name.Type == ValueType.String)
            {
                Set(StringTable.GetString((int)Name.Numeric), Val);
            }
            else
            {
                Logger.DError($"unknown var : {StringTable.GetString((int)Name.Numeric)}");
            }
        }

        public void SetSelf(string Name, Value Val)
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

        public void SetSelf(Value Name, Value Val)
        {
            if (Name.Type == ValueType.Ident || Name.Type == ValueType.String)
            {
                SetSelf(StringTable.GetString((int)Name.Numeric), Val);
            }
            else
            {
                Logger.DError($"unknown var : {StringTable.GetString((int)Name.Numeric)}");
            }
        }

        public Environment Where(string Name)
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