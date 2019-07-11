using LiteLang.Base;

namespace LiteLang.Compiletime.Analyzer
{
    public interface IAnalyzer
    {
        ExitCode Analyzing();
    }
}