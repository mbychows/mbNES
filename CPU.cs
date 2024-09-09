using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ProgressBar;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;
using System.ComponentModel;
using System.Runtime.Remoting.Contexts;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TreeView;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;
using static System.Net.Mime.MediaTypeNames;
using System.Net.NetworkInformation;
using System.Xml.Linq;

namespace mbNES
{
    internal class CPU
    {



        // ---------------
        //    Registers   |  
        // ---------------

        public int a { get; private set; } = 0x00;         // Accumulator
        public int x { get; private set; } = 0x00;         // x register
        public int y { get; private set; } = 0x00;         // y register 
        public int s { get; private set; } = 0x00;         // Stack Pointer
        public int p { get; private set; } = 0x00;         // Status register

        public int pc { get; private set; } = 0x0000;    // Program counter, 2 bytes

        // public int cycleCount { get; private set; } = 0;   // Cycle counter

        private int currentOpcode = 0x00;

        public int workingData = 0x00;            // Holds the data read from address during each instruction
        public int effectiveAddress = 0x0000;    // Final address to be used by the instruction after addressing mode
                                                    //   operations have been applied

        private int tempLowOrderByte = 0x00;
        private int tempHighOrderByte = 0x00;
        private int tempAddress = 0x0000;
        private int tempResult = 0x0000;
        private int baseAddress;

        private int register = 0;
        
        public void SetRegisters(int a, int x, int y, int s, int p)
        {
            this.a = a;
            this.x = x;
            this.y = y;
            this.s = s;
            this.p = p;
        }

        public void SetPRegisterBit(int BitNumber, int value)  // No error checking on this:  TODO
        {
            if (value == 1)  {  p |= (1 << BitNumber); }  // Setting bit
            else { p &= ~(1 << BitNumber); }         // Clearing bit
        }

        public void ResetCPU()
        {
            SetRegisters(0x00, 0x00, 0x00, 0x00, 0x00);
            ResetPC();
        }

        public void SetPC(int pc)  { this.pc = pc; }

        private void ResetPC()  {  pc = 0x0000;  }

        private void IncrementPC()
        {
            if (pc == 65535) { ResetPC(); }     // Program counter wraparound
            else { pc++; }
        }

        private void ReadOpcode()
        { 
            Bus.cycleCount = 0;     // Reset cycle count for new instruction
            // get opcode stored at value in program counter
            currentOpcode = Bus.ReadBus(pc,true);
            IncrementPC();
        }

        // ----------------------
        //    Addressing Modes   |  
        // ----------------------

        //  ACCUMULATOR - This form of addressing is represented with a one byte instruction,
        //  implying an operation on the accumulator
        private void AddressingMode_Accumulator()
        {
            // Not needed?
        }



        // IMMEDIATE - In immediate addressing, the operand is contained in the second byte of the instruction,
        // with no further memory addressing required.
        //
        // 2 CYCLES
        // 
        private void AddressingMode_Immediate()
        {
             workingData = Bus.ReadBus(pc, true);   // CYCLE 2           
        }




        //  ABSOLUTE - In absolute addressing, the second byte of the instruction specifies the eight low order
        //  bits of the effective address while the third byte specifies the eight high order bits.Thus, the
        //  absolute addressing mode allows access to the entire 65K bytes of addressable memory.
        //
        //  4 CYCLES
        //
        public void AddressingMode_Absolute()  // done? // tested?
        {
            tempLowOrderByte = Bus.ReadBus(pc, true);           // CYCLE 2 - Read the 8 low-order bits of the address     
            IncrementPC();
            effectiveAddress = Bus.ReadBus(pc, true);           // CYCLE 3 - Read the 8 high-order bits of the address

            effectiveAddress <<= 8;                             // Shift the data into the high-order byte and add in the low order byte
            effectiveAddress += tempLowOrderByte;
            workingData = Bus.ReadBus(effectiveAddress, true);  // CYCLE 4
            
        }



        //  ZERO PAGE - The zero page instructions allow for shorter code and execution times by only fetching
        //  the second byte of the instruction and assuming a zero high address byte. Careful use of the zero
        //  page can result in significant increase in code efficiency.
        //
        //
        //
        private void AddressingMode_ZeroPage()  // Done?  // tested?
        {
            effectiveAddress = Bus.ReadBus(pc, true);   
            workingData = Bus.ReadBus(effectiveAddress, true);       
        }



