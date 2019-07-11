namespace LiteLang.Compiletime.Description
{
    public interface ILanguageDescription
    {
        /// <summary>
        /// 是否为空白字符
        /// </summary>
        bool IsWhitespaceChar(char Ch);

        /// <summary>
        /// 是否为行结束字符
        /// </summary>
        bool IsEndOfLineChar(char Ch);

        /// <summary>
        /// 是否为数字字符
        /// </summary>
        bool IsDigitChar(char Ch);

        /// <summary>
        /// 是否为引号字符(定义字符串)
        /// </summary>
        bool IsQuoteChar(char Ch);

        /// <summary>
        /// 是否为有效标识符字符
        /// </summary>
        bool IsIdentityChar(char Ch);

        /// <summary>
        /// 是否为操作符字符
        /// </summary>
        bool IsOperatorChar(char Ch);

        /// <summary>
        /// 是否为分隔符
        /// </summary>
        bool IsDelimiterChar(char Ch);

        /// <summary>
        /// 是否为空类型字符串
        /// </summary>
        bool IsNilString(string Value);

        /// <summary>
        /// 是否为布尔值字符串
        /// </summary>
        bool IsBooleanString(string Value);

        /// <summary>
        /// 是否为关键字字符串
        /// </summary>
        bool IsKeywordString(string Value);

        /// <summary>
        /// 是否为一元操作符字符串
        /// </summary>
        bool IsUnaryOperatorString(string Value);

        /// <summary>
        /// 是否为二元操作符字符串
        /// </summary>
        bool IsBinaryOperatorString(string Value);

        /// <summary>
        /// 是否为操作符字符串
        /// </summary>
        bool IsOperatorString(string Value);
    }
}