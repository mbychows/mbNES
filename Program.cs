using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace mbNES
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            //byte byte1 = 0xFF;
            //byte byte2 = 0xFF;
            //byte result = 0x00;

            //result = (byte)(byte1 + byte2);
            //Console.WriteLine(result);

            //CPU TestCPU = new CPU();

                        
            CPUTest TestCPU = new CPUTest();

            //TestCPU.TestAddressingMode_Absolute();
            //TestCPU.TestAddressingMode_ZeroPage();
            //TestCPU.TestAddressingMode_IndexedZeroPage();
            //TestCPU.TestAddressingMode_IndexedAbsolute();
            //TestCPU.TestAddressingMode_Relative();
            //TestCPU.TestAddressingMode_IndirectX();
            //TestCPU.TestAddressingMode_IndirectY();
            //TestCPU.TestAddressingMode_AbsoluteIndirect();

            //TestCPU.TestADC();
            OpcodeTest OpcodeTest = new OpcodeTest();

            //// ADC - DONE
            //OpcodeTest.JSONTest("69");
            //OpcodeTest.JSONTest("65");
            //OpcodeTest.JSONTest("75");
            //OpcodeTest.JSONTest("6D");
            //OpcodeTest.JSONTest("7D");
            //OpcodeTest.JSONTest("79");
            //OpcodeTest.JSONTest("61");
            //OpcodeTest.JSONTest("71");

            //// AND - DONE
            //OpcodeTest.JSONTest("29");
            //OpcodeTest.JSONTest("25");
            //OpcodeTest.JSONTest("35");
            //OpcodeTest.JSONTest("2D");
            //OpcodeTest.JSONTest("3D");
            //OpcodeTest.JSONTest("39");
            //OpcodeTest.JSONTest("21");
            //OpcodeTest.JSONTest("31");

            //// ASL
            //OpcodeTest.JSONTest("0A");
            //OpcodeTest.JSONTest("06");
            //OpcodeTest.JSONTest("16");
            //OpcodeTest.JSONTest("0E");
            //OpcodeTest.JSONTest("1E");

            //// BCC
            //OpcodeTest.JSONTest("90");

            //// BCS 
            //OpcodeTest.JSONTest("B0");

            //// BEQ 
            //OpcodeTest.JSONTest("F0");

            //// BMI 
            //OpcodeTest.JSONTest("30");

            //// BNE 
            //OpcodeTest.JSONTest("D0");

            //// BPL 
            //OpcodeTest.JSONTest("10");

            //// BVC 
            //OpcodeTest.JSONTest("50");

            //// BVS 
            //OpcodeTest.JSONTest("70");

            //// CLC - Commit
            //OpcodeTest.JSONTest("18");

            //// CLD - Commit
            //OpcodeTest.JSONTest("D8");

            //// CLI - Commit
            //OpcodeTest.JSONTest("58");

            //// CLV - Commit
            //OpcodeTest.JSONTest("B8");


            //// CMP
            //OpcodeTest.JSONTest("C9");
            //OpcodeTest.JSONTest("C5");
            //OpcodeTest.JSONTest("D5");
            //OpcodeTest.JSONTest("CD");
            //OpcodeTest.JSONTest("DD");
            //OpcodeTest.JSONTest("D9");
            //OpcodeTest.JSONTest("C1");
            //OpcodeTest.JSONTest("D1");

            //// DEC
            //OpcodeTest.JSONTest("C6");
            //OpcodeTest.JSONTest("D6");
            //OpcodeTest.JSONTest("CE");
            //OpcodeTest.JSONTest("DE");

            //// DEX - Commit
            //OpcodeTest.JSONTest("CA");


            //// DEY - Commit
            //OpcodeTest.JSONTest("88");

            //// EOR
            //OpcodeTest.JSONTest("49");
            //OpcodeTest.JSONTest("45");
            //OpcodeTest.JSONTest("55");
            //OpcodeTest.JSONTest("4D");
            //OpcodeTest.JSONTest("5D");
            //OpcodeTest.JSONTest("59");
            //OpcodeTest.JSONTest("41");
            //OpcodeTest.JSONTest("51");


            //// INC
            //OpcodeTest.JSONTest("E6");
            //OpcodeTest.JSONTest("F6");
            //OpcodeTest.JSONTest("EE");
            //OpcodeTest.JSONTest("FE");

            //// INX - Commit
            //OpcodeTest.JSONTest("E8");

            //// INY - Commit
            //OpcodeTest.JSONTest("C8");


            //// LDA
            //OpcodeTest.JSONTest("A9");
            //OpcodeTest.JSONTest("A5");
            //OpcodeTest.JSONTest("B5");
            //OpcodeTest.JSONTest("AD");
            //OpcodeTest.JSONTest("BD");
            //OpcodeTest.JSONTest("B9");
            //OpcodeTest.JSONTest("A1");
            //OpcodeTest.JSONTest("B1");

            //// LSR
            //OpcodeTest.JSONTest("4A");
            //OpcodeTest.JSONTest("46");
            //OpcodeTest.JSONTest("56");
            //OpcodeTest.JSONTest("4E");
            //OpcodeTest.JSONTest("5E");

            //// ORA
            //OpcodeTest.JSONTest("09");
            //OpcodeTest.JSONTest("05");
            //OpcodeTest.JSONTest("15");
            //OpcodeTest.JSONTest("0D");
            //OpcodeTest.JSONTest("1D");
            //OpcodeTest.JSONTest("19");
            //OpcodeTest.JSONTest("01");
            //OpcodeTest.JSONTest("11");

            //// ROL
            //OpcodeTest.JSONTest("2A");
            //OpcodeTest.JSONTest("26");
            //OpcodeTest.JSONTest("36");
            //OpcodeTest.JSONTest("2E");
            //OpcodeTest.JSONTest("3E");

            ////ROR
            //OpcodeTest.JSONTest("6A");
            //OpcodeTest.JSONTest("66");
            //OpcodeTest.JSONTest("76");
            //OpcodeTest.JSONTest("6E");
            //OpcodeTest.JSONTest("7E");

            ////SBC

            //// SEC - Commit
            //OpcodeTest.JSONTest("38");

            //// SED - Commit
            //OpcodeTest.JSONTest("F8");

            //// SEI - Commit
            //OpcodeTest.JSONTest("78");

            //// STA
            //OpcodeTest.JSONTest("85");
            //OpcodeTest.JSONTest("95");
            //OpcodeTest.JSONTest("8D");
            //OpcodeTest.JSONTest("9D");
            //OpcodeTest.JSONTest("99");
            //OpcodeTest.JSONTest("81");
            //OpcodeTest.JSONTest("91");












            //TestCPU.CombineBytes();
            //Bus.WriteBus(0x0000, 0x69);
            //Bus.WriteBus(0x0001, 0x65);
            //Bus.WriteBus(0x0002, 0x75);
            //Bus.WriteBus(0x0003, 0x6D);
            //Bus.WriteBus(0x0004, 0x7D);



            //TestCPU.ExecuteInstruction();
            //TestCPU.ExecuteInstruction();
            //TestCPU.ExecuteInstruction();
            //TestCPU.ExecuteInstruction();
            //TestCPU.ExecuteInstruction();

            /*
            BusTest test = new BusTest();
            test.WriteTest(0x0000, 0xFF);
            test.WriteTest(0x0001, 0x11);
            test.WriteTest(0xFFFF, 0xF1);
            test.WriteTest(0xFF11, 0x22);

            test.ReadTest(0x0000);
            test.ReadTest(0x0001);
            test.ReadTest(0xFFFF);
            test.ReadTest(0xFF11);
            */
            Console.ReadLine();
            //Application.EnableVisualStyles();
            //Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run(new Form1());
        }
    }
}
