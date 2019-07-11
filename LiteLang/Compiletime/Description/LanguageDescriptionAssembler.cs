using System.Collections.Generic;

namespace LiteLang.Compiletime.Description
{
    public class LanguageDescriptionAssembler : LanguageDescriptionBase
    {
        private readonly HashSet<string> KeywordList_ = new HashSet<string>
        {
            // Keyword
            "Var", "Func", "Param",
            // Instr
            "Mov",
            "Add", "Sub", "Mul", "Div", "Mod", "Exp", "Neg", "Inc", "Dec",
            "And", "Or", "Xor", "Not", "Shl", "Shr",
            "Concat", "GetChar", "SetChar",
            "Jmp", "Je", "Jne", "Jg", "Jl", "Jge", "Jle",
            "Push", "Pop",
            "Call", "Ret", "CallHost",
            "Pause", "Exit",
        };

        private readonly HashSet<char> DelimiterList_ = new HashSet<char>
        {
            ',', ':', ';', '.', '(', ')', '[', ']', '{', '}',
        };

        private readonly HashSet<char> OperatorList_ = new HashSet<char>
        {
            '+', '-', '*', '/', '=', '%', '^', '&', '|', '~', '<', '>', '.', '!', '#',
        };

        private readonly HashSet<string> UnaryOperatorList_ = new HashSet<string>
        {
            "!", "-", "#",
        };

        private readonly HashSet<string> BinaryOperatorList_ = new HashSet<string>
        {
            "+", "-", "*", "/", "=", "%", "^", "&", "|", "~", "<", ">",
            "<=", ">=", "&&", "||", "..", "~=", "==", "<<", ">>"
        };

        /// <summary>
        /// 是否为空白字符
        /// </summary>
        public override bool IsWhitespaceChar(char Ch)
        {
            switch (Ch)
            {
                case ' ':
                case '\t':
                case '\r':
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// 是否为行结束字符
        /// </summary>
        public override bool IsEndOfLineChar(char Ch)
        {
            return Ch == '\n';
        }

        /// <summary>
        /// 是否为数字字符
        /// </summary>
        public override bool IsDigitChar(char Ch)
        {
            return Ch >= '0' && Ch <= '9';
        }

        /// <summary>
        /// 是否为引号字符(定义字符串)
        /// </summary>
        public override bool IsQuoteChar(char Ch)
        {
            return Ch == '"' || Ch == '\'';
        }

        /// <summary>
        /// 是否为有效标识符字符
        /// </summary>
        public override bool IsIdentityChar(char Ch)
        {
            return (Ch >= 'a' && Ch <= 'z') || (Ch >= 'A' && Ch <= 'Z') || Ch == '_';
        }

        /// <summary>
        /// 是否为操作符字符
        /// </summary>
        public override bool IsOperatorChar(char Ch)
        {
            return OperatorList_.Contains(Ch);
        }

        /// <summary>
        /// 是否为分隔符
        /// </summary>
        public override bool IsDelimiterChar(char Ch)
        {
            return DelimiterList_.Contains(Ch);
        }

        /// <summary>
        /// 是否为空类型字符串
        /// </summary>
        public override bool IsNilString(string Value)
        {
            return Value == "nil";
        }

        /// <summary>
        /// 是否为布尔值字符串
        /// </summary>
        public override bool IsBooleanString(string Value)
        {
            return Value == "true" || Value == "false";
        }

        /// <summary>
        /// 是否为关键字字符串
        /// </summary>
        public override bool IsKeywordString(string Value)
        {
            return KeywordList_.Contains(Value);
        }

        /// <summary>
        /// 是否为一元操作符字符串
        /// </summary>
        public override bool IsUnaryOperatorString(string Value)
        {
            return UnaryOperatorList_.Contains(Value);
        }

        /// <summary>
        /// 是否为二元操作符字符串
        /// </summary>
        public override bool IsBinaryOperatorString(string Value)
        {
            return BinaryOperatorList_.Contains(Value);
        }
    }
}