        //  INDEXED ZERO PAGE - (X, Y indexing) - This form of addressing is used in conjunction with the index
        //  register and is referred to as "Zero Page, X" or "Zero Page, Y". The effective address is calculated
        //  by adding the second byte to the contents of the index register.  Since this is a form of "Zero Page"
        //  addressing, the content of the second byte references a location in page zero. Additionally due to
        //  the "Zero Page" addressing nature of this mode, no carry is added to the high order 8 bits of memory
        //  and crossing of page boundaries does not occur.
        //
        // 4 CYCLES
        //
        public void AddressingMode_IndexedZeroPage(ref int register)  // Done?
        {
            effectiveAddress = 0;
            effectiveAddress += Bus.ReadBus(pc, true);          // CYCLE 2 - Read second byte of instruction
            Bus.ReadBus(effectiveAddress, true);                // CYCLE 3 - Seems like this isn't needed but apparently it happens
            effectiveAddress += register;                       // Add the contents of the index register
            effectiveAddress = (effectiveAddress & ~(0xFF00));  // Clear the high order byte
            workingData = Bus.ReadBus(effectiveAddress, true);  // CYCLE 4
        }




        //  INDEXED ABSOLUTE - (X, Y indexing) - This form of addressing is used in conjunction with X and Y
        //  index register and is referred to as "Absolute, X", and "Absolute, Y". The effective address is
        //  formed by adding the contents of X or Y to the address contained in the second and third bytes of the
        //  instruction.This mode allows the index register to contain the index or count value and the in
        //  struction to contain the base address.  This type of indexing allows any location referencing and
        //  the index to modify multiple fields resulting in reduced coding and execution time.
        //
        //  4 CYCLES (+1 IF PAGE CROSSED)
        //
        public void AddressingMode_IndexedAbsolute(ref int register)
        {
            tempLowOrderByte = Bus.ReadBus(pc, true);           // CYCLE 2 - Read the 8 low-order bits of the address     
            IncrementPC();
            baseAddress = Bus.ReadBus(pc, true);                // CYCLE 3 - Read the 8 high-order bits of the base address

            baseAddress <<= 8;                                  // Shift the data into the high-order byte and add in the low order byte
            baseAddress += tempLowOrderByte;
            effectiveAddress = baseAddress + register;          // Add the register contents to base address to get effective address

            if ((baseAddress >> 8) != (effectiveAddress >> 8))  // If a page boundary is crossed when reading the effective address,
            { Bus.cycleCount++; }                               // Increment the cycle count

            effectiveAddress &= ~(1 << 16);                     // Clear bit 16 in case the addition carries over

            workingData = Bus.ReadBus(effectiveAddress, true);  // CYCLE 4/5 
        }



        //  IMPLIED ADDRESSING - In the implied addressing mode, the address containing the operand is implicitly stated
        //  in the operation code of the instruction.
        private void AddressingMode_Implied()
        {
            // Not needed?
        }



        //  RELATIVE ADDRESSING - Relative addressing is used only with branch instructions and
        //  establishes a destination for the conditional branch.
        //  The second byte of the instruction becomes the operand which is an "Offset" added to the contents of
        //  the lower eight bits of the program counter when the counter is set at the next instruction.The
        //  range of the offset is -128 to +127 bytes from the next instruction.
        public void AddressingMode_Relative()  // Check for page boundary crossing and negative PC values
        {
            workingData = Bus.ReadBus(pc);          // Get the offset
            IncrementPC();                          // Set program counter for "next instruction"
            effectiveAddress = pc;
            if (workingData >> 7 == 1)              // If offset is negative
            {
                workingData &= ~(1 << 7);           // Clear bit 8 to prep for subtraction
                Console.WriteLine("Working Data after bit 8 clear: " + workingData.ToString("x2"));
                effectiveAddress -= workingData;    // Subtract from program counter
            }
            else                                    // Offset is positive
            {
                effectiveAddress += workingData;    // Add to program counter
            }

            workingData = Bus.ReadBus(effectiveAddress);
        }



        //  INDEXED INDIRECT ADDRESSING - In indexed indirect addressing (referred to as (Indirect,X)), the second byte of
        //  the instruction is added to the contents of the X index register, discarding the carry. The result
        //  of this addition points to a memory location on page zero whose contents is the low order eight bits
        //  of the effective address.The next memory location in page zero contains the high order eight bits
        //  of the effective address. Both memory locations specifying the high and low order bytes of the
        //  effective address must be in page zero.
        //
        //  6 CYCLES
        //
        public void AddressingMode_IndirectX()  // done?
        {
            baseAddress = Bus.ReadBus(pc,true);                 // CYCLE 2 - Read second byte of the instruction
            tempAddress = baseAddress + x;                      // Add to x register
            //tempAddress = (Bus.ReadBus(pc, true) + x);          
            tempAddress &= ~(1 << 8);                           // Clear bit 8 in case the addition carries over
            Bus.cycleCount++;                                   // CYCLE 3 - Not sure why this occurs
            tempLowOrderByte = Bus.ReadBus(tempAddress, true);  // CYCLE 4 - Get value of this zero page memory location, store as low order byte
            tempAddress++;                                      // Go to the next zero page address
            tempAddress &= ~(1 << 8);                           // Clear bit 8 to stay on zero page

            effectiveAddress = Bus.ReadBus(tempAddress, true);  // CYCLE 5 - Get value of the address, store as high order byte
            effectiveAddress <<= 8;                             // Shift value into high order byte

            effectiveAddress += tempLowOrderByte;               // Combine to get effective address
            workingData = Bus.ReadBus(effectiveAddress, true);  // CYCLE 6
        }


