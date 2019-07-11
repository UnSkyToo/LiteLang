using System.Collections.Generic;

namespace LiteLang.Compiletime.Assembler.Base
{
    public class Instruct
    {
        public int OpCode;
        public int OpCount;
        public List<Operand> OpList;
    }
}