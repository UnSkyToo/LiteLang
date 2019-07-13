using LiteLang.Base.Log;

namespace LiteLang.Base
{
    public class Elements
    {
        private readonly LiteValue[] Values_;

        public Elements(int Count)
        {
            Values_ = new LiteValue[Count];
            for (var Index = 0; Index < Values_.Length; ++Index)
            {
                Values_[Index] = LiteValue.Nil;
            }
        }

        public Elements(LiteValue[] Values)
        {
            Values_ = new LiteValue[Values.Length];
            for (var Index = 0; Index < Values_.Length; ++Index)
            {
                Values_[Index] = Values[Index];
            }
        }

        public LiteValue Set(int Index, LiteValue Val)
        {
            if (Index < 0 || Index >= Values_.Length)
            {
                Logger.DError($"index [{Index}] out of range, max len = {Values_.Length}");
                return LiteValue.Error;
            }

            Values_[Index] = Val;
            return Val;
        }

        public LiteValue Get(int Index)
        {
            if (Index < 0 || Index >= Values_.Length)
            {
                Logger.DError($"index [{Index}] out of range, max len = {Values_.Length}");
                return LiteValue.Error;
            }

            return Values_[Index];
        }
    }
}