        //  INDIRECT INDEXED ADDRESSING - In indirect indexed addressing (referred to as (Indirect),Y), the second byte
        //  of the instruction points to a memory location in page zero.The contents of this memory location
        //  is added to the contents of the Y index register, the result being the low order eight bits of the
        //  effective address. The carry from this addition is added to the contents of the next pagezero
        //  memory location, the result being the high order eight bits of the effective address.
        //
        //  5 CYCLES (+1 IF PAGE CROSSED)
        //
        public void AddressingMode_IndirectY()
        {

            tempAddress = Bus.ReadBus(pc, true);                    // CYCLE 2 - Read operand and store as first page zero memory location 
            tempLowOrderByte = Bus.ReadBus(tempAddress, true);      // CYCLE 3 - Read value of said location, which is low-order byte of base address
            tempAddress++;                                          // Next page zero location
            tempAddress &= ~(1 << 8);                               // Make sure we're staying on page zero

            baseAddress = Bus.ReadBus(tempAddress, true);           // CYCLE 4 - Read next page zero memory location, which is high-order byte of base address
            baseAddress <<= 8;                                      // Shift value into high order byte
            baseAddress += tempLowOrderByte;                        // Add the low order byte
            
            effectiveAddress = baseAddress + y;                     // Add the contents of the index register
            effectiveAddress &= ~(1 << 16);                         // Clear bit 16 in case it carried over from the addition

            if ((baseAddress >> 8) != (effectiveAddress >> 8))      // Check for page crossing and add another cycle if needed
            {  Bus.cycleCount++;  }

            workingData = Bus.ReadBus(effectiveAddress, true);      // CYCLE 5/6

            //tempAddress = Bus.ReadBus(pc, true);                    // CYCLE 2 - Read second byte and create a zero page address from the data
            //tempLowOrderByte = Bus.ReadBus(tempAddress, true) + y;  // CYCLE 3 - Read value of zero page memory location, add to y register,
                                                                    // store as low order byte of effective adddress
            //tempAddress++;
            //tempAddress &= ~(1 << 8);                               // Clear bit 8 to stay on zero page
            
            //effectiveAddress = Bus.ReadBus(tempAddress, true);      // CYCLE 4 - Get next page zero memory value
            
            //effectiveAddress <<=  8;                                // Shift value into high-order byte
            //effectiveAddress += tempLowOrderByte;                   // Combine with low-order byte to form effective address
            //effectiveAddress &= ~(1 << 16);                         // Clear bit 16 in case the addition carries over

            //if ((baseAddress >> 8) != (effectiveAddress >> 8))  // If a page boundary is crossed when reading the effective address,
            //{ Bus.cycleCount++; }                               // Increment the cycle count

            //workingData = Bus.ReadBus(effectiveAddress,true);       // CYCLE 6
        }


        //  ABSOLUTE INDIRECT - The second byte of the instruction contains the low order eight bits of a memory location.
        //  The high order eight bits of that memory location is contained in the third byte of the instruction.
        //  The contents of the fully specified memory location is the low order byte of the effective address.
        //  The next memory location contains the high order byte of the effective address which is loaded
        //  into the sixteen bits of the program counter.
        //
        //
        //
        public void AddressingMode_AbsoluteIndirect() // done?
        {
            tempLowOrderByte = Bus.ReadBus(pc,true);            // Get second byte of instruction, assign to low order byte of address

            IncrementPC();
            tempAddress = Bus.ReadBus(pc, true);                // Get third byte of instruction,
            tempAddress = (tempAddress << 8);                   // assign to high order byte of address
            tempAddress += tempLowOrderByte;

            tempLowOrderByte = Bus.ReadBus(tempAddress, true);  // Get contents of address, assign to low order byte of effective address

            tempAddress++;
            effectiveAddress = Bus.ReadBus(tempAddress,true);   // Get contents of next address, 
            effectiveAddress = (effectiveAddress << 8);         // assign to high order byte of effective address

            effectiveAddress += tempLowOrderByte;               // Combine for effective address 

            workingData = Bus.ReadBus(effectiveAddress,true);
        }




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
                    //Console.WriteLine("0x69 - ADC - immediate");
                    AddressingMode_Immediate();
                    ADC();
                    break;
                case 0x65:      // ADC - Zero page
                    //Console.WriteLine("0x65 - ADC - Zero page");
                    AddressingMode_ZeroPage();
                    ADC();
                    break;
                case 0x75:      // ADC - Zero page, X
                    //Console.WriteLine("0x75 - ADC - Zero page, X");
                    register = x;
                    AddressingMode_IndexedZeroPage(ref register);
                    ADC();
                    break;
                case 0x6D:      // ADC - Absolute
                    //Console.WriteLine("0x6D - ADC - Absolute");
                    AddressingMode_Absolute();
                    ADC();
                    break;
                case 0x7D:      // ADC - Absolute, X
                    //Console.WriteLine("0x7D - ADC - Absolute, X");
                    register = x;
                    AddressingMode_IndexedAbsolute(ref register);
                    ADC();
                    break;
                case 0x79:      // ADC - Absolute, Y
                    //Console.WriteLine("0x79 - ADC - Absolute, Y");
                    register = y;
                    AddressingMode_IndexedAbsolute(ref register);
                    ADC();
                    break;
                case 0x61:      // ADC - Indirect, X
                    //Console.WriteLine("0x61 - ADC - Indirect, X");
                    AddressingMode_IndirectX();
                    ADC();
                    break;
                case 0x71:      // ADC - Indirect, Y
                    //Console.WriteLine("0x71 - ADC - Indirect, Y");
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
                    // AND - Indirect, Y
                    AddressingMode_IndirectY();
                    CMP();
                    break;



