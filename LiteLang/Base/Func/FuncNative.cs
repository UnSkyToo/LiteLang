namespace LiteLang.Base.Func
{
    public delegate bool LiteLangNativeFunc(LiteEnv Env);

    public class FuncNative : FuncBase
    {
        private readonly LiteEnv Env_;
        private readonly LiteLangNativeFunc Func_;

        public FuncNative(string Name, LiteEnv Env, LiteLangNativeFunc Func)
            : base(Name)
        {
            this.Env_ = Env;
            this.Func_ = Func;
        }

        public LiteValue Invoke()
        {
            var HasReturn = Func_.Invoke(Env_);
            var Ret = LiteValue.Nil;
            if (HasReturn)
            {
                Ret = Env_.Pop();
            }

            return Ret;
        }

        public void Push(LiteValue Val)
        {
            Env_.Push(Val);
        }

        public void Push(int Val)
        {
            Env_.Push(new LiteValue(LiteValueType.Numeric, Val));
        }
    }
}