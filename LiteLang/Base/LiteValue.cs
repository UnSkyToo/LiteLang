using System;
using System.Runtime.InteropServices;
using LiteLang.Base.Log;

namespace LiteLang.Base
{
    public enum LiteValueType : byte
    {
        Error,
        Nil,
        Boolean,
        Numeric,
        String,
        Function,
        Class,
        Object,
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct LiteValue
    {
        private const double Ep = 0.00000001d;

        public static readonly LiteValue Error = new LiteValue(LiteValueType.Error, 0);
        public static readonly LiteValue Nil = new LiteValue(LiteValueType.Nil, 0);
        public static readonly LiteValue True = new LiteValue(LiteValueType.Boolean, 1);
        public static readonly LiteValue False = new LiteValue(LiteValueType.Boolean, 0);

        [FieldOffset(0)] public LiteValueType Type;
        [FieldOffset(1)] public double Numeric;

        public LiteValue(LiteValueType Type, double Numeric)
        {
            this.Type = Type;
            this.Numeric = Numeric;
        }

        public bool IsZero()
        {
            return Math.Abs(Numeric) <= Ep;
        }

        public bool IsError()
        {
            return Type == LiteValueType.Error;
        }

        public override string ToString()
        {
            switch (Type)
            {
                case LiteValueType.Error:
                    return "error";
                case LiteValueType.Nil:
                    return "nil";
                case LiteValueType.Boolean:
                    return IsZero() ? "false" : "true";
                case LiteValueType.Numeric:
                    return Numeric.ToString();
                case LiteValueType.String:
                    return $"{StringTable.GetString((int)Numeric)}";
                case LiteValueType.Function:
                    return $"<fn:{(int)Numeric}>";
                case LiteValueType.Class:
                    return $"<class:{(int)Numeric}>";
                case LiteValueType.Object:
                    return $"<object:{(int)Numeric}>";
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

            if (Obj.GetType() != typeof(LiteValue))
            {
                return false;
            }

            var Val = (LiteValue)Obj;
            return Equals(Val);
        }

        public bool Equals(LiteValue Other)
        {
            if (Type != Other.Type)
            {
                return false;
            }

            switch (Type)
            {
                case LiteValueType.Error:
                    return false;
                case LiteValueType.Nil:
                    return true;
                case LiteValueType.Boolean:
                case LiteValueType.String:
                case LiteValueType.Numeric:
                case LiteValueType.Function:
                case LiteValueType.Class:
                case LiteValueType.Object:
                    return Math.Abs(Numeric - Other.Numeric) <= Ep;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        public static bool operator ==(LiteValue Left, LiteValue Right)
        {
            return Left.Equals(Right);
        }

        public static bool operator !=(LiteValue Left, LiteValue Right)
        {
            return !Left.Equals(Right);
        }

        public static LiteValue operator +(LiteValue Left, LiteValue Right)
        {
            if (Left.Type == LiteValueType.String || Right.Type == LiteValueType.String)
            {
                return StringTable.AddStringEx($"{Left}{Right}");
            }

            return new LiteValue(LiteValueType.Numeric, Left.Numeric + Right.Numeric);
        }

        public static LiteValue operator -(LiteValue Left, LiteValue Right)
        {
            return new LiteValue(LiteValueType.Numeric, Left.Numeric - Right.Numeric);
        }

        public static LiteValue operator *(LiteValue Left, LiteValue Right)
        {
            return new LiteValue(LiteValueType.Numeric, Left.Numeric * Right.Numeric);
        }

        public static LiteValue operator /(LiteValue Left, LiteValue Right)
        {
            return new LiteValue(LiteValueType.Numeric, Left.Numeric / Right.Numeric);
        }

        public static LiteValue operator %(LiteValue Left, LiteValue Right)
        {
            return new LiteValue(LiteValueType.Numeric, Left.Numeric % Right.Numeric);
        }

        public static bool operator <(LiteValue Left, LiteValue Right)
        {
            return Left.Numeric < Right.Numeric;
        }

        public static bool operator <=(LiteValue Left, LiteValue Right)
        {
            return Left.Numeric <= Right.Numeric;
        }

        public static bool operator >(LiteValue Left, LiteValue Right)
        {
            return Left.Numeric > Right.Numeric;
        }

        public static bool operator >=(LiteValue Left, LiteValue Right)
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