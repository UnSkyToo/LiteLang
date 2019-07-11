using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using LiteLang.Base;

namespace LiteLang
{
    public class Asm
    {
        public class ScriptHeader
        {
            public int StackSize;
            public int GlobalDataSize;
            public int IsMainFuncPresent;
            public int MainFuncIndex;
        }

        public class FuncNode
        {
            public int Index;
            public string Name;
            public int EntryPoint;
            public int ParamCount;
            public int LocalDataSize;
        }

        public class SymbolNode
        {
            public int Index;
            public string Ident;
            public int Size;
            public int StackIndex;
            public int FuncIndex;
        }

        public class LabelNode
        {
            public int Index;
            public string Ident;
            public int TargetIndex;
            public int FuncIndex;
        }

        [Flags]
        public enum OpType : byte
        {
            Int = 1 << 0,
            Float = 1 << 1,
            String = 1 << 2,
            MemRef = 1 << 3,
            LineLabel = 1 << 4,
            FuncName = 1 << 5,
            HostApiCall = 1 << 6,
            Reg = 1 << 7,
        }

        public class InstrLookup
        {
            public string Mnemonic;
            public int OpCode;
            public int OpCount;
            public List<int> OpList = new List<int>();
        }

        private string[] SourceCode_;
        private int SourceCodeSize_;

        private List<InstrLookup> InstrTable_;
        //private Instr[] InstrStream_;
        private int InstrStreamSize_;

        private ScriptHeader ScriptHeader_;

        private List<string> StringTable_;
        private List<FuncNode> FuncTable_;
        private List<SymbolNode> SymbolTable_;
        private List<LabelNode> LabelTable_;
        private List<string> HostApiTable_;

        public Asm()
        {
            InstrTable_ = new List<InstrLookup>();

            ScriptHeader_ = new ScriptHeader();

            StringTable_ = new List<string>();
            FuncTable_ = new List<FuncNode>();
            SymbolTable_ = new List<SymbolNode>();
            LabelTable_ = new List<LabelNode>();
            HostApiTable_ = new List<string>();

            var Index = AddInstrLookup("Mov", 0, 2);
            SetOpType(Index, 0, (int)(OpType.MemRef | OpType.Reg));
            SetOpType(Index, 1, (int)(OpType.Int | OpType.Float | OpType.String | OpType.MemRef | OpType.Reg));
        }

        public int AddString(string Value)
        {
            for (var Index = 0; Index < StringTable_.Count; ++Index)
            {
                if (StringTable_[Index] == Value)
                {
                    return Index;
                }
            }

            StringTable_.Add(Value);
            return StringTable_.Count - 1;
        }

        public FuncNode GetFuncByName(string Name)
        {
            if (FuncTable_.Count == 0)
            {
                return null;
            }

            for (var Index = 0; Index < FuncTable_.Count; ++Index)
            {
                if (FuncTable_[Index].Name == Name)
                {
                    return FuncTable_[Index];
                }
            }

            return null;
        }

        public void SetFuncInfo(string Name, int ParamCount, int LocalDataSize)
        {
            var Func = GetFuncByName(Name);
            Func.ParamCount = ParamCount;
            Func.LocalDataSize = LocalDataSize;
        }

        public int AddFunc(string Name, int EntryPoint)
        {
            if (GetFuncByName(Name) != null)
            {
                return -1;
            }

            var Func = new FuncNode();
            Func.Name = Name;
            Func.EntryPoint = EntryPoint;
            FuncTable_.Add(Func);
            var Index = FuncTable_.Count - 1;
            Func.Index = Index;
            
            return Index;
        }

        public SymbolNode GetSymbolByIdent(string Ident, int FuncIndex)
        {
            for (var Index = 0; Index < SymbolTable_.Count; ++Index)
            {
                if (SymbolTable_[Index].Ident == Ident)
                {
                    if (SymbolTable_[Index].FuncIndex == FuncIndex || SymbolTable_[Index].StackIndex >= 0)
                    {
                        return SymbolTable_[Index];
                    }
                }
            }

            return null;
        }

        public int AddSymbol(string Ident, int Size, int StackIndex, int FuncIndex)
        {
            if (GetSymbolByIdent(Ident, FuncIndex) != null)
            {
                return -1;
            }

            var Symbol = new SymbolNode();
            Symbol.Ident = Ident;
            Symbol.Size = Size;
            Symbol.StackIndex = StackIndex;
            Symbol.FuncIndex = FuncIndex;
            SymbolTable_.Add(Symbol);
            var Index = SymbolTable_.Count - 1;
            Symbol.Index = Index;
            return Index;
        }

        public int GetStackIndexByIdent(string Ident, int FuncIndex)
        {
            var Symbol = GetSymbolByIdent(Ident, FuncIndex);
            return Symbol.StackIndex;
        }

        public int GetSizeByIdent(string Ident, int FuncIndex)
        {
            var Symbol = GetSymbolByIdent(Ident, FuncIndex);
            return Symbol.Size;
        }

        public LabelNode GetLabelByIdent(string Ident, int FuncIndex)
        {
            if (LabelTable_.Count == 0)
            {
                return null;
            }

            for (var Index = 0; Index < LabelTable_.Count; ++Index)
            {
                if (LabelTable_[Index].Ident == Ident && LabelTable_[Index].FuncIndex == FuncIndex)
                {
                    return LabelTable_[Index];
                }
            }

            return null;
        }

        public int AddLabel(string Ident, int TargetIndex, int FuncIndex)
        {
            if (GetLabelByIdent(Ident, FuncIndex) != null)
            {
                return -1;
            }

            var Label = new LabelNode();
            Label.Ident = Ident;
            Label.TargetIndex = TargetIndex;
            Label.FuncIndex = FuncIndex;
            LabelTable_.Add(Label);
            var Index = LabelTable_.Count - 1;
            Label.Index = Index;
            return Index;
        }

        public int AddInstrLookup(string Mnemonic, int OpCode, int OpCount)
        {
            var Instr = new InstrLookup();
            Instr.Mnemonic = Mnemonic;
            Instr.OpCode = OpCode;
            Instr.OpCount = OpCount;
            InstrTable_.Add(Instr);
            return InstrTable_.Count - 1;
        }

        public void SetOpType(int InstrIndex, int OpIndex, int OpType)
        {
            InstrTable_[InstrIndex].OpList[OpIndex] = OpType;
        }

        public bool GetInstrByMnemonic(string Mnemonic, ref InstrLookup Instr)
        {
            for (var Index = 0; Index < InstrTable_.Count; ++Index)
            {
                if (InstrTable_[Index].Mnemonic == Mnemonic)
                {
                    Instr = InstrTable_[Index];
                    return true;
                }
            }

            return false;
        }
    }
}