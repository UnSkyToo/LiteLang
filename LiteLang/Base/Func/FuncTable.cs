using System.Collections.Generic;

namespace LiteLang.Base.Func
{
    public static class FuncTable
    {
        private static readonly List<FuncBase> FuncTable_ = new List<FuncBase>();
        private static readonly Dictionary<FuncBase, int> FuncHash_ = new Dictionary<FuncBase, int>();

        public static int AddFunc(FuncBase Func)
        {
            if (FuncHash_.ContainsKey(Func))
            {
                return FuncHash_[Func];
            }

            FuncTable_.Add(Func);
            FuncHash_.Add(Func, FuncTable_.Count - 1);
            return FuncTable_.Count - 1;
        }

        public static LiteValue AddFuncEx(FuncBase Func)
        {
            return new LiteValue(LiteValueType.Function, AddFunc(Func));
        }

        public static FuncBase GetFunc(int Index)
        {
            if (Index < 0 || Index >= FuncTable_.Count)
            {
                return null;
            }

            return FuncTable_[Index];
        }
    }
}