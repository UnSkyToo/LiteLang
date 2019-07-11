namespace LiteLang.Base.Stream
{
    public abstract class StreamStableBase<T>
    {
        protected T[] Buffer_ = null;
        protected int Index_ = 0;

        protected StreamStableBase()
        {
        }

        public bool IsEnd()
        {
            return Index_ >= Buffer_.Length;
        }

        public void Reset()
        {
            Index_ = 0;
        }

        public void Back()
        {
            if (Index_ > 0)
            {
                Index_--;
            }
        }

        public T Take()
        {
            return Index_ < Buffer_.Length ? Buffer_[Index_++] : default;
        }

        public T Peek()
        {
            return Index_ < Buffer_.Length ? Buffer_[Index_] : default;
        }
    }
}