                    //
                    //  EOR 
                    //

                    //
                    //  LDA 
                    //

                    //
                    //  ORA 
                    //

                    //
                    //  SBC 
                    //

                    //
                    //  STA
                    //

                    //    Group 2 instructions
                    //    ASL ROL ROR -Accumulator only
                    //    INC DEC LDX STX TAX TXS TSX DEX


            } // End current opcode SWITCH

            IncrementPC();



        }

        public void ADC()
        {
            int aTemp = a;  // For calculating overflow flag

            // a += workingData;  // Adds the operand and carry bit to accumulator
            // Adds the operand and carry bit to accumulator
            aTemp = a + workingData + (p & 1);   // This works because the carry bit is bit 0 in the P register

            //cycleCount++;

            // P Register Flags

            // C: Carry - set if Bit 8 of the result is set (an int can hold this result)
            if ((aTemp & (1 << 8)) != 0)
            {
                // Set the carry bit in the p register
                SetPRegisterBit(0, 1);
                // Clear bit 8 in aTemp
                aTemp &= ~(1 << 8);
            }
            else { SetPRegisterBit(0, 0); }

            // Z: Zero 
            if (aTemp == 0) { SetPRegisterBit(1, 1); }
            else { SetPRegisterBit(1, 0); }

            // V: Overflow
            // Overflow if  (M^result) & (N^result) & 0x80 != 0
            if (((a ^ aTemp) & (workingData ^ aTemp) & 0x80) != 0)
            {
                //Console.WriteLine("Overflow!");
                SetPRegisterBit(6, 1);           // Set V
            }
            else
            {
                SetPRegisterBit(6, 0);           // Clear V
            }

            // N: Negative - equal to vaue of sign bit (7) 
            if ((aTemp & (1 << 7)) != 0) { SetPRegisterBit(7, 1); }
            else { SetPRegisterBit(7, 0); }

            a = aTemp;
        }  // End ADC()

        public void AND()
        {
            a &= workingData;

            // Z: Zero 
            if (a == 0) { SetPRegisterBit(1, 1); }
            else { SetPRegisterBit(1, 0); }

            // N: Negative - equal to vaue of sign bit (7) 
            if ((a & (1 << 7)) != 0) { SetPRegisterBit(7, 1); }
            else { SetPRegisterBit(7, 0); }
        }  // End AND()
	 
        public void CMP()
        {
            tempResult = a - workingData;
            // C: Carry - set if a >= working data
            if (a >= workingData)
            {
                // Set the carry bit in the p register
                SetPRegisterBit(0, 1);
            }
            else { SetPRegisterBit(0, 0); }

            // Z: Zero 
            if (a == workingData) { SetPRegisterBit(1, 1); }
            else { SetPRegisterBit(1, 0); }

            // N: Negative - equal to vaue of sign bit (7) 
            if ((tempResult & (1 << 7)) != 0) { SetPRegisterBit(7, 1); }
            else { SetPRegisterBit(7, 0); }

        }  // End CMP()

    }

    /*


    Group 3 instructions
    CPX LDY STY CPY INX INY

    BCC BCS BEQ BMI BNE BPL BVC BVS - Relative only

    CLC SEC CLD SED CLI SEI CLV - Implied only

    PHA PHP PLA PLP BRK JSR RTI RTS JMP BIT

     */
}
