namespace LiteLang
{
    // ident        ::= {"_"} { "a-z" | "A-Z" | "_" }
    // param        ::= ident
    // params       ::= param { "," param }
    // fn           ::= "fn" ident params block
    // args         ::= expr { "," expr }
    // postfix      ::= "(" [ args ] ")"
    // primary      ::= ( "(" expr ")" | Number | true | false | nil | ident | String ) { postfix }
    // factor       ::= "-" primary | primary
    // expr         ::= factor { Op factor }
    // block        ::= "{" [ statement ] { ( ";" | EOL ) [ statement ] } "}"
    // simple       ::= expr [ args ]
    // statement    ::= "if" expr block [ "else" block ] | "white" expr block | simple
    // program      ::= [ fn | statement ] ( ";" | EOL )

    // Instructions
    // 1. Memory
    // Mov Dst, Src
    // 2. Arithmetic
    // Add Dst, Src
    // Sub Dst, Src
    // Mul Dst, Src
    // Div Dst, Src
    // Mod Dst, Src
    // Exp Dst, Power
    // Neg Dst
    // Inc Dst
    // Dec Dst
    // 3. Bit
    // And Dst, Src
    // Or Dst, Src
    // Xor Dst, Src
    // Not Dst
    // Shl Dst, Count
    // Shr Dst, Count
    // 4. String
    // Concat Str1, Str2
    // GetChar Dst, Src, Index
    // SetChar Index, Dst, Src
    // 5. Condition
    // Jmp Label
    // Je Op1, Op2, Label
    // Jne Op1, Op2, Label
    // Jg Op1, Op2, Label
    // Jl Op1, Op2, Label
    // Jge Op1, Op2, Label
    // Jle Op1, Ope2, Label
    // 6. Stack
    // Push Src,
    // Pop Dst
    // 7. Function
    // Call FuncName
    // Ret
    // CallHost FuncName
    // 8. Other
    // Pause Duration
    // Exit Code

    // SetStackSie 1024
    // Func
    // {
    //    Param Y
    //    Param X
    //    Var Sum
    //    Mov Sum, X
    //    Add Sum, Y
    //    Mov _RetVal, Sum
    // }
    // Var/Var[]


    // OperandType
    // 0 Integer
    // 1 Float
    // 2 String
    // 3 Abstract Index
    // 4 Relative Index
    // 5 Instruction Index
    // 6 Function Index
    // 7 HostApi Index
    // 8 Register
}