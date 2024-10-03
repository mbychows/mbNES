using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mbNES
{
    public partial class CPU
    {

        //  ACCUMULATOR - This form of addressing is represented with a one byte instruction,
        //  implying an operation on the accumulator
        private void AddressingMode_Accumulator()
        {
            IncrementPC();   
            // Not needed?
        }



        // IMMEDIATE - In immediate addressing, the operand is contained in the second byte of the instruction,
        // with no further memory addressing required.
        //
        // 2 CYCLES
        // 
        private void AddressingMode_Immediate()
        {
            effectiveAddress = pc;
            IncrementPC();
            //workingData = Bus.ReadBus(pc, true);   // CYCLE 2           
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
            //workingData = Bus.ReadBus(effectiveAddress, true);  // CYCLE 4
            IncrementPC();

        }



        //  ZERO PAGE - The zero page instructions allow for shorter code and execution times by only fetching
        //  the second byte of the instruction and assuming a zero high address byte. Careful use of the zero
        //  page can result in significant increase in code efficiency.
        //
        //  3 CYCLES
        //
        private void AddressingMode_ZeroPage()  // Done?  // tested?
        {
            effectiveAddress = Bus.ReadBus(pc, true);       // CYCLE 2
            // workingData = Bus.ReadBus(effectiveAddress, true);
            IncrementPC();
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
            //workingData = Bus.ReadBus(effectiveAddress, true);  // CYCLE 4
            IncrementPC();
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
            IncrementPC();
            baseAddress <<= 8;                                  // Shift the data into the high-order byte and add in the low order byte
            baseAddress += tempLowOrderByte;
            effectiveAddress = baseAddress + register;          // Add the register contents to base address to get effective address
            //Console.WriteLine("effective address: " + baseAddress.ToString("x2"));
            //Console.WriteLine("register value: " + register.ToString("x2"));
            //Console.WriteLine("indexed address: " + (baseAddress + register).ToString("x2"));

            //Console.WriteLine(baseAddress >> 8);
            //Console.WriteLine(effectiveAddress >> 8);

            if ((baseAddress >> 8) != (effectiveAddress >> 8))  // If a page boundary is crossed when reading the effective address,
            {
                Bus.cycleCount++;                                // Increment the cycle count
                //Console.WriteLine("Crossed!");
            }

            else switch (currentOpcode)                         // The following instructions always use an extra cycle here:
                {
                    // Read-Modify-Write
                    case 0x1E: // ASL
                    case 0xDE: // DEC
                    case 0xFE: // INC
                    case 0x5E: // LSR
                    case 0x3E: // ROL
                    case 0x7E: // ROR
                    // Store
                    case 0x9D: // STA
                    case 0x99: // STA
                        {
                            Bus.cycleCount++;
                            break;
                        }
                }

            effectiveAddress &= ~(1 << 16);                     // Clear bit 16 in case the addition carries over

            //workingData = Bus.ReadBus(effectiveAddress, true);  // CYCLE 4/5 
            
            
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
            relativeOffset = Bus.ReadBus(pc,true);      // Get the offset
            IncrementPC();                              // Set program counter for "next instruction"
            
            effectiveAddress = pc;
            if ((relativeOffset >> 7) == 1)               // If offset is negative
            {
                tempRelativeOffset = relativeOffset;
                //Console.WriteLine(tempRelativeOffset);
                relativeOffset = (~tempRelativeOffset & 0x000000FF) + 1;   // Two's complement to get the value
                //Console.WriteLine(relativeOffset);
                //relativeOffset++;
                //relativeOffset &= ~(1 << 7);            // Clear bit 7 to prep for subtraction
                //Console.WriteLine("Working Data after bit 7 clear: " + relativeOffset.ToString("x2"));
                //Console.WriteLine(relativeOffset);
                effectiveAddress -= relativeOffset;     // Subtract from program counter
                
                if (effectiveAddress < 0)               // Adjust for PC wraparound
                {
                    effectiveAddress = (effectiveAddress + 65536);
                }
            }
            else                                        // Offset is positive
            {
                effectiveAddress += relativeOffset;     // Add to program counter
                
                if (effectiveAddress > 65535)           // Adjust for PC wraparound
                {
                    effectiveAddress = (effectiveAddress - 65536);
                }
            }

            //Console.WriteLine(effectiveAddress);
            //workingData = Bus.ReadBus(effectiveAddress);
            
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
            baseAddress = Bus.ReadBus(pc, true);                 // CYCLE 2 - Read second byte of the instruction
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
            //workingData = Bus.ReadBus(effectiveAddress, true);  // CYCLE 6
            IncrementPC();
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
            { Bus.cycleCount++; }

            else switch (currentOpcode)                             // The following instructions always use an extra cycle here:
                {
                    // Store
                    case 0x91: // STA
                        {
                            Bus.cycleCount++;
                            break;
                        }
                }


            IncrementPC();

            //workingData = Bus.ReadBus(effectiveAddress, true);      // CYCLE 5/6

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
            tempLowOrderByte = Bus.ReadBus(pc, true);            // Get second byte of instruction, assign to low order byte of address

            IncrementPC();
            tempAddress = Bus.ReadBus(pc, true);                // Get third byte of instruction,
            tempAddress = (tempAddress << 8);                   // assign to high order byte of address
            tempAddress += tempLowOrderByte;

            tempLowOrderByte = Bus.ReadBus(tempAddress, true);  // Get contents of address, assign to low order byte of effective address

            tempAddress++;
            effectiveAddress = Bus.ReadBus(tempAddress, true);   // Get contents of next address, 
            effectiveAddress = (effectiveAddress << 8);         // assign to high order byte of effective address

            effectiveAddress += tempLowOrderByte;               // Combine for effective address 

            //workingData = Bus.ReadBus(effectiveAddress, true);
            IncrementPC();
        }


    }
}
