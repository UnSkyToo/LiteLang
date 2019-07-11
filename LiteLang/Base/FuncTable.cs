using System.Collections.Generic;

namespace LiteLang.Base
{
    public static class FuncTable
    {
        private static readonly List<Function> FuncTable_ = new List<Function>();
        private static readonly Dictionary<Function, int> FuncHash_ = new Dictionary<Function, int>();

        public static int AddFunc(Function Func)
        {
            if (FuncHash_.ContainsKey(Func))
            {
                return FuncHash_[Func];
            }

            FuncTable_.Add(Func);
            FuncHash_.Add(Func, FuncTable_.Count - 1);
            return FuncTable_.Count - 1;
        }

        public static Function GetFunc(int Index)
        {
            if (Index < 0 || Index >= FuncTable_.Count)
            {
                return null;
            }

            return FuncTable_[Index];
        }
    }
}