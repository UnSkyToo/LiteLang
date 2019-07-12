using System.Collections.Generic;

namespace LiteLang.Base
{
    public static class ObjectTable
    {
        private static readonly List<LiteObject> ObjectTable_ = new List<LiteObject>();
        private static readonly Dictionary<LiteObject, int> ObjectHash_ = new Dictionary<LiteObject, int>();

        public static int AddObject(LiteObject Obj)
        {
            if (ObjectHash_.ContainsKey(Obj))
            {
                return ObjectHash_[Obj];
            }

            ObjectTable_.Add(Obj);
            ObjectHash_.Add(Obj, ObjectTable_.Count - 1);
            return ObjectTable_.Count - 1;
        }

        public static LiteValue AddObjectEx(LiteObject Obj)
        {
            return new LiteValue(LiteValueType.Object, AddObject(Obj));
        }

        public static LiteObject GetObject(int Index)
        {
            if (Index < 0 || Index >= ObjectTable_.Count)
            {
                return null;
            }

            return ObjectTable_[Index];
        }
    }
}