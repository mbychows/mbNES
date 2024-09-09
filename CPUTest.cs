using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mbNES
{
    internal class CPUTest
    {
        

        //private ushort finalAddress = 0x0000;
        //private ushort tempAddress = 0x0000;
        //private byte tempLowOrderByte = 0xFF;
        //private byte tempHighOrderByte = 0x11;

        private int ExpectedEffectiveAddress = 0x0000;
        private int ComputedEffectiveAddress = 0x0000;

        CPU TestCPU = new CPU();

        public CPUTest()
        {


        }

        public void PrintRegisters(string format)
        {
            if (format == "hex")
            {
                Console.WriteLine("a:  " + TestCPU.a.ToString("x2"));
                Console.WriteLine("x:  " + TestCPU.x.ToString("x2"));
                Console.WriteLine("y:  " + TestCPU.y.ToString("x2"));
                Console.WriteLine("s:  " + TestCPU.s.ToString("x2"));
                Console.WriteLine("p:  " + TestCPU.p.ToString("x2"));
                Console.WriteLine("pc: " + TestCPU.pc.ToString("x4"));
                Console.WriteLine("\n");
            }

            else
            {
                Console.WriteLine("a:  " + TestCPU.a);
                Console.WriteLine("x:  " + TestCPU.x);
                Console.WriteLine("y:  " + TestCPU.y);
                Console.WriteLine("s:  " + TestCPU.s);
                Console.WriteLine("p:  " + TestCPU.p);
                Console.WriteLine("pc: " + TestCPU.pc);
                Console.WriteLine("\n");
            }
        }

        public void PrintPC()
        {
            Console.WriteLine("pc:  " + TestCPU.pc);
        }




        public void PrintMemoryContents(int startingAddress, int range)
        {
            for (int i = 0; i < range; i++)
            {
                Console.WriteLine("0x" + (startingAddress + i).ToString("x4") + ": " + Bus.ReadBus((ushort)(startingAddress + i)).ToString("x2"));
            }
            Console.WriteLine("\n");
        }




        public void PrintEffectiveAddress()
        {
            Console.WriteLine("Expected effective address: " + ExpectedEffectiveAddress.ToString("x4"));
            Console.WriteLine("Effective address: " + TestCPU.effectiveAddress.ToString("x4"));
        }


        public void SetupOpcodeTest()
        {
            TestCPU.SetPC(49119);
            TestCPU.SetRegisters(76,36,4,46,228);
            Bus.WriteBus(49119, 105);
            Bus.WriteBus(49120, 27);
            Bus.WriteBus(49121, 145);

            // Initial
            // "pc": 49119
            // "s": 46
            // "a": 76
            // "x": 36
            // "y": 4
            // "p": 228
            // "ram": [49119, 105]
            //        [49120, 27]
            //        [49121, 145] 

            // Final
            // "pc": 49121
            // "s": 46
            // "a": 103
            // "x": 36
            // "y": 4
            // "p": 36
            // "ram": [49119, 105]
            //        [49120, 27]
            //        [49121, 145]]}


            // "final": { "pc": 49121, "s": 46, "a": 103, "x": 36, "y": 4, "p": 36, "ram": [ [49119, 105], [49120, 27], [49121, 145]]},

        }




        // IMMEDIATE - In immediate addressing, the operand is contained in the second byte of the instruction,
        // with no further memory addressing required.
        private void TestAddressingMode_Immediate()
        {
            // Probably not needed
            Bus.WriteBus(0x0000, 0xC9);   // CMP Immediate
            // Bus.WriteBus(0x0001, 0x);   // Next address holds the operand
        }


        //  ABSOLUTE - In absolute addressing, the second byte of the instruction specifies the eight low order
        //  bits of the effective address while the third byte specifies the eight high order bits.Thus, the
        //  absolute addressing mode allows access to the entire 65K bytes of addressable memory.
        public void TestAddressingMode_Absolute()
        {
            Bus.WriteBus(0x0000, 0x4C); // JMP Absolute
            Bus.WriteBus(0x0001, 0x53); // Low order bits of address
            Bus.WriteBus(0x0002, 0xF1); // High order bits of address
            ExpectedEffectiveAddress = 0xF153;

            PrintMemoryContents(0x0000, 10);
            TestCPU.AddressingMode_Absolute();
            PrintRegisters("hex");
            PrintEffectiveAddress();

        }

        //  ZERO PAGE - The zero page instructions allow for shorter code and execution times by only fetching
        //  the second byte of the instruction and assuming a zero high address byte. Careful use of the zero
        //  page can result in significant increase in code efficiency.
        public void TestAddressingMode_ZeroPage()
        {
            Bus.WriteBus(0x0000, 0xC6); //  DEC Zero Page
            Bus.WriteBus(0x0001, 0x69); // Low order bits of address
            ExpectedEffectiveAddress = 0x0069;

            PrintMemoryContents(0x0000, 10);
            TestCPU.AddressingMode_Absolute();
            PrintRegisters("hex");
            PrintEffectiveAddress();

        }


        //  INDEXED ZERO PAGE - (X, Y indexing) - This form of addressing is used in conjunction with the index
        //  register and is referred to as "Zero Page, X" or "Zero Page, Y". The effective address is calculated
        //  by adding the second byte to the contents of the index register.Since this is a form of "Zero Page"
        //  addressing, the content of the second byte references a location in page zero. Additionally due to
        //  the "Zero Page" addressing nature of this mode, no carry is added to the high order 8 bits of memory
        //  and crossing of page boundaries does not occur.
        public void TestAddressingMode_IndexedZeroPage()
        {
            Bus.WriteBus(0x0000, 0xF6);     // INC Zero Page, X
            Bus.WriteBus(0x0001, 0xF0);
            TestCPU.SetRegisters(0, 0xF1, 0, 0, 0);

            int register = TestCPU.x;
            ExpectedEffectiveAddress = 0x00E1;
            PrintMemoryContents(0x0000, 10);
            PrintRegisters("hex");
            TestCPU.AddressingMode_IndexedZeroPage(ref register);
            PrintEffectiveAddress();

            // Reset for next test
            Bus.ResetBus();
            TestCPU.ResetCPU();


            Bus.WriteBus(0x0000, 0xB6);     // LDX Zero Page, Y
            Bus.WriteBus(0x0001, 0xBB);
            TestCPU.SetRegisters(0, 0, 0xFA, 0, 0);


            register = TestCPU.y;
            ExpectedEffectiveAddress = 0x00B5;
            PrintMemoryContents(0x0000, 10);
            PrintRegisters("hex");
            TestCPU.AddressingMode_IndexedZeroPage(ref register);
            PrintEffectiveAddress();
        }

        //  INDEXED ABSOLUTE - (X, Y indexing) - This form of addressing is used in conjunction with X and Y
        //  index register and is referred to as "Absolute, X", and "Absolute, Y". The effective address is
        //  formed by adding the contents of X or Y to the address contained in the second and third bytes of the
        //  instruction.This mode allows the index register to contain the index or count value and the in
        //  struction to contain the base address.This type of indexing allows any location referencing and
        //  the index to modify multiple fields resulting in reduced coding and execution time.
        public void TestAddressingMode_IndexedAbsolute()   // need to check for page boundary crossing
        {
            Bus.WriteBus(0x0000, 0x3D);     // AND Absolute, X
            Bus.WriteBus(0x0001, 0xFE);
            Bus.WriteBus(0x0002, 0x31);
            TestCPU.SetRegisters(0, 0xAB, 0, 0, 0); // Set x register

            int register = TestCPU.x;
            ExpectedEffectiveAddress = 0x32A9;
            PrintMemoryContents(0x0000, 10);
            PrintRegisters("hex");
            TestCPU.AddressingMode_IndexedAbsolute(ref register);
            PrintEffectiveAddress();

            // Reset for next test
            Bus.ResetBus();
            TestCPU.ResetCPU();

            Bus.WriteBus(0x0000, 0x39);     // AND Absolute, Y
            Bus.WriteBus(0x0001, 0xFE);
            Bus.WriteBus(0x0002, 0x31);
            TestCPU.SetRegisters(0, 0, 0xAB, 0, 0); // Set Y register

            register = TestCPU.y;
            ExpectedEffectiveAddress = 0x32A9;
            PrintMemoryContents(0x0000, 10);
            PrintRegisters("hex");
            TestCPU.AddressingMode_IndexedAbsolute(ref register);
            PrintEffectiveAddress();

        }

        //  RELATIVE ADDRESSING - Relative addressing is used only with branch instructions and
        //  establishes a destination for the conditional branch.
        //  The second byte of the instruction becomes the operand which is an "Offset" added to the contents of
        //  the lower eight bits of the program counter when the counter is set at the next instruction.The
        //  range of the offset is -128 to +127 bytes from the next instruction.
        public void TestAddressingMode_Relative()  // Check for page boundary crossing and negative PC values
        {
            TestCPU.SetPC(0x0255);
            Bus.WriteBus(0x0255, 0xF0);     // BEQ, Relative
            Bus.WriteBus(0x0256, 0xBB);     // Program counter offset -69 (-0x3B)

            ExpectedEffectiveAddress = 0x021C;  // 0257-3B
            PrintMemoryContents(0x0255, 10);
            PrintRegisters("hex");
            TestCPU.AddressingMode_Relative();
            PrintEffectiveAddress();
            Console.WriteLine("Program Counter: " + TestCPU.pc.ToString("x4"));

        }


        //  INDEXED INDIRECT ADDRESSING - In indexed indirect addressing (referred to as (Indirect,X)), the second byte of
        //  the instruction is added to the contents of the X index register, discarding the carry. The result
        //  of this addition points to a memory location on page zero whose contents is the low order eight bits
        //  of the effective address.The next memory location in page zero contains the high order eight bits
        //  of the effective address. Both memory locations specifying the high and low order bytes of the
        //  effective address must be in page zero.
        public void TestAddressingMode_IndirectX()
        {
            Bus.WriteBus(0x0000, 0x61);     // ADC - Indirect, X
            Bus.WriteBus(0x0001, 0x11);
            Bus.WriteBus(0x0066, 0x31);     // 0x11 + 0x55 = 0x66
            Bus.WriteBus(0x0067, 0xAA);
            TestCPU.SetRegisters(0, 0x55, 0, 0, 0); // Set X register

            ExpectedEffectiveAddress = 0xAA31;
            // PrintMemoryContents(0x0255, 10);
            PrintRegisters("hex");
            TestCPU.AddressingMode_IndirectX();
            PrintEffectiveAddress();


        }


        //  INDIRECT INDEXED ADDRESSING - In indirect indexed addressing (referred to as (Indirect),Y), the second byte
        //  of the instruction points to a memory location in page zero.The contents of this memory location
        //  is added to the contents of the Y index register, the result being the low order eight bits of the
        //  effective address. The carry from this addition is added to the contents of the next pagezero
        //  memory location, the result being the high order eight bits of the effective address.
        public void TestAddressingMode_IndirectY()  // done?
        {

            Bus.WriteBus(0x0000, 0x71);     // ADC - Indirect, Y
            Bus.WriteBus(0x0001, 0x11);
            Bus.WriteBus(0x0011, 0x31);
            Bus.WriteBus(0x0012, 0xAA);
            TestCPU.SetRegisters(0, 0, 0x22, 0, 0); // Set Y register

            ExpectedEffectiveAddress = 0xAA53;
            // PrintMemoryContents(0x0255, 10);
            PrintRegisters("hex");
            TestCPU.AddressingMode_IndirectY();
            PrintEffectiveAddress();
            PrintRegisters("hex");

        }

        //  ABSOLUTE INDIRECT - The second byte of the instruction contains the low order eight bits of a memory location.
        //  The high order eight bits of that memory location is contained in the third byte of the instruction.
        //  The contents of the fully specified memory location is the low order byte of the effective address.
        //  The next memory location contains the high order byte of the effective address which is loaded
        //  into the sixteen bits of the program counter.
        public void TestAddressingMode_AbsoluteIndirect() 
        {
            Bus.WriteBus(0x0000, 0x6D);     // ADC - Absolute
            Bus.WriteBus(0x0001, 0x22);
            Bus.WriteBus(0x0002, 0x01);
            Bus.WriteBus(0x0122, 0x12);
            Bus.WriteBus(0x0123, 0x34);

            TestCPU.SetRegisters(0, 0, 0, 0, 0); // Set Y register

            ExpectedEffectiveAddress = 0x3412;
            // PrintMemoryContents(0x0255, 10);
            PrintRegisters("hex");
            TestCPU.AddressingMode_AbsoluteIndirect();
            PrintEffectiveAddress();
           
        }


        public void TestADC()
        {
            TestCPU.SetPC(49119);
            TestCPU.SetRegisters(76, 36, 4, 46, 228);
            Bus.WriteBus(49119, 105);
            Bus.WriteBus(49120, 27);
            Bus.WriteBus(49121, 145);

            Console.WriteLine("Initial:");
            PrintRegisters("dec");
            PrintMemoryContents(49119, 3);

            TestCPU.ExecuteInstruction();

            Console.WriteLine("Final:");
            PrintRegisters("dec");
            PrintMemoryContents(49119, 3);


        }



    }
}
