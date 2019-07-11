namespace LiteLang.Base.Stream
{
    public class CharacterStream : StreamStableBase<char>
    {
        public CharacterStream(string Source)
        {
            if (!Source.EndsWith('\n'))
            {
                Source += '\n';
            }

            Buffer_ = Source.ToCharArray();
            Index_ = 0;
        }
    }
}