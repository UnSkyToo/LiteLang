using System.Collections.Generic;

namespace LiteLang.Base
{
    public static class ElementsTable
    {
        private static readonly List<Elements> ElementsTable_ = new List<Elements>();
        private static readonly Dictionary<Elements, int> ElementsHash_ = new Dictionary<Elements, int>();

        public static int AddElements(Elements Ele)
        {
            if (ElementsHash_.ContainsKey(Ele))
            {
                return ElementsHash_[Ele];
            }

            ElementsTable_.Add(Ele);
            ElementsHash_.Add(Ele, ElementsTable_.Count - 1);
            return ElementsTable_.Count - 1;
        }

        public static LiteValue AddElementsEx(Elements Ele)
        {
            return new LiteValue(LiteValueType.Elements, AddElements(Ele));
        }

        public static Elements GetElements(int Index)
        {
            if (Index < 0 || Index >= ElementsTable_.Count)
            {
                return null;
            }

            return ElementsTable_[Index];
        }
    }
}