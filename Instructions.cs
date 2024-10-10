using mbNES.Properties;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mbNES
{
    public partial class CPU
    {
        // ADC - Add with Carry
        // This instruction adds the contents of a memory location to the accumulator together with the carry bit.
        // If overflow occurs the carry bit is set, this enables multiple byte addition to be performed.
        // A,Z,C,N = A+M+C
        public void ADC()
        {
            workingData = Bus.ReadBus(effectiveAddress, true);

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


        // AND - Logical AND
        // A logical AND is performed, bit by bit, on the accumulator contents using the contents of a byte of memory
        // A,Z,N = A&M
        public void AND()
        {
            workingData = Bus.ReadBus(effectiveAddress, true);
            a &= workingData;

            // Z: Zero 
            if (a == 0) { SetPRegisterBit(1, 1); }
            else { SetPRegisterBit(1, 0); }

            // N: Negative - equal to vaue of sign bit (7) 
            if ((a & (1 << 7)) != 0) { SetPRegisterBit(7, 1); }
            else { SetPRegisterBit(7, 0); }
        }  // End AND()



        // ASL - Arithmetic Shift Left
        // This operation shifts all the bits of the accumulator or memory contents one bit left. Bit 0 is set
        // to 0 and bit 7 is placed in the carry flag. The effect of this operation is to multiply the memory
        // contents by 2 (ignoring 2's complement considerations), setting the carry if the result will not
        // fit in 8 bits.
        // A,Z,C,N = M*2
        // or
        // M,Z,C,N = M*2
        public void ASL()
        {
            if (currentOpcode == 0x0A)      // Accumulator addressing mode
            {
                workingData = a;            // Load data from accumulator
                Bus.cycleCount++;           // Increment cycle count
                this.pc--;                  // One byte instruction, don't need the next PC increment at the end of CPU::ExecuteInstruction()
            }

            else
            {
                workingData = Bus.ReadBus(effectiveAddress, true);       // All other modes 
                Bus.cycleCount++;                                        // Read-Modify-Write instructions read from the effective address,
            }                                                            //    then write it back on the next cycle

            // C: Carry - Set to contents of bit 7 before shift
            if ((workingData & (1 << 7)) != 0) { SetPRegisterBit(0, 1); }
            else { SetPRegisterBit(0, 0); }

            workingData <<= 1;                          // Shift left by 1 bit
            workingData &= ~(1 << 8);                   // Clear bit 8 

            // Z: Zero 
            if (workingData == 0) { SetPRegisterBit(1, 1); }
            else { SetPRegisterBit(1, 0); }

            // N: Negative - equal to vaue of sign bit (7) 
            if ((workingData & (1 << 7)) != 0) { SetPRegisterBit(7, 1); }
            else { SetPRegisterBit(7, 0); }

            // Write back to accumulator or memory location
            if (currentOpcode == 0x0A)
            {
                a = workingData;
            }
            else { Bus.WriteBus(effectiveAddress, workingData, true); }

            

        }  // End ASL()



        // BCC - Branch if Carry Clear
        // If the carry flag is clear then add the relative displacement to the program counter to cause a
        // branch to a new location.
        // NV1B DIZC
        public void BCC()
        {
            if ((p & (1 << 0)) == 0)        // If the carry bit is clear
            {
                if ((pc >> 8) != (effectiveAddress >> 8))       // If page crossed,
                {
                    //njuh   Console.WriteLine("Page crossed - " + effectiveAddress);
                    Bus.cycleCount++;                           // Add extra cycle
                }
                SetPC(effectiveAddress);                        // Set PC to (PC + offset) calculated from AddressingMode_Relative()
                Bus.cycleCount++;
            }
        }



        // BCS - Branch if Carry Set
        // If the carry flag is set then add the relative displacement to the program counter to cause a
        // branch to a new location.
        // NV1B DIZC
        public void BCS()
        {
            if ((p & (1 << 0)) == 1)        // If the carry bit is set
            {
                if ((pc >> 8) != (effectiveAddress >> 8))       // If page crossed,
                {
                    //njuh   Console.WriteLine("Page crossed - " + effectiveAddress);
                    Bus.cycleCount++;                           // Add extra cycle
                }
                SetPC(effectiveAddress);                        // Set PC to (PC + offset) calculated from AddressingMode_Relative()
                Bus.cycleCount++;
            }

        }



        // BEQ - Branch if Equal
        // If the zero flag is set then add the relative displacement to the program counter to cause a
        // branch to a new location.
        // NV1B DIZC
        public void BEQ()
        {
            if ((p & (1 << 1)) != 0)                            // If the zero flag is set
            {
                if ((pc >> 8) != (effectiveAddress >> 8))       // If page crossed,
                {
                    //Console.WriteLine("Page crossed - " + effectiveAddress);
                    Bus.cycleCount++;                           // Add extra cycle
                }
                SetPC(effectiveAddress);                        // Set PC to (PC + offset) calculated from AddressingMode_Relative()
                Bus.cycleCount++;
            }

        }



        // BIT - Bit Test
        // This instruction is used to test if one or more bits are set in a target memory location. The
        // mask pattern in A is ANDed with the value in memory to set or clear the zero flag, but the result
        // is not kept. Bits 7 and 6 of the value from memory are copied into the N and V flags.
        // A & M, N = M7, V = M6
        // NV1B DIZC
        public void BIT()
        {
            tempMemoryContents = Bus.ReadBus(effectiveAddress, true);
            tempResult = a & tempMemoryContents;

            // Z: Zero 
            if (tempResult == 0) { SetPRegisterBit(1, 1); }
            else { SetPRegisterBit(1, 0); }



            // O: Overflow - equal to vaue of overflow bit (6) 
            if ((tempMemoryContents & (1 << 6)) != 0) { SetPRegisterBit(6, 1); }
            else { SetPRegisterBit(6, 0); }

            // N: Negative - equal to vaue of sign bit (7) 
            if ((tempMemoryContents & (1 << 7)) != 0) { SetPRegisterBit(7, 1); }
            else { SetPRegisterBit(7, 0); }
        }



        // BMI - Branch if Minus
        // If the negative flag is set then add the relative displacement to the program counter to cause a branch to a new location.
        // NV1B DIZC
        public void BMI()
        {
            if ((p & (1 << 7)) != 0)                            // If the negative flag is set
            {
                if ((pc >> 8) != (effectiveAddress >> 8))       // If page crossed,
                {
                    //Console.WriteLine("Page crossed - " + effectiveAddress);
                    Bus.cycleCount++;                           // Add extra cycle
                }
                SetPC(effectiveAddress);                        // Set PC to (PC + offset) calculated from AddressingMode_Relative()
                Bus.cycleCount++;
            }

        }



        // BNE - Branch if Not Equal
        // If the zero flag is clear then add the relative displacement to the program counter to cause a branch to a new location.
        // NV1B DIZC
        public void BNE()
        {
            if ((p & (1 << 1)) == 0)                            // If the zero flag is clear
            {
                if ((pc >> 8) != (effectiveAddress >> 8))       // If page crossed,
                {
                    //Console.WriteLine("Page crossed - " + effectiveAddress);
                    Bus.cycleCount++;                           // Add extra cycle
                }
                SetPC(effectiveAddress);                        // Set PC to (PC + offset) calculated from AddressingMode_Relative()
                Bus.cycleCount++;
            }

        }



        // BPL - Branch if Positive
        // If the negative flag is clear then add the relative displacement to the program counter to cause a branch to a new location.
        // NV1B DIZC
        public void BPL()
        {
            if ((p & (1 << 7)) == 0)                            // If the negative flag is clear
            {
                if ((pc >> 8) != (effectiveAddress >> 8))       // If page crossed,
                {
                    //Console.WriteLine("Page crossed - " + effectiveAddress);
                    Bus.cycleCount++;                           // Add extra cycle
                }
                SetPC(effectiveAddress);                        // Set PC to (PC + offset) calculated from AddressingMode_Relative()
                Bus.cycleCount++;
            }
        }



        // BRK - Force Interrupt
        // The BRK instruction forces the generation of an interrupt request. The program counter and processor status are pushed
        // on the stack then the IRQ interrupt vector at $FFFE/F is loaded into the PC and the break flag in the status set to one.
        // NV1B DIZC
        public void BRK()
        {
            IncrementPC();
            tempAddress = pc;
            tempLowOrderByte = tempAddress & 0x00FF;            // Grab the low order byte of PC
            tempHighOrderByte = tempAddress >> 8;               // Grab the high order byte of PC
            
            
            Bus.WriteBus(0x0100 + s, tempHighOrderByte, true);  // Push PC HOB onto the stack
            decrementStackPointer();

            Bus.WriteBus(0x0100 + s, tempLowOrderByte, true);   // Push PC HOB onto the stack
            decrementStackPointer();


            SetPRegisterBit(4, 1);                              // Set "B flag" in p register to be put on the stack
            
            Bus.WriteBus(0x0100 + s, p, true);                  // Push p onto the stack
            SetPRegisterBit(4, 0);                              // Reset "B flag" in p register because it doesn't really exist
            decrementStackPointer();

            SetPRegisterBit(2, 1);                              // Interrupts set the I flag

            tempLowOrderByte = Bus.ReadBus(0xFFFE, true);       // Read low order byte of IRQ vector
            tempAddress = Bus.ReadBus(0xFFFF, true);            // Read high order byte of IRQ vector
            tempAddress <<= 8;
            tempAddress += tempLowOrderByte;                    // Final IRQ vector

            SetPC(tempAddress);                                 // Set program counter to IRQ vector
            Bus.cycleCount++;                                

            
            
        }



        // BVC - Branch if Overflow Clear
        // If the overflow flag is clear then add the relative displacement to the program counter to cause a branch to a new location.
        // NV1B DIZC
        public void BVC()
        {
            if ((p & (1 << 6)) == 0)                            // If the overflow flag is clear
            {
                if ((pc >> 8) != (effectiveAddress >> 8))       // If page crossed,
                {
                    //Console.WriteLine("Page crossed - " + effectiveAddress);
                    Bus.cycleCount++;                           // Add extra cycle
                }
                SetPC(effectiveAddress);                        // Set PC to (PC + offset) calculated from AddressingMode_Relative()
                Bus.cycleCount++;
            }

        }



        // BVS - Branch if Overflow Set
        // If the overflow flag is set then add the relative displacement to the program counter to cause a branch to a new location.
        public void BVS()
        {
            if ((p & (1 << 6)) != 0)                            // If the overflow flag is set
            {
                if ((pc >> 8) != (effectiveAddress >> 8))       // If page crossed,
                {
                    //Console.WriteLine("Page crossed - " + effectiveAddress);
                    Bus.cycleCount++;                           // Add extra cycle
                }
                SetPC(effectiveAddress);                        // Set PC to (PC + offset) calculated from AddressingMode_Relative()
                Bus.cycleCount++;
            }
        }



        // CLC - Clear Carry Flag
        // Set the carry flag to zero.
        // C = 0
        // NV1B DIZC
        public void CLC()
        {
            SetPRegisterBit(0, 0);
            Bus.cycleCount++;

        }



        // CLD - Clear Decimal Mode
        // Sets the decimal mode flag to zero
        // D = 0
        // NV1B DIZC
        public void CLD()
        {
            SetPRegisterBit(3, 0);
            Bus.cycleCount++;
        }



        // CLI - Clear Interrupt Disable
        // Clears the interrupt disable flag allowing normal interrupt requests to be serviced.
        // I = 0
        // NV1B DIZC
        public void CLI()
        {
            SetPRegisterBit(2, 0);
            Bus.cycleCount++;
        }



        // CLV - Clear Overflow Flag
        // Clears the overflow flag
        // V = 0
        // NV1B DIZC
        public void CLV()
        {
            SetPRegisterBit(6, 0);
            Bus.cycleCount++;
        }



        // CMP - Compare
        // Compares the contents of the accumulator with another memory-held value and sets the zero and carry flags as appropriate
        // Z,C,N = A-M
        public void CMP()
        {
            workingData = Bus.ReadBus(effectiveAddress, true);
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



        // CPX - Compare X Register
        // This instruction compares the contents of the X register with another memory held value and sets the zero and carry flags as appropriate.
        // Z,C,N = X-M
        public void CPX()
        {
  
            tempResult = x - Bus.ReadBus(effectiveAddress, true);       // X - M
            
            // C: Carry - Set to contents of bit 7 before shift
            if ((tempResult >= 0)) { SetPRegisterBit(0, 1); }        // For compare operations with subtraction, set carry bit if there's                                                            
            else { SetPRegisterBit(0, 0); }                                     //   a "greater than or equal" result

            // Z: Zero 
            if (tempResult == 0) { SetPRegisterBit(1, 1); }
            else { SetPRegisterBit(1, 0); }


            // N: Negative - equal to vaue of sign bit (7) 
            if ((tempResult & (1 << 7)) != 0) { SetPRegisterBit(7, 1); }
            else { SetPRegisterBit(7, 0); }

        }



        // CPY - Compare Y Register
        // This instruction compares the contents of the Y register with another memory held value and sets the zero and carry flags as appropriate.
        // Z,C,N = Y-M
        public void CPY()
        {

            tempResult = y - Bus.ReadBus(effectiveAddress, true);       // Y - M

            // C: Carry - Set to contents of bit 7 before shift
            if ((tempResult >= 0)) { SetPRegisterBit(0, 1); }           // For compare operations with subtraction, set carry bit if there's                                                            
            else { SetPRegisterBit(0, 0); }                             //   a "greater than or equal" result

            // Z: Zero 
            if (tempResult == 0) { SetPRegisterBit(1, 1); }
            else { SetPRegisterBit(1, 0); }


            // N: Negative - equal to vaue of sign bit (7) 
            if ((tempResult & (1 << 7)) != 0) { SetPRegisterBit(7, 1); }
            else { SetPRegisterBit(7, 0); }
        }



        // DEC - Decrement Memory
        // Subtracts one from the value held at a specified memory location setting the zero and negative flags as appropriate.
        // M,Z,N = M-1
        public void DEC()
        {

            workingData = Bus.ReadBus(effectiveAddress, true);      // Read-Modify-Write instructions read from the effective address,
            Bus.cycleCount++;                                       //    then write it back on the next cycle 

            workingData--;
            
            if (workingData == -1)
            { workingData = 255; }

            // Z: Zero 
            if (workingData == 0) { SetPRegisterBit(1, 1); }
            else { SetPRegisterBit(1, 0); }

            // N: Negative - equal to vaue of sign bit (7) 
            if ((workingData & (1 << 7)) != 0) { SetPRegisterBit(7, 1); }
            else { SetPRegisterBit(7, 0); }

            Bus.WriteBus(effectiveAddress, workingData, true);

        }



        //DEX - Decrement X Register
        // Subtracts one from the X register setting the zero and negative flags as appropriate.
        // X, Z, N = X - 1
        public void DEX()
        {
            workingData = x;
            workingData--;
            if (workingData == -1) {  workingData = 255; }              // Correct for wraparound  
            

            // Z: Zero 
            if (workingData == 0) { SetPRegisterBit(1, 1); }
            else { SetPRegisterBit(1, 0); }

            // N: Negative - equal to vaue of sign bit (7) 
            if ((workingData & (1 << 7)) != 0) { SetPRegisterBit(7, 1); }
            else { SetPRegisterBit(7, 0); }

            x = workingData;
            Bus.cycleCount++;
        }



        // DEY - Decrement Y Register
        // Subtracts one from the Y register setting the zero and negative flags as appropriate.
        // Y, Z, N = Y - 1
        public void DEY()
        {
            workingData = y;
            workingData--;
            if (workingData == -1) { workingData = 255; }              // Correct for wraparound


            // Z: Zero 
            if (workingData == 0) { SetPRegisterBit(1, 1); }
            else { SetPRegisterBit(1, 0); }

            // N: Negative - equal to vaue of sign bit (7) 
            if ((workingData & (1 << 7)) != 0) { SetPRegisterBit(7, 1); }
            else { SetPRegisterBit(7, 0); }

            y = workingData;
            Bus.cycleCount++;
        }



        // EOR - Exclusive OR
        // An exclusive OR is performed, bit by bit, on the accumulator contents using the contents of a byte of memory
        // A,Z,N = A^M
        public void EOR()
        {
            workingData = Bus.ReadBus(effectiveAddress, true);
            a ^= workingData;

            // Z: Zero 
            if (a == 0) { SetPRegisterBit(1, 1); }
            else { SetPRegisterBit(1, 0); }

            // N: Negative - equal to vaue of sign bit (7) 
            if ((a & (1 << 7)) != 0) { SetPRegisterBit(7, 1); }
            else { SetPRegisterBit(7, 0); }
        }  // End EOR()



        // INC - Increment Memory
        // Adds one to the value held at a specified memory location setting the zero and negative flags as appropriate.
        // M,Z,N = M+1
        public void INC()
        {
            workingData = Bus.ReadBus(effectiveAddress, true);      // Read-Modify-Write instructions read from the effective address,
            Bus.cycleCount++;                                       //    then write it back on the next cycle 

            workingData++;

            if (workingData == 256)
            { workingData = 0; }

            // Z: Zero 
            if (workingData == 0) { SetPRegisterBit(1, 1); }
            else { SetPRegisterBit(1, 0); }

            // N: Negative - equal to vaue of sign bit (7) 
            if ((workingData & (1 << 7)) != 0) { SetPRegisterBit(7, 1); }
            else { SetPRegisterBit(7, 0); }

            Bus.WriteBus(effectiveAddress, workingData, true);


        }



        // INX - Increment X Register
        // Adds one to the X register setting the zero and negative flags as appropriate.
        // X,Z,N = X+1
        public void INX()
        {
            workingData = x;
            workingData++;
            if (workingData == 256) { workingData = 0; }              // Correct for wraparound  


            // Z: Zero 
            if (workingData == 0) { SetPRegisterBit(1, 1); }
            else { SetPRegisterBit(1, 0); }

            // N: Negative - equal to vaue of sign bit (7) 
            if ((workingData & (1 << 7)) != 0) { SetPRegisterBit(7, 1); }
            else { SetPRegisterBit(7, 0); }

            x = workingData;
            Bus.cycleCount++;
        }



        // INY - Increment Y Register
        // Adds one to the Y register setting the zero and negative flags as appropriate.
        // Y,Z,N = Y+1
        public void INY()
        {
            workingData = y;
            workingData++;
            if (workingData == 256) { workingData = 0; }              // Correct for wraparound


            // Z: Zero 
            if (workingData == 0) { SetPRegisterBit(1, 1); }
            else { SetPRegisterBit(1, 0); }

            // N: Negative - equal to vaue of sign bit (7) 
            if ((workingData & (1 << 7)) != 0) { SetPRegisterBit(7, 1); }
            else { SetPRegisterBit(7, 0); }

            y = workingData;
            Bus.cycleCount++;
        }



        // JMP - Jump
        // Sets the program counter to the address specified by the operand.
        public void JMP()
        {
            pc = effectiveAddress;
            //Bus.cycleCount++;
        }



        // JSR - Jump to Subroutine
        // The JSR instruction pushes the address (minus one) of the return point on to the stack and then
        // sets the program counter to the target memory address.
        public void JSR()
        {
                                                           
            tempAddress = (pc - 1);                             // We want to return to the memory location of the originial operand
            tempLowOrderByte = tempAddress & 0x00FF;            // Grab the low order byte of PC
            tempHighOrderByte = tempAddress >> 8;               // Grab the high order byte of PC

            Bus.cycleCount++;                                   // Discarded stack pointer read

            Bus.WriteBus(0x0100 + s, tempHighOrderByte, true);  // Push PC HOB onto the stack
            decrementStackPointer();

            Bus.WriteBus(0x0100 + s, tempLowOrderByte, true);   // Push PC LOB onto the stack
            decrementStackPointer();

            SetPC(effectiveAddress);

        }



        // LDA - Load Accumulator
        // Loads a byte of memory into the accumulator setting the zero and negative flags as appropriate
        // A,Z,N, = M
        public void LDA()
        {
            workingData = Bus.ReadBus(effectiveAddress, true);
            a = workingData;

            // Z: Zero 
            if (a == 0) { SetPRegisterBit(1, 1); }
            else { SetPRegisterBit(1, 0); }

            // N: Negative - equal to vaue of sign bit (7) 
            if ((a & (1 << 7)) != 0) { SetPRegisterBit(7, 1); }
            else { SetPRegisterBit(7, 0); }
        }  // End LDA()



        // LDX - Load X Register
        // Loads a byte of memory into the X register setting the zero and negative flags as appropriate.
        // X,Z,N = M
        public void LDX()
        {
            x = Bus.ReadBus(effectiveAddress, true);

            // Z: Zero 
            if (x == 0) { SetPRegisterBit(1, 1); }
            else { SetPRegisterBit(1, 0); }

            // N: Negative - equal to vaue of sign bit (7) 
            if ((x & (1 << 7)) != 0) { SetPRegisterBit(7, 1); }
            else { SetPRegisterBit(7, 0); }
        }



        // LDY - Load Y Register
        // Loads a byte of memory into the Y register setting the zero and negative flags as appropriate.
        // Y,Z,N = M
        public void LDY()
        {
            y = Bus.ReadBus(effectiveAddress, true);

            // Z: Zero 
            if (y == 0) { SetPRegisterBit(1, 1); }
            else { SetPRegisterBit(1, 0); }

            // N: Negative - equal to vaue of sign bit (7) 
            if ((y & (1 << 7)) != 0) { SetPRegisterBit(7, 1); }
            else { SetPRegisterBit(7, 0); }
        }



        // LSR - Logical Shift Right
        // Each of the bits in A or M is shift one place to the right. The bit that was in bit 0 is shifted into the carry flag. Bit 7 is set to zero.
        // A,C,Z,N = A/2 or M,C,Z,N = M/2
        public void LSR()
        {

            if (currentOpcode == 0x4A)      // Accumulator addressing mode
            {
                workingData = a;            // Load data from accumulator
                Bus.cycleCount++;           // Increment cycle count
                this.pc--;                  // One byte instruction, don't need the next PC increment at the end of CPU::ExecuteInstruction()
            }

            else
            {
                workingData = Bus.ReadBus(effectiveAddress, true);       // All other modes 
                Bus.cycleCount++;                                        // Read-Modify-Write instructions read from the effective address,
            }                                                            //    then write it back on the next cycle

            // C: Carry - Set to contents of bit 0 before shift
            if ((workingData & 1) != 0) { SetPRegisterBit(0, 1); }
            else { SetPRegisterBit(0, 0); }

            workingData >>= 1;                            // Shift right by 1 bit
            //workingData &= ~(1 << 8);                   // Clear bit 8 

            // Z: Zero 
            if (workingData == 0) { SetPRegisterBit(1, 1); }
            else { SetPRegisterBit(1, 0); }

            // N: Negative - equal to vaue of sign bit (7) 
            if ((workingData & (1 << 7)) != 0) { SetPRegisterBit(7, 1); }
            else { SetPRegisterBit(7, 0); }

            // Write back to accumulator or memory location
            if (currentOpcode == 0x4A)
            {
                a = workingData;
            }
            else { Bus.WriteBus(effectiveAddress, workingData, true); }


        }



        // NOP - No Operation
        // The NOP instruction causes no changes to the processor other than the normal incrementing of the program counter to the next instruction.
        public void NOP()
        {
            Bus.cycleCount++;
        }



        // ORA - Loigical Inclusive OR
        // An inclusive OR is performed, bit by bit, on the accumulator contents using the contents of a byte of memory
        // A,Z,N = A|M
        public void ORA()
        {
            workingData = Bus.ReadBus(effectiveAddress, true);
            a |= workingData;

            // Z: Zero 
            if (a == 0) { SetPRegisterBit(1, 1); }
            else { SetPRegisterBit(1, 0); }

            // N: Negative - equal to vaue of sign bit (7) 
            if ((a & (1 << 7)) != 0) { SetPRegisterBit(7, 1); }
            else { SetPRegisterBit(7, 0); }

        }  // End ORA()



        // PHA - Push Accumulator
        // Pushes a copy of the accumulator onto the stack
        public void PHA()
        {
            Bus.cycleCount++;                           // CYCLE 2 - discarded read of PC+1
            Bus.WriteBus(0x0100 + s, a, true);          // CYCLE 3 - Copy contents of a to address in stack
            decrementStackPointer();                    // Decrement stack pointer, descending stack
            
        }



        // PHP - Push Processor Status
        // Pushes a copy of the status flags on to the stack.
        // The B flag is pushed as 1 during PHP and BRK instructions, but bit 4 is not actually set in the P register
        public void PHP()
        {
            Bus.cycleCount++;                           // CYCLE 2 - discarded read of PC+1
            Bus.WriteBus(0x0100 + s, p+16, true);       // CYCLE 3 - Copy contents of p to address in stack, with the B flag set (+16)
            decrementStackPointer();                    // Decrement stack pointer, descending stack
        }



        // PLA - Pull Accumulator
        // Pulls an 8 bit value from the stack and into the accumulator. The zero and negative flags are set as appropriate.
        public void PLA()
        {
            Bus.cycleCount++;                           // CYCLE 2 - discarded read of PC+1
            Bus.cycleCount++;                           // CYCLE 3 - discarded read of Stack pointer
            incrementStackPointer();                    
            a = Bus.ReadBus(0x0100 + s, true);          // CYCLE 4 - Pop stack contents into accumulator

            // Z: Zero 
            if (a == 0) { SetPRegisterBit(1, 1); }
            else { SetPRegisterBit(1, 0); }

            // N: Negative - equal to vaue of sign bit (7) 
            if ((a & (1 << 7)) != 0) { SetPRegisterBit(7, 1); }
            else { SetPRegisterBit(7, 0); }
        }



        // PLP - Pull Processor Status
        // Pulls an 8 bit value from the stack and into the processor flags. The flags will take on new states as determined by the value pulled.
        public void PLP()
        {
            Bus.cycleCount++;                           // CYCLE 2 - discarded read of PC+1
            Bus.cycleCount++;                           // CYCLE 3 - discarded read of Stack pointer
            incrementStackPointer();
            p = Bus.ReadBus(0x0100 + s, true);          // CYCLE 4 - Pop stack contents into p register
            SetPRegisterBit(4, 0);                      // Bits 4 and 5 in the P register aren't real, 
            SetPRegisterBit(5, 1);                      //    So they'll be set to their "defaults"

        }



        // ROL - Rotate Left
        // Move each of the bits in either A or M one place to the left. Bit 0 is filled with the
        // current value of the carry flag whilst the old bit 7 becomes the new carry flag value.
        public void ROL()
        {

            if (currentOpcode == 0x2A)      // Accumulator addressing mode
            {
                workingData = a;            // Load data from accumulator
                Bus.cycleCount++;           // Increment cycle count
                this.pc--;                  // One byte instruction, don't need the next PC increment at the end of CPU::ExecuteInstruction()
            }

            else
            {
                workingData = Bus.ReadBus(effectiveAddress, true);       // All other modes 
                Bus.cycleCount++;                                        // Read-Modify-Write instructions read from the effective address,
            }                                                            //    then write it back on the next cycle

            // Store value of carry bit for later
            if ((p & (1 << 0)) != 0) { tempBitValue = 1; }
            else { tempBitValue = 0; }
            //Console.WriteLine(tempBitValue);

            // C: Carry - Set to contents of bit 7 before shift
            if ((workingData & (1 << 7)) != 0) { SetPRegisterBit(0, 1); }
            else { SetPRegisterBit(0, 0); }

            workingData <<= 1;                          // Shift left by 1 bit
            workingData &= ~(1 << 8);                   // Clear bit 8 

            // Set bit 0 to old value of carry bit
            if (tempBitValue == 0)
            { workingData &= ~(1 << 0); }
            else
            { workingData |= (1 << 0); }
            
            // Z: Zero
            if (workingData == 0) { SetPRegisterBit(1, 1); }
            else { SetPRegisterBit(1, 0); }

            // N: Negative - equal to vaue of sign bit (7) 
            if ((workingData & (1 << 7)) != 0) { SetPRegisterBit(7, 1); }
            else { SetPRegisterBit(7, 0); }

            // Write back to accumulator or memory location
            if (currentOpcode == 0x2A)
            {
                a = workingData;
            }
            else { Bus.WriteBus(effectiveAddress, workingData, true); }
        }



        // ROR - Rotate Right
        // Move each of the bits in either A or M one place to the right. Bit 7 is filled with the current value of the carry flag whilst the old bit 0 becomes the new carry flag value.
        public void ROR()
        {
            if (currentOpcode == 0x6A)      // Accumulator addressing mode
            {
                workingData = a;            // Load data from accumulator
                Bus.cycleCount++;           // Increment cycle count
                this.pc--;                  // One byte instruction, don't need the next PC increment at the end of CPU::ExecuteInstruction()
            }

            else
            {
                workingData = Bus.ReadBus(effectiveAddress, true);       // All other modes 
                Bus.cycleCount++;                                        // Read-Modify-Write instructions read from the effective address,
            }                                                            //    then write it back on the next cycle

            // Store value of carry bit for later
            if ((p & (1 << 0)) != 0) { tempBitValue = 1; }
            else { tempBitValue = 0; }
            //Console.WriteLine(tempBitValue);

            // C: Carry - Set to contents of bit 0 before shift
            if ((workingData & (1 << 0)) != 0) { SetPRegisterBit(0, 1); }
            else { SetPRegisterBit(0, 0); }

            workingData >>= 1;                          // Shift right by 1 bit
            //workingData &= ~(1 << 8);                   // Clear bit 8 

            // Set bit 7 to old value of carry bit
            if (tempBitValue == 0)
            { workingData &= ~(1 << 7); }
            else
            { workingData |= (1 << 7); }

            // Z: Zero
            if (workingData == 0) { SetPRegisterBit(1, 1); }
            else { SetPRegisterBit(1, 0); }

            // N: Negative - equal to vaue of sign bit (7) 
            if ((workingData & (1 << 7)) != 0) { SetPRegisterBit(7, 1); }
            else { SetPRegisterBit(7, 0); }

            // Write back to accumulator or memory location
            if (currentOpcode == 0x6A)
            {
                a = workingData;
            }
            else { Bus.WriteBus(effectiveAddress, workingData, true); }
        }
    



        // RTI - Return from Interrupt
        // The RTI instruction is used at the end of an interrupt processing routine. It pulls the
        // processor flags from the stack followed by the program counter.
        public void RTI()
        {
            Bus.cycleCount++;                                   // CYCLE 2 - Read second operand, discard
            Bus.cycleCount++;                                   // CYCLE 3 - Read stack, discard
            incrementStackPointer();
            p = Bus.ReadBus(0x0100 + s, true);                   // CYCLE 4 - Pop the p register value from the stack 

            SetPRegisterBit(4, 0);                              // Clear b flag even though it doesn't exist in p
            SetPRegisterBit(5, 1);                              // Set bit 5 even though it also doesn't exist in p

            incrementStackPointer();
            tempLowOrderByte = Bus.ReadBus(0x0100 + s, true);   // CYCLE 5 - Pop the low order byte of the PC

            incrementStackPointer();
            tempHighOrderByte = Bus.ReadBus(0x0100 + s, true);  // CYCLE 6 - Pop the high order byte of the PC
            

            pc = (tempHighOrderByte << 8) + tempLowOrderByte;   // Combine and store in PC

        }



        // RTS - Return from Subroutine
        // The RTS instruction is used at the end of a subroutine to return to the calling routine. It
        // pulls the program counter (minus one) from the stack.
        public void RTS()
        {
            Bus.cycleCount++;                                       // CYCLE 2 - Read second operand, discard
            Bus.cycleCount++;                                       // CYCLE 3 - Read stack, discard

            incrementStackPointer();
            tempLowOrderByte = Bus.ReadBus(0x0100 + s, true);       // CYCLE 4 - Pop the low order byte of the PC

            incrementStackPointer();
            tempHighOrderByte = Bus.ReadBus(0x0100 + s, true);      // CYCLE 5 - Pop the high order byte of the PC

            Bus.cycleCount++;                                       // CYCLE 6 - Stack read, discarded

            pc = ((tempHighOrderByte << 8) + tempLowOrderByte) + 1; // Combine (+1) and store in PC

        }



        // SBC - Subtract with Carry
        // Subtracts the contents of a memory location to the accumulator together with the not of the carry bit.
        // If overflow occurs the carry bit is clear, this enables multiple byte subtraction to be performed.
        // A,Z,C,N = A-M-(1-C)
        public void SBC()
        {
            workingData = Bus.ReadBus(effectiveAddress, true);

            aTemp = a;                                          // For calculating overflow flag

            // Subtracts the operand and NOT of carry bit from accumulator
            aTemp = a - workingData - (~p & 1);                 // This works because the carry bit is bit 0 in the P register
            int overflowTemp = aTemp;
         
            if (aTemp < 0)                                      // If result is negative, two's complement the absolute value 
            {
                int calcTemp = Math.Abs(aTemp);                     
                aTemp = ((~calcTemp) + 1);                      // Two's complement
                aTemp &= 0x1FF;                                 // Mask out everything but the lowest two bytes, excluding bit 8 for the carry test below
            }

            // P Register Flags

            // C: Carry - set if Bit 8 of the result is set (an int can hold this result)
            if ((aTemp & (1 << 8)) != 0)
            {
                SetPRegisterBit(0, 0);                          // Clear the carry bit in the p register
                aTemp &= ~(1 << 8);                             // Clear bit 8 in aTemp
            }
            else { SetPRegisterBit(0, 1); }

            // Z: Zero 
            if (aTemp == 0) { SetPRegisterBit(1, 1); }
            else { SetPRegisterBit(1, 0); }

            // V: Overflow
            // No Overflow if  (M^result) & (~N^result) & 0x80 != 0
            // Note that ~N is different from overclod calc of ADC instruction

            if (((a ^ aTemp) & (~workingData ^ aTemp) & 0x80) != 0)
            {
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

        }



        // SEC - Set Carry Flag
        // Set the carry flag to one.
        // C = 1
        // NV1B DIZC
        public void SEC()
        {
            SetPRegisterBit(0, 1);
            Bus.cycleCount++;

        }



        // SED - Set Decimal Flag
        // Set the decimal mode flag to one.
        // NV1B DIZC
        public void SED()
        {
            SetPRegisterBit(3, 1);
            Bus.cycleCount++;
        }



        // SEI - Set Interrupt Disable
        // Set the interrupt disable flag to one.
        // I = 1
        // NV1B DIZC
        public void SEI()
        {
            SetPRegisterBit(2, 1);
            Bus.cycleCount++;
        }



        // STA - Store Accumulator
        // Stores the contents of the accumulator into memory
        // M = A
        public void STA()   
        {
            Bus.WriteBus(effectiveAddress, a, true);
        } // End STA()



        // STX - Store X Register
        // Stores the contents of the X register into memory.
        // M = X
        public void STX()
        {
            Bus.WriteBus(effectiveAddress, x, true);
        }


        // STY - Store Y Register
        // Stores the contents of the Y register into memory.
        // M = Y
        public void STY()
        {
            Bus.WriteBus(effectiveAddress, y, true);
        }



        // TAX - Transfer Accumulator to X
        // Copies the current contents of the accumulator into the X register and sets the zero and negative flags as appropriate.
        // X = A
        // NV1B DIZC
        public void TAX()
        {
            x = a;
            Bus.cycleCount++;

            // Z: Zero
            if (x == 0) { SetPRegisterBit(1, 1); }
            else { SetPRegisterBit(1, 0); }

            // N: Negative - equal to vaue of sign bit (7) 
            if ((x & (1 << 7)) != 0) { SetPRegisterBit(7, 1); }
            else { SetPRegisterBit(7, 0); }

        }



        // TAY - Transfer Accumulator to Y
        // Copies the current contents of the accumulator into the Y register and sets the zero and negative flags as appropriate.
        // Y = A
        public void TAY()
        {
            y = a;
            Bus.cycleCount++;

            // Z: Zero
            if (y == 0) { SetPRegisterBit(1, 1); }
            else { SetPRegisterBit(1, 0); }

            // N: Negative - equal to vaue of sign bit (7) 
            if ((y & (1 << 7)) != 0) { SetPRegisterBit(7, 1); }
            else { SetPRegisterBit(7, 0); }

        }



        // TSX - Transfer Stack Pointer to X
        // Copies the current contents of the stack register into the X register and sets the zero and negative flags as appropriate.
        // X = S
        public void TSX()
        {
            x = s;
            Bus.cycleCount++;

            // Z: Zero
            if (x == 0) { SetPRegisterBit(1, 1); }
            else { SetPRegisterBit(1, 0); }

            // N: Negative - equal to vaue of sign bit (7) 
            if ((x & (1 << 7)) != 0) { SetPRegisterBit(7, 1); }
            else { SetPRegisterBit(7, 0); }
        }



        // TXA - Transfer X to Accumulator
        // Copies the current contents of the X register into the accumulator and sets the zero and negative flags as appropriate.
        // A = X
        public void TXA()
        {
            a = x;
            Bus.cycleCount++;

            // Z: Zero
            if (a == 0) { SetPRegisterBit(1, 1); }
            else { SetPRegisterBit(1, 0); }

            // N: Negative - equal to vaue of sign bit (7) 
            if ((a & (1 << 7)) != 0) { SetPRegisterBit(7, 1); }
            else { SetPRegisterBit(7, 0); }
        }



        // TXS - Transfer X to Stack Pointer
        // Copies the current contents of the X register into the stack register.
        // S = X
        public void TXS()
        {
            s = x;
            Bus.cycleCount++;

            // Z: Zero
            if (s == 0) { SetPRegisterBit(1, 1); }
            else { SetPRegisterBit(1, 0); }

            // N: Negative - equal to vaue of sign bit (7) 
            if ((s & (1 << 7)) != 0) { SetPRegisterBit(7, 1); }
            else { SetPRegisterBit(7, 0); }

        }



        // TYA - Transfer Y to Accumulator
        // Copies the current contents of the Y register into the accumulator and sets the zero and negative flags as appropriate.
        // A = Y
        public void TYA()
        {
            a = y;
            Bus.cycleCount++;

            // Z: Zero
            if (a == 0) { SetPRegisterBit(1, 1); }
            else { SetPRegisterBit(1, 0); }

            // N: Negative - equal to vaue of sign bit (7) 
            if ((a & (1 << 7)) != 0) { SetPRegisterBit(7, 1); }
            else { SetPRegisterBit(7, 0); }
        }
    }
}
