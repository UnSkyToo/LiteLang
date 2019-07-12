using System.Collections.Generic;

namespace LiteLang.Base
{
    public static class ClassTable
    {
        private static readonly List<ClassInfo> ClassTable_ = new List<ClassInfo>();
        private static readonly Dictionary<ClassInfo, int> ClassHash_ = new Dictionary<ClassInfo, int>();

        public static int AddClass(ClassInfo Cls)
        {
            if (ClassHash_.ContainsKey(Cls))
            {
                return ClassHash_[Cls];
            }

            ClassTable_.Add(Cls);
            ClassHash_.Add(Cls, ClassTable_.Count - 1);
            return ClassTable_.Count - 1;
        }

        public static LiteValue AddClassEx(ClassInfo Cls)
        {
            return new LiteValue(LiteValueType.Class, AddClass(Cls));
        }

        public static ClassInfo GetClass(int Index)
        {
            if (Index < 0 || Index >= ClassTable_.Count)
            {
                return null;
            }

            return ClassTable_[Index];
        }
    }
}
