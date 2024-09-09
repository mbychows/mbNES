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

            // ADC - DONE
            //OpcodeTest.JSONTest("69");    
            //OpcodeTest.JSONTest("65");    
            //OpcodeTest.JSONTest("75");    
            //OpcodeTest.JSONTest("6D");    
            //OpcodeTest.JSONTest("7D");     
            //OpcodeTest.JSONTest("79");    
            //OpcodeTest.JSONTest("61");     
            //OpcodeTest.JSONTest("71");    

            // AND - DONE
            //OpcodeTest.JSONTest("29");        
            //OpcodeTest.JSONTest("25");    
            //OpcodeTest.JSONTest("35");
            //OpcodeTest.JSONTest("2D");
            //OpcodeTest.JSONTest("3D");
            //OpcodeTest.JSONTest("39");
            //OpcodeTest.JSONTest("21");
            //OpcodeTest.JSONTest("31");

            // CMP
            //OpcodeTest.JSONTest("C9");        
            //OpcodeTest.JSONTest("C5");    
            //OpcodeTest.JSONTest("D5");
            //OpcodeTest.JSONTest("CD");
            //OpcodeTest.JSONTest("DD");
            //OpcodeTest.JSONTest("D9");
            //OpcodeTest.JSONTest("C1");
            //OpcodeTest.JSONTest("D1");



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
