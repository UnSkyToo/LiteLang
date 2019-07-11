namespace LiteLang.Base.Stream
{
    public class TokenStream : StreamStableBase<Token>
    {
        public TokenStream(Token[] Tokens)
        {
            Buffer_ = Tokens;
            Index_ = 0;
        }

        public bool TakeExpect(TokenType TokType, out Token Tok)
        {
            Tok = Take();
            if (Tok.Type != TokType)
            {
                return false;
            }
            return true;
        }

        public bool TakeExpect(TokenType TokType, string TokCode, out Token Tok)
        {
            Tok = Take();
            if (Tok.Type != TokType || Tok.Code != TokCode)
            {
                return false;
            }
            return true;
        }

        public bool PeekExpect(TokenType TokType)
        {
            var Next = Peek();
            if (Next.Type != TokType)
            {
                return false;
            }
            return true;
        }

        public bool PeekExpect(TokenType TokType, string TokCode)
        {
            var Next = Peek();
            if (Next.Type != TokType || Next.Code != TokCode)
            {
                return false;
            }
            return true;
        }
    }
}