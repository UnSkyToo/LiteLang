using System.Collections.Generic;

namespace LiteLang.Base
{
    public static class StringTable
    {
        private static readonly List<string> StringTable_ = new List<string>();
        private static readonly Dictionary<string, int> StringHash_ = new Dictionary<string, int>();

        public static int AddString(string Str)
        {
            if (StringHash_.ContainsKey(Str))
            {
                return StringHash_[Str];
            }

            StringTable_.Add(Str);
            StringHash_.Add(Str, StringTable_.Count - 1);
            return StringTable_.Count - 1;
        }

        public static string GetString(int Index)
        {
            if (Index < 0 || Index >= StringTable_.Count)
            {
                return null;
            }

            return StringTable_[Index];
        }
    }
}