using System.Runtime.InteropServices;

namespace LiteLang.Compiletime.Assembler.Base
{
    [StructLayout(LayoutKind.Explicit)]
    public class Operand
    {
        [FieldOffset(0)] public int Type;
        // Union Value
        [FieldOffset(4)] public int IntLiteral;
        [FieldOffset(4)] public float FloatLiteral;
        [FieldOffset(4)] public int StringTableIndex;
        [FieldOffset(4)] public int StackIndex;
        [FieldOffset(4)] public int InstrIndex;
        [FieldOffset(4)] public int FuncIndex;
        [FieldOffset(4)] public int HostApiIndex;
        [FieldOffset(4)] public int Reg;
        //
        [FieldOffset(8)] public int OffsetIndex;
    }
}