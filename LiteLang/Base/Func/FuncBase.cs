namespace LiteLang.Base.Func
{
    public abstract class FuncBase
    {
        public string Name { get; }

        protected FuncBase(string Name)
        {
            this.Name = Name;
        }
    }
}