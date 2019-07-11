namespace LiteLang.Base
{
    public enum TokenType : byte
    {
        Error,
        None,
        Nil,
        Boolean,
        Numeric,
        String,
        Keyword,
        Identifier,
        Delimiter,
        Operator,
    }

    public struct Token
    {
        public static readonly Token Error = new Token(TokenType.Error, string.Empty, 0);

        public TokenType Type { get; }
        public string Code { get; }
        public int Line { get; }

        public Token(TokenType Type, string Code)
        {
            this.Type = Type;
            this.Code = Code;
            this.Line = 0;
        }

        public Token(TokenType Type, string Code, int Line)
        {
            this.Type = Type;
            this.Code = Code;
            this.Line = Line;
        }

        public override string ToString()
        {
            return $"{Type} {Code}:{Line}";
        }
    }
}