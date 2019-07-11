namespace LiteLang.Base
{
    public enum ExitCodeType : byte
    {
        Successful = 0,
        Failed = 1,
        UnexpectedSymbol = 2,
    }

    public class ExitCode
    {
        public static readonly ExitCode Successful = new ExitCode(ExitCodeType.Successful, string.Empty);

        public ExitCodeType Code { get; }
        public string Msg { get; }

        public ExitCode(ExitCodeType Code, string Msg)
        {
            this.Code = Code;
            this.Msg = Msg;
        }

        public bool IsError()
        {
            return Code != ExitCodeType.Successful;
        }
    }

    public class ExitFailedCode : ExitCode
    {
        public ExitFailedCode(string Format, params object[] Args)
            : base(ExitCodeType.Failed, string.Format(Format, Args))
        {
        }
    }

    public class ExitUnexpectedSymbolCode : ExitCode
    {
        public ExitUnexpectedSymbolCode(Token Tok)
            : base(ExitCodeType.UnexpectedSymbol,
                $"line {Tok.Line}: unexpected symbol near '{Tok.Code}'")
        {
        }
    }
}