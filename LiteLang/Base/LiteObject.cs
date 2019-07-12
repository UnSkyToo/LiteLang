using LiteLang.Base.Log;

namespace LiteLang.Base
{
    public class LiteObject
    {
        private LiteEnv Env_;

        public LiteObject(LiteEnv Env)
        {
            this.Env_ = Env;
        }

        public LiteEnv GetEnv(string Member)
        {
            var Env = Env_.Where(Member);
            if (Env != null && Env == Env_)
            {
                return Env;
            }

            Logger.DError($"bad access : {Member}");
            return null;
        }

        public void InitObject(IVisitor Visitor, ClassInfo Cls, LiteEnv Env)
        {
            if (Cls.GetBaseClass() != null)
            {
                InitObject(Visitor, Cls.GetBaseClass(), Env);
            }

            Cls.GetBody().Accept(Visitor, Env);
        }

        public LiteValue Read(string Member)
        {
            var Env = GetEnv(Member);
            if (Env != null)
            {
                return Env.Get(Member);
            }
            return LiteValue.Error;
        }

        public LiteValue Write(string Member, LiteValue Val)
        {
            var Env = GetEnv(Member);
            if (Env != null)
            {
                Env.SetSelf(Member, Val);
                return Val;
            }
            return LiteValue.Error;
        }
    }
}