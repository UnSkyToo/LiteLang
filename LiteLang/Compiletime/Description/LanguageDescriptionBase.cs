namespace LiteLang.Compiletime.Description
{
    public abstract class LanguageDescriptionBase : ILanguageDescription
    {
        /// <summary>
        /// 是否为空白字符
        /// </summary>
        public abstract bool IsWhitespaceChar(char Ch);

        /// <summary>
        /// 是否为行结束字符
        /// </summary>
        public abstract bool IsEndOfLineChar(char Ch);

        /// <summary>
        /// 是否为数字字符
        /// </summary>
        public abstract bool IsDigitChar(char Ch);

        /// <summary>
        /// 是否为引号字符(定义字符串)
        /// </summary>
        public abstract bool IsQuoteChar(char Ch);

        /// <summary>
        /// 是否为有效标识符字符
        /// </summary>
        public abstract bool IsIdentityChar(char Ch);

        /// <summary>
        /// 是否为操作符字符
        /// </summary>
        public abstract bool IsOperatorChar(char Ch);

        /// <summary>
        /// 是否为分隔符
        /// </summary>
        public abstract bool IsDelimiterChar(char Ch);

        /// <summary>
        /// 是否为空类型字符串
        /// </summary>
        public abstract bool IsNilString(string Value);

        /// <summary>
        /// 是否为布尔值字符串
        /// </summary>
        public abstract bool IsBooleanString(string Value);

        /// <summary>
        /// 是否为关键字字符串
        /// </summary>
        public abstract bool IsKeywordString(string Value);

        /// <summary>
        /// 是否为一元操作符字符串
        /// </summary>
        public abstract bool IsUnaryOperatorString(string Value);

        /// <summary>
        /// 是否为二元操作符字符串
        /// </summary>
        public abstract bool IsBinaryOperatorString(string Value);

        /// <summary>
        /// 是否为操作符字符串
        /// </summary>
        public bool IsOperatorString(string Value)
        {
            return IsUnaryOperatorString(Value) || IsBinaryOperatorString(Value);
        }
    }
}