using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mbNES
{
    public partial class CPU
    {

        public void ExecuteInstruction()
        {
            ReadOpcode();



            switch (currentOpcode)
            {
                // Group 1 instructions - 8 addressing modes
                // ADC AND CMP EOR LDA ORA SBC STA

                //
                //  ADC - Add with Carry
                //
                case 0x69:      // ADC - Immediate
                    AddressingMode_Immediate();
                    ADC();
                    break;
                case 0x65:      // ADC - Zero page
                    AddressingMode_ZeroPage();
                    ADC();
                    break;
                case 0x75:      // ADC - Zero page, X
                    register = x;
                    AddressingMode_IndexedZeroPage(ref register);
                    ADC();
                    break;
                case 0x6D:      // ADC - Absolute
                    AddressingMode_Absolute();
                    ADC();
                    break;
                case 0x7D:      // ADC - Absolute, X
                    register = x;
                    AddressingMode_IndexedAbsolute(ref register);
                    ADC();
                    break;
                case 0x79:      // ADC - Absolute, Y
                    register = y;
                    AddressingMode_IndexedAbsolute(ref register);
                    ADC();
                    break;
                case 0x61:      // ADC - Indirect, X
                    AddressingMode_IndirectX();
                    ADC();
                    break;
                case 0x71:      // ADC - Indirect, Y
                    AddressingMode_IndirectY();
                    ADC();
                    break;

                //
                //  AND - Logical AND
                //  0x29 0x25 0x35 0x2D 0x3D 0x39 0x21 0x31
                case 0x29:
                    // AND - Immediate
                    AddressingMode_Immediate();
                    AND();
                    break;
                case 0x25:
                    // AND - Zero page
                    AddressingMode_ZeroPage();
                    AND();
                    break;
                case 0x35:
                    // AND - Zero page, X
                    register = x;
                    AddressingMode_IndexedZeroPage(ref register);
                    AND();
                    break;
                case 0x2D:
                    // AND - Absolute
                    AddressingMode_Absolute();
                    AND();
                    break;
                case 0x3D:
                    // AND - Absolute, X
                    register = x;
                    AddressingMode_IndexedAbsolute(ref register);
                    AND();
                    break;
                case 0x39:
                    // AND - Absolute, Y
                    register = y;
                    AddressingMode_IndexedAbsolute(ref register);
                    AND();
                    break;
                case 0x21:
                    // AND - Indirect, X
                    AddressingMode_IndirectX();
                    AND();
                    break;
                case 0x31:
                    // AND - Indirect, Y
                    AddressingMode_IndirectY();
                    AND();
                    break;

                //
                //  CMP - Compare
                //  C9 C5 D5 CD DD D9 C1 D1
                case 0xC9:
                    // CMP - Immediate
                    AddressingMode_Immediate();
                    CMP();
                    break;
                case 0xC5:
                    // CMP - Zero page
                    AddressingMode_ZeroPage();
                    CMP();
                    break;
                case 0xD5:
                    // CMP - Zero page, X
                    register = x;
                    AddressingMode_IndexedZeroPage(ref register);
                    CMP();
                    break;
                case 0xCD:
                    // CMP - Absolute
                    AddressingMode_Absolute();
                    CMP();
                    break;
                case 0xDD:
                    // CMP - Absolute, X
                    register = x;
                    AddressingMode_IndexedAbsolute(ref register);
                    CMP();
                    break;
                case 0xD9:
                    // CMP - Absolute, Y
                    register = y;
                    AddressingMode_IndexedAbsolute(ref register);
                    CMP();
                    break;
                case 0xC1:
                    // CMP - Indirect, X
                    AddressingMode_IndirectX();
                    CMP();
                    break;
                case 0xD1:
                    // CMP - Indirect, Y
                    AddressingMode_IndirectY();
                    CMP();
                    break;

                //
                //  EOR - Exclusive OR
                //  49 45 55 4d 5d 59 41 51
                case 0x49:
                    // EOR - Immediate
                    AddressingMode_Immediate();
                    EOR();
                    break;
                case 0x45:
                    // EOR - Zero page
                    AddressingMode_ZeroPage();
                    EOR();
                    break;
                case 0x55:
                    // EOR - Zero page, X
                    register = x;
                    AddressingMode_IndexedZeroPage(ref register);
                    EOR();
                    break;
                case 0x4D:
                    // EOR - Absolute
                    AddressingMode_Absolute();
                    EOR();
                    break;
                case 0x5D:
                    // EOR - Absolute, X
                    register = x;
                    AddressingMode_IndexedAbsolute(ref register);
                    EOR();
                    break;
                case 0x59:
                    // EOR - Absolute, Y
                    register = y;
                    AddressingMode_IndexedAbsolute(ref register);
                    EOR();
                    break;
                case 0x41:
                    // EOR - Indirect, X
                    AddressingMode_IndirectX();
                    EOR();
                    break;
                case 0x51:
                    // EOR - Indirect, Y
                    AddressingMode_IndirectY();
                    EOR();
                    break;

                //
                //  LDA - Load Accumulator
                //  A9 A5 B5 AD BD B9 A1 B1
                case 0xA9:
                    // LDA - Immediate
                    AddressingMode_Immediate();
                    LDA();
                    break;
                case 0xA5:
                    // LDA - Zero page
                    AddressingMode_ZeroPage();
                    LDA();
                    break;
                case 0xB5:
                    // LDA - Zero page, X
                    register = x;
                    AddressingMode_IndexedZeroPage(ref register);
                    LDA();
                    break;
                case 0xAD:
                    // LDA - Absolute
                    AddressingMode_Absolute();
                    LDA();
                    break;
                case 0xBD:
                    // LDA - Absolute, X
                    register = x;
                    AddressingMode_IndexedAbsolute(ref register);
                    LDA();
                    break;
                case 0xB9:
                    // LDA - Absolute, Y
                    register = y;
                    AddressingMode_IndexedAbsolute(ref register);
                    LDA();
                    break;
                case 0xA1:
                    // LDA - Indirect, X
                    AddressingMode_IndirectX();
                    LDA();
                    break;
                case 0xB1:
                    // LDA - Indirect, Y
                    AddressingMode_IndirectY();
                    LDA();
                    break;

                //
                //  ORA - Logical Inclusive OR
                //  09 05 15 0d 1d 19 01 11
                case 0x09:
                    // ORA - Immediate
                    AddressingMode_Immediate();
                    ORA();
                    break;
                case 0x05:
                    // ORA - Zero page
                    AddressingMode_ZeroPage();
                    ORA();
                    break;
                case 0x15:
                    // ORA - Zero page, X
                    register = x;
                    AddressingMode_IndexedZeroPage(ref register);
                    ORA();
                    break;
                case 0x0D:
                    // ORA - Absolute
                    AddressingMode_Absolute();
                    ORA();
                    break;
                case 0x1D:
                    // ORA - Absolute, X
                    register = x;
                    AddressingMode_IndexedAbsolute(ref register);
                    ORA();
                    break;
                case 0x19:
                    // ORA - Absolute, Y
                    register = y;
                    AddressingMode_IndexedAbsolute(ref register);
                    ORA();
                    break;
                case 0x01:
                    // ORA - Indirect, X
                    AddressingMode_IndirectX();
                    ORA();
                    break;
                case 0x11:
                    // ORA - Indirect, Y
                    AddressingMode_IndirectY();
                    ORA();
                    break;

                //
                //  SBC 
                //  E9 E5 F5 ED FD F9 E1 F1

                case 0xE9:
                    // SBC - Immediate
                    AddressingMode_Immediate();
                    SBC();
                    break;
                case 0xE5:
                    // SBC - Zero page
                    AddressingMode_ZeroPage();
                    SBC();
                    break;
                case 0xF5:
                    // SBC - Zero page, X
                    register = x;
                    AddressingMode_IndexedZeroPage(ref register);
                    SBC();
                    break;
                case 0xED:
                    // SBC - Absolute
                    AddressingMode_Absolute();
                    SBC();
                    break;
                case 0xFD:
                    // SBC - Absolute, X
                    register = x;
                    AddressingMode_IndexedAbsolute(ref register);
                    SBC();
                    break;
                case 0xF9:
                    // SBC - Absolute, Y
                    register = y;
                    AddressingMode_IndexedAbsolute(ref register);
                    SBC();
                    break;
                case 0xE1:
                    // SBC - Indirect, X
                    AddressingMode_IndirectX();
                    SBC();
                    break;
                case 0xF1:
                    // SBC - Indirect, Y
                    AddressingMode_IndirectY();
                    SBC();
                    break;

                //
                //  STA
                //  85 95 8D 9D 99 81 91
                case 0x85:
                    // STA - Zero page
                    AddressingMode_ZeroPage();
                    STA();
                    break;
                case 0x95:
                    // STA - Zero page, X
                    register = x;
                    AddressingMode_IndexedZeroPage(ref register);
                    STA();
                    break;
                case 0x8D:
                    // STA - Absolute
                    AddressingMode_Absolute();
                    STA();
                    break;
                case 0x9D:
                    // STA - Absolute, X
                    register = x;
                    AddressingMode_IndexedAbsolute(ref register);
                    STA();
                    break;
                case 0x99:
                    // STA - Absolute, Y
                    register = y;
                    AddressingMode_IndexedAbsolute(ref register);
                    STA();
                    break;
                case 0x81:
                    // STA - Indirect, X
                    AddressingMode_IndirectX();
                    STA();
                    break;
                case 0x91:
                    // STA - Indirect, Y
                    AddressingMode_IndirectY();
                    STA();
                    break;

                //  Group 2 instructions
                //  LSR ASL ROL ROR                                 
                //  INC DEC LDX STX TAX TXS TSX DEX

                //
                //  LSR
                //
                case 0x4A:
                    // LSR - Accumulator
                    AddressingMode_Accumulator();
                    LSR();
                    break;
                case 0x46:
                    // LSR - Zero page
                    AddressingMode_ZeroPage();
                    LSR();
                    break;
                case 0x56:
                    // LSR - Zero page, X
                    register = x;
                    AddressingMode_IndexedZeroPage(ref register);
                    LSR();
                    break;
                case 0x4E:
                    // LSR - Absolute
                    AddressingMode_Absolute();
                    LSR();
                    break;
                case 0x5E:
                    // LSR - Absolute, X
                    register = x;
                    AddressingMode_IndexedAbsolute(ref register);
                    LSR();
                    break;

                //
                // ASL
                //
                case 0x0A:
                    // ASL - Accumulator
                    AddressingMode_Accumulator();
                    ASL();
                    break;
                case 0x06:
                    // ASL - Zero page
                    AddressingMode_ZeroPage();
                    ASL();
                    break;
                case 0x16:
                    // ASL - Zero page, X
                    register = x;
                    AddressingMode_IndexedZeroPage(ref register);
                    ASL();
                    break;
                case 0x0E:
                    // ASL - Absolute
                    AddressingMode_Absolute();
                    ASL();
                    break;
                case 0x1E:
                    // ASL - Absolute, X
                    register = x;
                    AddressingMode_IndexedAbsolute(ref register);
                    ASL();
                    break;

                //
                // ROL
                //
                case 0x2A:
                    // ROL - Accumulator
                    AddressingMode_Accumulator();
                    ROL();
                    break;
                case 0x26:
                    // ROL - Zero page
                    AddressingMode_ZeroPage();
                    ROL();
                    break;
                case 0x36:
                    // ROL - Zero page, X
                    register = x;
                    AddressingMode_IndexedZeroPage(ref register);
                    ROL();
                    break;
                case 0x2E:
                    // ROL - Absolute
                    AddressingMode_Absolute();
                    ROL();
                    break;
                case 0x3E:
                    // ROL - Absolute, X
                    register = x;
                    AddressingMode_IndexedAbsolute(ref register);
                    ROL();
                    break;

                //
                // ROR
                //
                case 0x6A:
                    // ROR - Accumulator
                    AddressingMode_Accumulator();
                    ROR();
                    break;
                case 0x66:
                    // ROR - Zero page
                    AddressingMode_ZeroPage();
                    ROR();
                    break;
                case 0x76:
                    // ROR - Zero page, X
                    register = x;
                    AddressingMode_IndexedZeroPage(ref register);
                    ROR();
                    break;
                case 0x6E:
                    // ROR - Absolute
                    AddressingMode_Absolute();
                    ROR();
                    break;
                case 0x7E:
                    // ROR - Absolute, X
                    register = x;
                    AddressingMode_IndexedAbsolute(ref register);
                    ROR();
                    break;

                //
                // INC
                //
                case 0xE6:
                    // INC - Zero page
                    AddressingMode_ZeroPage();
                    INC();
                    break;
                case 0xF6:
                    // INC - Zero page, X
                    register = x;
                    AddressingMode_IndexedZeroPage(ref register);
                    INC();
                    break;
                case 0xEE:
                    // INC - Absolute
                    AddressingMode_Absolute();
                    INC();
                    break;
                case 0xFE:
                    // INC - Absolute, X
                    register = x;
                    AddressingMode_IndexedAbsolute(ref register);
                    INC();
                    break;

                //
                // DEC
                //
                case 0xC6:
                    // DEC - Zero page
                    AddressingMode_ZeroPage();
                    DEC();
                    break;
                case 0xD6:
                    // DEC - Zero page, X
                    register = x;
                    AddressingMode_IndexedZeroPage(ref register);
                    DEC();
                    break;
                case 0xCE:
                    // DEC - Absolute
                    AddressingMode_Absolute();
                    DEC();
                    break;
                case 0xDE:
                    // DEC - Absolute, X
                    register = x;
                    AddressingMode_IndexedAbsolute(ref register);
                    DEC();
                    break;

                //
                // LDX
                //
                case 0xA2:
                    // LDX - Immediate
                    AddressingMode_Immediate();
                    LDX();
                    break;
                case 0xA6:
                    // LDX - Zero page
                    AddressingMode_ZeroPage();
                    LDX();
                    break;
                case 0xB6:
                    // LDX - Zero page, y
                    register = y;
                    AddressingMode_IndexedZeroPage(ref register);
                    LDX();
                    break;
                case 0xAE:
                    // LDX - Absolute
                    AddressingMode_Absolute();
                    LDX();
                    break;
                case 0xBE:
                    // LDX - Absolute, Y
                    register = y;
                    AddressingMode_IndexedAbsolute(ref register);
                    LDX();
                    break;

                //
                // STX
                //
                case 0x86:
                    // STX - Zero page
                    AddressingMode_ZeroPage();
                    STX();
                    break;
                case 0x96:
                    // STX - Zero page, y
                    register = y;
                    AddressingMode_IndexedZeroPage(ref register);
                    STX();
                    break;
                case 0x8E:
                    // STX - Absolute
                    AddressingMode_Absolute();
                    STX();
                    break;

                //
                // TAX
                //
                case 0xAA:
                    // TAX - Implied
                    AddressingMode_Implied();
                    TAX();
                    break;

                //
                // TXS
                //
                case 0x9A:
                    // TXS - Implied
                    AddressingMode_Implied();
                    TXS();
                    break;

                //
                // TSX
                //
                case 0xBA:
                    // TSX - Implied
                    AddressingMode_Implied();
                    TSX();
                    break;

                //
                // DEX
                //
                case 0xCA:
                    // DEX - Implied
                    AddressingMode_Implied();
                    DEX();
                    break;

                //  Group 3 instructions
                //  CPX LDY STY CPY INX INY
                //  BCC BCS BEQ BMI BNE BPL BVC BVS             - Relative only
                //  CLC SEC CLD SED CLI SEI CLV                 - Implied only
                //  PHA PHP PLA PLP BRK JSR RTI RTS JMP BIT

                //
                //  CPX
                //
                case 0xE0:
                    // CPX - Immediate
                    AddressingMode_Immediate();
                    CPX();
                    break;
                case 0xE4:
                    // CPX - Zero page
                    AddressingMode_ZeroPage();
                    CPX();
                    break;
                case 0xEC:
                    // CPX - Absolute
                    AddressingMode_Absolute();
                    CPX();
                    break;

                //
                //  LDY
                //
                case 0xA0:
                    // LDY - Immediate
                    AddressingMode_Immediate();
                    LDY();
                    break;
                case 0xA4:
                    // LDY - Zero page
                    AddressingMode_ZeroPage();
                    LDY();
                    break;
                case 0xB4:
                    // LDY - Zero page, X
                    register = x;
                    AddressingMode_IndexedZeroPage(ref register);
                    LDY();
                    break;
                case 0xAC:
                    // LDY - Absolute
                    AddressingMode_Absolute();
                    LDY();
                    break;
                case 0xBC:
                    // LDY - Absolute, X
                    register = x;
                    AddressingMode_IndexedAbsolute(ref register);
                    LDY();
                    break;

                //
                //  STY
                //
                case 0x84:
                    // STY - Zero page
                    AddressingMode_ZeroPage();
                    STY();
                    break;
                case 0x94:
                    // STY - Zero page, X
                    register = x;
                    AddressingMode_IndexedZeroPage(ref register);
                    STY();
                    break;
                case 0x8C:
                    // STY - Absolute
                    AddressingMode_Absolute();
                    STY();
                    break;

                //
                //  CPY
                //
                case 0xC0:
                    // CPY - Immediate
                    AddressingMode_Immediate();
                    CPY();
                    break;
                case 0xC4:
                    // CPY - Zero page
                    AddressingMode_ZeroPage();
                    CPY();
                    break;
                case 0xCC:
                    // CPY - Absolute
                    AddressingMode_Absolute();
                    CPY();
                    break;

                //
                //  INX
                //
                case 0xE8:
                    // INX - Implied
                    AddressingMode_Implied();
                    INX();
                    break;

                //
                //  INY
                //
                case 0xC8:
                    // INY - Implied
                    AddressingMode_Implied();
                    INY();
                    break;

                //   BCC BCS BEQ BMI BNE BPL BVC BVS
                    
                //
                //  BCC
                //
                case 0x90:
                    // BCC - Relative
                    AddressingMode_Relative();
                    BCC();
                    break;

                //
                //  BCS
                //
                case 0xB0:
                    // BCS - Relative
                    AddressingMode_Relative();
                    BCS();
                    break;

                //
                //  BEQ
                //
                case 0xF0:
                    // BEQ - Relative
                    AddressingMode_Relative();
                    BEQ();
                    break;

                //
                //  BMI
                //
                case 0x30:
                    // BMI - Relative
                    AddressingMode_Relative();
                    BMI();
                    break;

                //
                //  BNE
                //
                case 0xD0:
                    // BNE - Relative
                    AddressingMode_Relative();
                    BNE();
                    break;

                //
                //  BPL
                //
                case 0x10:
                    // BPL - Relative
                    AddressingMode_Relative();
                    BPL();
                    break;

                //
                //  BVC
                //
                case 0x50:
                    // BVC - Relative
                    AddressingMode_Relative();
                    BVC();
                    break;

                //
                //  BVS
                //
                case 0x70:
                    // BVS - Relative
                    AddressingMode_Relative();
                    BVS();
                    break;

                //  CLC SEC CLD SED CLI SEI CLV

                //
                //  CLC
                //
                case 0x18:
                    // CLC - Implied
                    AddressingMode_Implied();
                    CLC();
                    break;

                //
                //  SEC
                //
                case 0x38:
                    // SEC - Implied
                    AddressingMode_Implied();
                    SEC();
                    break;

                //
                //  CLD
                //
                case 0xD8:
                    // CLD - Implied
                    AddressingMode_Implied();
                    CLD();
                    break;

                //
                //  SED
                //
                case 0xF8:
                    // SED - Implied
                    AddressingMode_Implied();
                    SED();
                    break;

                //
                //  CLI
                //
                case 0x58:
                    // CLI - Implied
                    AddressingMode_Implied();
                    CLI();
                    break;

                //
                //  SEI
                //
                case 0x78:
                    // SEI - Implied
                    AddressingMode_Implied();
                    SEI();
                    break;

                //
                //  CLV
                //
                case 0xB8:
                    // CLV - Implied
                    AddressingMode_Implied();
                    CLV();
                    break;

                //  PHA PHP PLA PLP BRK JSR RTI RTS JMP BIT
                
                //
                //  PHA
                //
                case 0x48:
                    // PHA - Implied
                    AddressingMode_Implied();
                    PHA();
                    break;

                //
                //  PHP
                //
                case 0x08:
                    // PHP - Implied
                    AddressingMode_Implied();
                    PHP();
                    break;

                //
                //  PLA
                //
                case 0x68:
                    // PLA - Implied
                    AddressingMode_Implied();
                    PLA();
                    break;

                //
                //  PLP
                //
                case 0x28:
                    // PLP - Implied
                    AddressingMode_Implied();
                    PLP();
                    break;

                //
                //  BRK
                //
                case 0x00:
                    // BRK - Implied
                    AddressingMode_Implied();
                    BRK();
                    break;

                //
                //  JSR
                //
                case 0x20:
                    // JSR - Absolute
                    AddressingMode_Absolute();
                    JSR();
                    break;

                //
                //  RTI
                //
                case 0x40:
                    // RTI - Implied
                    AddressingMode_Implied();
                    RTI();
                    break;

                //
                //  RTS
                //
                case 0x60:
                    // RTS - Implied
                    AddressingMode_Implied();
                    RTS();
                    break;

                //
                //  JMP
                //
                case 0x4C:
                    // JMP - Absolute
                    AddressingMode_Absolute();
                    JMP();
                    break;
                case 0x6C:
                    // JMP - Indirect
                    AddressingMode_AbsoluteIndirect();
                    JMP();
                    break;

                //
                //  BIT
                //
                case 0x24:
                    // BIT - Zero Page
                    AddressingMode_ZeroPage();
                    BIT();
                    break;
                case 0x2C:
                    // BIT - Absolute 
                    AddressingMode_Absolute();
                    BIT();
                    break;

            } // End current opcode SWITCH

            IncrementPC();

        }
    }
}
