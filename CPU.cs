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

        public byte a { get; private set; } = 0x00;         // Accumulator
        public byte x { get; private set; } = 0x00;         // x register
        public byte y { get; private set; } = 0x00;         // y register 
        public byte s { get; private set; } = 0x00;         // Stack Pointer
        public byte p { get; private set; } = 0x00;         // Status register

        public ushort pc { get; private set; } = 0x0000;    // Program counter, 2 bytes

        public byte cycleCount { get; private set; } = 0;   // Cycle counter

        private byte currentOpcode = 0x00;

        private byte workingData = 0x00;            // Holds the data read from address during each instruction
        public ushort effectiveAddress = 0x0000;    // Final address to be used by the instruction after addressing mode
                                                    //   operations have been applied

        private byte tempLowOrderByte = 0x00;
        private byte tempHighOrderByte = 0x00;
        private ushort tempAddress = 0x0000;
        private ushort tempResult = 0x0000;
        
        public void SetRegisters(byte a, byte x, byte y, byte s, byte p)
        {
            this.a = a;
            this.x = x;
            this.y = y;
            this.s = s;
            this.p = p;
        }

        public void ResetCPU()
        {
            SetRegisters(0x00, 0x00, 0x00, 0x00, 0x00);
            ResetPC();
        }

        public void SetPC(ushort pc)
        { this.pc = pc; }

        private void ResetPC()
        {
            pc = 0x0000;
        }

        private void ReadOpcode()  // Probably not needed
        {
            // get opcode stored at value in program counter
            currentOpcode = Bus.ReadBus(pc);

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
        private void AddressingMode_Immediate()  // done?
        {
            // Move to the next address in the program counter since
            // that's where the operand is, store the value

            // Is this even needed?
            pc++;  
            workingData = Bus.ReadBus(pc);
        }


        //  ABSOLUTE - In absolute addressing, the second byte of the instruction specifies the eight low order
        //  bits of the effective address while the third byte specifies the eight high order bits.Thus, the
        //  absolute addressing mode allows access to the entire 65K bytes of addressable memory.
        public void AddressingMode_Absolute()  // done? // tested?
        {
            effectiveAddress = 0x0000;
            //ResetCPU();
            // Increment program counter to read the 8 low-order bits of the address
            pc++;
            tempLowOrderByte = Bus.ReadBus(pc);
            // Increment program counter to read the 8 high-order bits of the address
            pc++;
            effectiveAddress += (ushort)Bus.ReadBus(pc);
            
            // Shift the data into the high-order byte and add in the low order byte
            effectiveAddress = (ushort)(effectiveAddress << 8);
            effectiveAddress += tempLowOrderByte;
            
        }

        //  ZERO PAGE - The zero page instructions allow for shorter code and execution times by only fetching
        //  the second byte of the instruction and assuming a zero high address byte. Careful use of the zero
        //  page can result in significant increase in code efficiency.
        private void AddressingMode_ZeroPage()  // Done?  // tested?
        {
            pc++;
            effectiveAddress = 0x0000;
            effectiveAddress += Bus.ReadBus(pc);
            
        }

        //  INDEXED ZERO PAGE - (X, Y indexing) - This form of addressing is used in conjunction with the index
        //  register and is referred to as "Zero Page, X" or "Zero Page, Y". The effective address is calculated
        //  by adding the second byte to the contents of the index register.  Since this is a form of "Zero Page"
        //  addressing, the content of the second byte references a location in page zero. Additionally due to
        //  the "Zero Page" addressing nature of this mode, no carry is added to the high order 8 bits of memory
        //  and crossing of page boundaries does not occur.
        public void AddressingMode_IndexedZeroPage(ref byte register)  // Done?
        {
            pc++;
            effectiveAddress = 0x0000;
            effectiveAddress += Bus.ReadBus(pc); // Read second byte of instruction
            effectiveAddress += register;  // Add the contents of the index register
            effectiveAddress = (ushort)(effectiveAddress & ~(0xFF00));  // Clear the high order byte

        
        }


        //  INDEXED ABSOLUTE - (X, Y indexing) - This form of addressing is used in conjunction with X and Y
        //  index register and is referred to as "Absolute, X", and "Absolute, Y". The effective address is
        //  formed by adding the contents of X or Y to the address contained in the second and third bytes of the
        //  instruction.This mode allows the index register to contain the index or count value and the in
        //  struction to contain the base address.  This type of indexing allows any location referencing and
        //  the index to modify multiple fields resulting in reduced coding and execution time.
        public void AddressingMode_IndexedAbsolute(ref byte register)   // need to check for page boundary crossing   
        { 
            AddressingMode_Absolute();      // Load the effective address with the address formed from the next two bytes
            // Console.WriteLine("Effective address: " + effectiveAddress.ToString("x4"));
            effectiveAddress += register;   // Add the contents of the index register
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
            pc++;
            workingData = Bus.ReadBus(pc);  // Get the offset
            pc++;  // Set program counter for "next instruction"
            effectiveAddress = pc;
            if (workingData >> 7 == 1)  // If offset is positive
            {
                workingData &= ~(1 << 7);  // Clear bit 8 to prep for addition
                Console.WriteLine("Working Data after bit 8 clear: " + workingData.ToString("x2"));
                effectiveAddress += workingData;  // Add to program counter
            }
            else  // Offset is negative
            {
                effectiveAddress -= workingData;  // Subtract from program counter
            }
            

        }



        //  INDEXED INDIRECT ADDRESSING - In indexed indirect addressing (referred to as (Indirect,X)), the second byte of
        //  the instruction is added to the contents of the X index register, discarding the carry. The result
        //  of this addition points to a memory location on page zero whose contents is the low order eight bits
        //  of the effective address.The next memory location in page zero contains the high order eight bits
        //  of the effective address. Both memory locations specifying the high and low order bytes of the
        //  effective address must be in page zero.
        private void AddressingMode_IndirectX()  // done?
        {
            tempAddress = 0x0000;
            // Read second byte of the instruction, add to x register
            pc++;
            tempAddress = (ushort)(Bus.ReadBus(pc) + x);
            // Discard carry
            tempAddress = (byte)(tempAddress & ~(1 << 9));   // Clear bit 9 in case the addition carries over

            // Get value of this zero page memory location, store as low order byte
            tempLowOrderByte = Bus.ReadBus(tempAddress);

            // Get value of following address, store as high order byte
            effectiveAddress = 0x0000;
            effectiveAddress += Bus.ReadBus(++tempAddress);
            effectiveAddress = (ushort)(effectiveAddress << 8);  // Shift value into high order byte

            // Combine to get effective address
            effectiveAddress += tempLowOrderByte;
        }


        //  INDIRECT INDEXED ADDRESSING - In indirect indexed addressing (referred to as (Indirect),Y), the second byte
        //  of the instruction points to a memory location in page zero.The contents of this memory location
        //  is added to the contents of the Y index register, the result being the low order eight bits of the
        //  effective address. The carry from this addition is added to the contents of the next pagezero
        //  memory location, the result being the high order eight bits of the effective address.
        private void AddressingMode_IndirectY()  // done?
        {
            tempAddress = 0x0000;
            // Read second byte and create a zero page address from the data
            pc++;
            tempAddress += (Bus.ReadBus(pc));

            // Read value of zero page memory location, add to y register
            tempResult = (ushort)(Bus.ReadBus(tempAddress) + y);

            // Result of add stored as low order byte of effectve address
            tempLowOrderByte = (byte)tempResult;

            // Get next page zero memory value
            tempHighOrderByte = Bus.ReadBus(++tempAddress);

            if (tempResult >> 8 == 1)   // If there is a carry from the addition
            {
                tempHighOrderByte++;  // Increment the high order byte
            }

            // Combine to form effective address
            effectiveAddress += tempHighOrderByte;
            effectiveAddress += (ushort)(effectiveAddress << 8);
            effectiveAddress += tempLowOrderByte;

        }


        //  ABSOLUTE INDIRECT - The second byte of the instruction contains the low order eight bits of a memory location.
        //  The high order eight bits of that memory location is contained in the third byte of the instruction.
        //  The contents of the fully specified memory location is the low order byte of the effective address.
        //  The next memory location contains the high order byte of the effective address which is loaded
        //  into the sixteen bits of the program counter.
        private void AddressingMode_AbsoluteIndirect() // done?
        {
            // Get second byte of instruction, assign to low order byte of address
            pc++;
            tempLowOrderByte = Bus.ReadBus(pc);

            // Get third byte of instruction, assign to high order byte of address
            pc++;
            tempAddress = Bus.ReadBus(pc);
            tempAddress += (ushort)(tempAddress << 8);
            tempAddress += tempLowOrderByte;

            // Get contents of address, assign to low order byte of effective address
            tempLowOrderByte = Bus.ReadBus(tempAddress);

            // Get contents of next address, assign to high order byte of effective address
            effectiveAddress = Bus.ReadBus(++tempAddress);
            effectiveAddress = (ushort)(effectiveAddress << 8);

            // Combine for effective address and load into program counter
            effectiveAddress += tempLowOrderByte;
            pc = effectiveAddress;

        }

        public void ExecuteInstruction()
        {
            this.ReadOpcode();

            switch (currentOpcode)
            {
                // Group 1 instructions - 8 addressing modes
                // ADC AND CMP EOR LDA ORA SBC STA

                //
                //  ADC - Add with Carry
                //
                case 0x69:
                    // ADC - Immediate
                    Console.WriteLine("0x69 - ADC - immediate");
                    break;
                case 0x65:
                    // ADC - Zero page
                    Console.WriteLine("0x65 - ADC - Zero page");
                    break;
                case 0x75:
                    // ADC - Zero page, X
                    Console.WriteLine("0x75 - ADC - Zero page, X");
                    break;
                case 0x6D:
                    // ADC - Absolute
                    Console.WriteLine("0x6D - ADC - Absolute");
                    break;
                case 0x7D:
                    // ADC - Absolute, X
                    Console.WriteLine("0x7D - ADC - Absolute, X");
                    break;
                case 0x79:
                    // ADC - Absolute, Y
                    Console.WriteLine("0x79 - ADC - Absolute, Y");
                    break;
                case 0x61:
                    // ADC - Indirect, X
                    Console.WriteLine("0x61 - ADC - Indirect, X");
                    break;
                case 0x71:
                    // ADC - Indirect, Y
                    Console.WriteLine("0x71 - ADC - Indirect, Y");
                    break;

                //
                //  AND - Logical AND
                //  0x29 0x25 0x35 0x2D 0x3D 0x39 0x21 0x31
                case 0x29:
                    // AND - immediate
                    break;
                case 0x25:
                    // AND - Zero page
                    break;
                case 0x35:
                    // AND - Zero page, X
                    break;
                case 0x2D:
                    // AND - Absolute
                    break;
                case 0x3D:
                    // AND - Absolute, X
                    break;
                case 0x39:
                    // AND - Absolute, Y
                    break;
                case 0x21:
                    // AND - Indirect, X
                    break;
                case 0x31:
                    // AND - Indirect, Y
                    break;

                    //
                    //  CMP - 
                    //  

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


            }

            pc++;



        }
	 
    }

    /*


    Group 3 instructions
    CPX LDY STY CPY INX INY

    BCC BCS BEQ BMI BNE BPL BVC BVS - Relative only

    CLC SEC CLD SED CLI SEI CLV - Implied only

    PHA PHP PLA PLP BRK JSR RTI RTS JMP BIT

     */
}
