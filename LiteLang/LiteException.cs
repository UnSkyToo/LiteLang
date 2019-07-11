using System;

namespace LiteLang
{
    public class LiteException : Exception
    {
        public LiteException()
        {
        }

        public LiteException(string Message)
            : base(Message)
        {
        }

        public LiteException(string Message, Exception InnerException)
            : base(Message, InnerException)
        {
        }
    }

    public class LiteUnexpectedException<T> : LiteException
    {
        public LiteUnexpectedException(T Value)
            : base($"Unexpected {typeof(T).Name} : {Value}")
        {
        }
    }
}