using System;
using System.Runtime.InteropServices;

namespace LiteLang.Base
{
    public enum ValueType : byte
    {
        Nil,
        Boolean,
        Numeric,
        String,
        Ident,
        Function,
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct Value
    {
        private const double Ep = 0.00000001d;

        public static readonly Value Nil = new Value(ValueType.Nil, 0);
        public static readonly Value True = new Value(ValueType.Boolean, 1);
        public static readonly Value False = new Value(ValueType.Boolean, 0);

        [FieldOffset(0)] public ValueType Type;
        [FieldOffset(1)] public double Numeric;

        public Value(ValueType Type, double Numeric)
        {
            this.Type = Type;
            this.Numeric = Numeric;
        }

        public bool IsZero()
        {
            return Math.Abs(Numeric) <= Ep;
        }

        public override string ToString()
        {
            switch (Type)
            {
                case ValueType.Nil:
                    return "nil";
                case ValueType.Boolean:
                    return IsZero() ? "false" : "true";
                case ValueType.Numeric:
                    return Numeric.ToString();
                case ValueType.String:
                case ValueType.Ident:
                    return "string";
                case ValueType.Function:
                    return $"<fn:{(int)Numeric}>";
                default:
                    return "unknown";
            }
        }

        public override bool Equals(object Obj)
        {
            if (object.Equals(Obj, null))
            {
                return false;
            }

            if (Obj.GetType() != typeof(Value))
            {
                return false;
            }

            var Val = (Value)Obj;
            return Equals(Val);
        }

        public bool Equals(Value Other)
        {
            if (Type != Other.Type)
            {
                return false;
            }

            switch (Type)
            {
                case ValueType.Nil:
                    return true;
                case ValueType.Boolean:
                case ValueType.Function:
                case ValueType.String:
                case ValueType.Ident:
                case ValueType.Numeric:
                    return Math.Abs(Numeric - Other.Numeric) <= Ep;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        public static bool operator ==(Value Left, Value Right)
        {
            return Left.Equals(Right);
        }

        public static bool operator !=(Value Left, Value Right)
        {
            return !Left.Equals(Right);
        }

        public static Value operator +(Value Left, Value Right)
        {
            return new Value(ValueType.Numeric, Left.Numeric + Right.Numeric);
        }

        public static Value operator -(Value Left, Value Right)
        {
            return new Value(ValueType.Numeric, Left.Numeric - Right.Numeric);
        }

        public static Value operator *(Value Left, Value Right)
        {
            return new Value(ValueType.Numeric, Left.Numeric * Right.Numeric);
        }

        public static Value operator /(Value Left, Value Right)
        {
            return new Value(ValueType.Numeric, Left.Numeric / Right.Numeric);
        }

        public static Value operator %(Value Left, Value Right)
        {
            return new Value(ValueType.Numeric, Left.Numeric % Right.Numeric);
        }

        public static bool operator <(Value Left, Value Right)
        {
            return Left.Numeric < Right.Numeric;
        }

        public static bool operator <=(Value Left, Value Right)
        {
            return Left.Numeric <= Right.Numeric;
        }

        public static bool operator >(Value Left, Value Right)
        {
            return Left.Numeric > Right.Numeric;
        }

        public static bool operator >=(Value Left, Value Right)
        {
            return Left.Numeric >= Right.Numeric;
        }
    }

    /*[StructLayout(LayoutKind.Explicit)]
    public class Value
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
    }*/
}