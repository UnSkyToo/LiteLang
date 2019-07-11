using System;
using System.IO;
using LiteLang.Base.Log;
using LiteLang.Base.Stream;
using LiteLang.Compiletime.Analyzer;
using LiteLang.Compiletime.Description;
using LiteLang.Runtime;

namespace LiteLang
{
    internal class Program
    {
        private static void Main(string[] Args)
        {
            var txt = File.ReadAllText("asm.txt");
            var c = new CharacterStream(txt);
            var l = new LexicalAnalyzer(c, new LanguageDescriptionLite());
            var e = l.Analyzing();
            if (e.IsError())
            {
                Logger.DError(e.Msg);
            }
            else
            {
                //l.Display();
            }

            var s = new SyntaxAnalyzer(l.GetTokenStream());
            e = s.Analyzing();
            if (e.IsError())
            {
                Logger.DError(e.Msg);
            }
            else
            {
                new Evaluator().Eval(s.GetProgramNode());
            }

            Console.ReadLine();

            /*var Eval = new Evaluator();
            while (true)
            {
                Console.Write(">");
                var Line = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(Line))
                {
                    break;
                }

                var l = new LexicalAnalyzer(new CharacterStream(Line), new LanguageDescriptionLite());
                var e = l.Analyzing();
                if (e.IsError())
                {
                    Logger.DError(e.Msg);
                }
                else
                {
                    l.Display();
                }

                var s = new SyntaxAnalyzer(l.GetTokenStream());
                e = s.Analyzing();
                if (e.IsError())
                {
                    Logger.DError(e.Msg);
                }
                else
                {
                    Eval.Eval(s.GetProgramNode());
                }
            }

            Logger.DInfo("Done");
            Console.ReadLine();*/
        }
    }
}