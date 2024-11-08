using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;

namespace mbNES
{
    
    /*
    internal class OpcodeTestJSON
    {
        [JsonProperty("OpcodeTest")]
        public OpcodeTest OpcodeTest { get; set; }
    }
    */

    public class OpcodeTestCase
    {
        [JsonProperty("name")]
        public string TestName { get; set; }

        [JsonProperty("initial")]
        public InitialState InitialState { get; set; }

        [JsonProperty("final")]
        public FinalState FinalState { get; set; }

        [JsonProperty("cycles")]
        public List<List<object>> Cycles { get; set; }

    }


    public class InitialState
    {
        [JsonProperty("pc")]
        public int pc {  get; set; }

        [JsonProperty("s")]
        public int s {  get; set; }

        [JsonProperty("a")]
        public int a {  get; set; }

        [JsonProperty("x")]
        public int x { get; set; }

        [JsonProperty("y")]
        public int y { get; set; }

        [JsonProperty("p")]
        public int p { get; set; }

        [JsonProperty("ram")]
        public List<List<int>> ram { get; set; }
    }


    public class FinalState
    {
        [JsonProperty("pc")]
        public int pc { get; set; }

        [JsonProperty("s")]
        public int s { get; set; }

        [JsonProperty("a")]
        public int a { get; set; }

        [JsonProperty("x")]
        public int x { get; set; }

        [JsonProperty("y")]
        public int y { get; set; }

        [JsonProperty("p")]
        public int p { get; set; }

        [JsonProperty("ram")]
        public List<List<int>> ram { get; set; }
    }

    public class OpcodeTest
    {

        CPU TestCPU = new CPU();
        public void PrintRegisters(string format)
        {
            
            if (format == "hex")
            {
                Console.WriteLine("a:  " + TestCPU.a.ToString("x2"));
                Console.WriteLine("x:  " + TestCPU.x.ToString("x2"));
                Console.WriteLine("y:  " + TestCPU.y.ToString("x2"));
                Console.WriteLine("s:  " + TestCPU.s.ToString("x2"));
                Console.WriteLine("p:  " + TestCPU.p.ToString("x2") + " - " + Convert.ToString(TestCPU.p, 2));
                Console.WriteLine("pc: " + TestCPU.pc.ToString("x4"));
                Console.WriteLine("\n");
            }

            else
            {
                Console.WriteLine("a:  " + TestCPU.a);
                Console.WriteLine("x:  " + TestCPU.x);
                Console.WriteLine("y:  " + TestCPU.y);
                Console.WriteLine("s:  " + TestCPU.s);
                Console.WriteLine("p:  " + TestCPU.p + " - " + Convert.ToString(TestCPU.p, 2));
                Console.WriteLine("pc: " + TestCPU.pc);
                Console.WriteLine("\n");
            }
        }

        public void PrintPC()
        {
            Console.WriteLine("pc:  " + TestCPU.pc);
        }

        public void PrintMemoryContents(int startingAddress, int range, string format)
        {
            for (int i = 0; i < range; i++)
            {
                if (format == "hex")
                {
                    Console.WriteLine("0x" + (startingAddress + i).ToString("x4") + ": " + Bus.ReadBus((ushort)(startingAddress + i)).ToString("x2"));
                }
                else
                {
                    Console.WriteLine( (startingAddress + i) + ": " + Bus.ReadBus( (startingAddress + i)));
                }
            }
            Console.WriteLine("\n");
        }

        public async Task JSONTest(string opcode)
        {
            //int testNumber = 3;
            int testsFailed = 0;
            bool testFailed;

            List<OpcodeTestCase> Tests = JsonConvert.DeserializeObject<List<OpcodeTestCase>>(File.ReadAllText(@"C:\Users\matthew.bychowski\source\repos\mbNES\Test Files\" + opcode + ".json"));


            //    "initial": { "pc": 49119
            // "s": 46
            //  "a": 76
            //  "x": 36
            //  "y": 4
            //  "p": 228
            //  "ram": [ [49119, 105], [49120, 27], [49121, 145]]}

            for (int testNumber = 0; testNumber < Tests.Count; testNumber++)
            {
                testFailed = false;

                //
                // Initial setup
                //

                // PC and registers 
                TestCPU.SetPC(Tests[testNumber].InitialState.pc);
                TestCPU.SetRegisters(Tests[testNumber].InitialState.a, Tests[testNumber].InitialState.x, Tests[testNumber].InitialState.y, Tests[testNumber].InitialState.s, Tests[testNumber].InitialState.p);
                // RAM
                for (int i = 0; i < Tests[testNumber].InitialState.ram.Count; i++)
                {
                    Bus.WriteBus(Tests[testNumber].InitialState.ram[i][0], Tests[testNumber].InitialState.ram[i][1]);
                }

                // Print the initial state
                //Console.WriteLine("INITIAL STATE:");
                //PrintRegisters("int");
                //PrintRegisters("hex");
                //PrintPC();
                //PrintMemoryContents(Tests[testNumber].InitialState.ram[0][0], Tests[testNumber].InitialState.ram.Count, "int");
                //PrintMemoryContents(Tests[testNumber].InitialState.ram[0][0], Tests[testNumber].InitialState.ram.Count, "hex");
                //Console.WriteLine("\n");


                //
                // Execution
                //
                TestCPU.ExecuteInstruction();


                //
                // Expected final values checked against computed values 
                //

                // Check registers and PC
                if (TestCPU.pc != Tests[testNumber].FinalState.pc) { testFailed = true; }
                if (TestCPU.s != Tests[testNumber].FinalState.s) { testFailed = true; }
                if (TestCPU.a != Tests[testNumber].FinalState.a) { testFailed = true; }
                if (TestCPU.x != Tests[testNumber].FinalState.x) { testFailed = true; }
                if (TestCPU.y != Tests[testNumber].FinalState.y) { testFailed = true; }
                if (TestCPU.p != Tests[testNumber].FinalState.p) { testFailed = true; }

                // Check memory contents
                for (int i = 0; i < Tests[testNumber].FinalState.ram.Count; i++)
                {
                    // FinalState.ram[i][0] is the memory location, FinalState.ram[i][1] is the value
                    if (Bus.ReadBus(Tests[testNumber].FinalState.ram[i][0]) != Tests[testNumber].FinalState.ram[i][1]) { testFailed = true; }
                }

                if (Bus.cycleCount != Tests[testNumber].Cycles.Count) { testFailed = true; }

                // Increment the number of whole tests failed if any value didn't come out as expected
                if (testFailed)
                {
                    testsFailed++;
                    Console.WriteLine("Test failed: " + Tests[testNumber].TestName);
                    


                    // Check the results against the expected values
                    // This way of checking sucks but I'll refactor later... or maybe not

                    if (TestCPU.pc == Tests[testNumber].FinalState.pc) { Console.WriteLine("pc: PASS"); }
                    else
                    {
                        Console.WriteLine("pc: FAIL");
                        Console.WriteLine("Expected pc: " + Tests[testNumber].FinalState.pc);
                        Console.WriteLine("Actual pc: " + TestCPU.pc);
                    }

                    if (TestCPU.s == Tests[testNumber].FinalState.s) { Console.WriteLine("s: PASS"); }
                    else
                    {
                        Console.WriteLine("s: FAIL");
                        Console.WriteLine("Expected s: " + Tests[testNumber].FinalState.s);
                        Console.WriteLine("Actual s: " + TestCPU.s);
                    }

                    if (TestCPU.a == Tests[testNumber].FinalState.a) { Console.WriteLine("a: PASS"); }
                    else
                    {
                        Console.WriteLine("a: FAIL");
                        Console.WriteLine("Initial a: " + Tests[testNumber].InitialState.a + " - " + Tests[testNumber].InitialState.a.ToString("x4") + " - " + Convert.ToString(Tests[testNumber].InitialState.a, 2).PadLeft(8, '0'));
                        Console.WriteLine("Expected a: " + Tests[testNumber].FinalState.a + " - " + Tests[testNumber].FinalState.a.ToString("x4") + " - " + Convert.ToString(Tests[testNumber].FinalState.a, 2).PadLeft(8, '0'));
                        Console.WriteLine("Actual a: " + TestCPU.a + " - " + TestCPU.a.ToString("x4") + " - " + Convert.ToString(TestCPU.a, 2).PadLeft(8, '0'));
                        Console.WriteLine("Working Data: " + Convert.ToString(TestCPU.workingData, 2).PadLeft(8, '0'));
                    }

                    if (TestCPU.x == Tests[testNumber].FinalState.x) { Console.WriteLine("x: PASS"); }
                    else
                    {
                        Console.WriteLine("x: FAIL");
                        Console.WriteLine("Expected x: " + Tests[testNumber].FinalState.x);
                        Console.WriteLine("Actual x: " + TestCPU.x);
                    }

                    if (TestCPU.y == Tests[testNumber].FinalState.y) { Console.WriteLine("y: PASS"); }
                    else
                    {
                        Console.WriteLine("y: FAIL");
                        Console.WriteLine("Expected y: " + Tests[testNumber].FinalState.y);
                        Console.WriteLine("Actual y: " + TestCPU.y);
                    }

                    if (TestCPU.p == Tests[testNumber].FinalState.p) { Console.WriteLine("p: PASS"); }
                    else
                    {
                        Console.WriteLine("p: FAIL");
                        Console.WriteLine("\t\tNV1BDIZC");
                        Console.WriteLine("Initial p: \t" + Convert.ToString(Tests[testNumber].InitialState.p, 2).PadLeft(8, '0'));
                        Console.WriteLine("Expected p: \t" + Convert.ToString(Tests[testNumber].FinalState.p,2).PadLeft(8,'0'));
                        Console.WriteLine("Actual p: \t" + Convert.ToString(TestCPU.p,2).PadLeft(8,'0'));
                    }

                    //for (int i = 0; i < Tests[testNumber].FinalState.ram.Count; i++)
                    //{
                    //Console.WriteLine(Tests[testNumber].FinalState.ram[i][0] + " , " + Tests[testNumber].FinalState.ram[i][1]);
                    // Bus.WriteBus(Tests[0].FinalState.ram[i][0], Tests[0].FinalState.ram[i][1]);
                    //}

                    //Console.WriteLine("pc: " + Tests[testNumber].FinalState.pc);
                    //Console.WriteLine("s: " + Tests[testNumber].FinalState.s);
                    //Console.WriteLine("a: " + Tests[testNumber].FinalState.a);
                    //Console.WriteLine("x: " + Tests[testNumber].FinalState.x);
                    //Console.WriteLine("y: " + Tests[testNumber].FinalState.y);
                    // Console.WriteLine("p: " + Tests[testNumber].FinalState.p);

                    for (int i = 0; i < Tests[testNumber].FinalState.ram.Count; i++)
                    {
                        // FinalState.ram[i][0] is the memory location, FinalState.ram[i][1] is the value
                        if (Bus.ReadBus(Tests[testNumber].FinalState.ram[i][0]) == Tests[testNumber].FinalState.ram[i][1]) { Console.WriteLine("RAM: PASS"); }
                        else
                        {
                            Console.WriteLine("RAM [" + Tests[testNumber].FinalState.ram[i][0] + "]:FAIL");
                            Console.WriteLine("Expected value: " + Tests[testNumber].FinalState.ram[i][1]);
                            Console.WriteLine("Actual value: " + Bus.ReadBus(Tests[testNumber].FinalState.ram[i][0]));
                        }

                        //Console.WriteLine(Tests[testNumber].FinalState.ram[i][0] + " , " + Tests[testNumber].FinalState.ram[i][1]);
                        // Bus.WriteBus(Tests[0].FinalState.ram[i][0], Tests[0].FinalState.ram[i][1]);
                    }

                    // Check CPU Cycles
                    if (Bus.cycleCount == Tests[testNumber].Cycles.Count) { Console.WriteLine("Cycles: PASS"); }
                    else 
                    {
                        Console.WriteLine("Cycles: FAIL");
                        Console.WriteLine("Expected cycles: " + Tests[testNumber].Cycles.Count);
                        Console.WriteLine("Actual cycles: " + Bus.cycleCount);

                    }


                    //Console.WriteLine("Actual state: ");
                    //PrintRegisters("int");
                    //PrintPC();
                    //PrintMemoryContents(Tests[testNumber].InitialState.ram[0][0], Tests[testNumber].InitialState.ram.Count, "int");

                    Console.ReadLine();
                }

                // Console.WriteLine(testsFailed);
                
            }  // End Opcode test FOR

            Console.WriteLine("Tests failed: " + testsFailed);
        } // End JSONTest()

    } // End Class OpcodeTest

    

    public class OpcodeTestSuite
    {
        OpcodeTest OpcodeTest = new OpcodeTest();

        public OpcodeTestSuite() 
        {
           
        }

        public async Task Start()
        {
            // ADC
            await OpcodeTest.JSONTest("69");
            await OpcodeTest.JSONTest("65");
            await OpcodeTest.JSONTest("75");
            await OpcodeTest.JSONTest("6D");
            await OpcodeTest.JSONTest("7D");
            await OpcodeTest.JSONTest("79");
            await OpcodeTest.JSONTest("61");
            await OpcodeTest.JSONTest("71");

            //// AND
            //await OpcodeTest.JSONTest("29");
            //await OpcodeTest.JSONTest("25");
            //await OpcodeTest.JSONTest("35");
            //await OpcodeTest.JSONTest("2D");
            //await OpcodeTest.JSONTest("3D");
            //await OpcodeTest.JSONTest("39");
            //await OpcodeTest.JSONTest("21");
            //await OpcodeTest.JSONTest("31");

            //// ASL
            //await OpcodeTest.JSONTest("0A");
            //await OpcodeTest.JSONTest("06");
            //await OpcodeTest.JSONTest("16");
            //await OpcodeTest.JSONTest("0E");
            //await OpcodeTest.JSONTest("1E");

            //// BCC
            //await OpcodeTest.JSONTest("90");

            //// BCS 
            //await OpcodeTest.JSONTest("B0");

            //// BEQ 
            //await OpcodeTest.JSONTest("F0");

            //// BIT
            //await OpcodeTest.JSONTest("24");
            //await OpcodeTest.JSONTest("2C");

            //// BMI 
            //await OpcodeTest.JSONTest("30");

            //// BNE 
            //await OpcodeTest.JSONTest("D0");

            //// BPL 
            //await OpcodeTest.JSONTest("10");

            //// BRK
            //await OpcodeTest.JSONTest("00");

            //// BVC 
            //await OpcodeTest.JSONTest("50");

            //// BVS 
            //await OpcodeTest.JSONTest("70");

            //// CLC 
            //await OpcodeTest.JSONTest("18");

            //// CLD 
            //await OpcodeTest.JSONTest("D8");

            //// CLI 
            //await OpcodeTest.JSONTest("58");

            //// CLV 
            //await OpcodeTest.JSONTest("B8");


            //// CMP
            //await OpcodeTest.JSONTest("C9");
            //await OpcodeTest.JSONTest("C5");
            //await OpcodeTest.JSONTest("D5");
            //await OpcodeTest.JSONTest("CD");
            //await OpcodeTest.JSONTest("DD");
            //await OpcodeTest.JSONTest("D9");
            //await OpcodeTest.JSONTest("C1");
            //await OpcodeTest.JSONTest("D1");

            //// CPX
            //await OpcodeTest.JSONTest("E0");
            //await OpcodeTest.JSONTest("E4");
            //await OpcodeTest.JSONTest("EC");

            //// CPY
            //await OpcodeTest.JSONTest("C0");
            //await OpcodeTest.JSONTest("C4");
            //await OpcodeTest.JSONTest("CC");

            //// DEC
            //await OpcodeTest.JSONTest("C6");
            //await OpcodeTest.JSONTest("D6");
            //await OpcodeTest.JSONTest("CE");
            //await OpcodeTest.JSONTest("DE");

            //// DEX
            //await OpcodeTest.JSONTest("CA");


            //// DEY
            //await OpcodeTest.JSONTest("88");

            //// EOR
            //await OpcodeTest.JSONTest("49");
            //await OpcodeTest.JSONTest("45");
            //await OpcodeTest.JSONTest("55");
            //await OpcodeTest.JSONTest("4D");
            //await OpcodeTest.JSONTest("5D");
            //await OpcodeTest.JSONTest("59");
            //await OpcodeTest.JSONTest("41");
            //await OpcodeTest.JSONTest("51");


            //// INC
            //await OpcodeTest.JSONTest("E6");
            //await OpcodeTest.JSONTest("F6");
            //await OpcodeTest.JSONTest("EE");
            //await OpcodeTest.JSONTest("FE");

            //// INX
            //await OpcodeTest.JSONTest("E8");

            //// INY
            //await OpcodeTest.JSONTest("C8");

            //// JMP
            //await OpcodeTest.JSONTest("4C");
            //await OpcodeTest.JSONTest("6C");

            //// JSR
            //await OpcodeTest.JSONTest("20");

            //// LDA
            //await OpcodeTest.JSONTest("A9");
            //await OpcodeTest.JSONTest("A5");
            //await OpcodeTest.JSONTest("B5");
            //await OpcodeTest.JSONTest("AD");
            //await OpcodeTest.JSONTest("BD");
            //await OpcodeTest.JSONTest("B9");
            //await OpcodeTest.JSONTest("A1");
            //await OpcodeTest.JSONTest("B1");

            //// LDX
            //await OpcodeTest.JSONTest("A2");
            //await OpcodeTest.JSONTest("A6");
            //await OpcodeTest.JSONTest("B6");
            //await OpcodeTest.JSONTest("AE");
            //await OpcodeTest.JSONTest("BE");

            //// LDY
            //await OpcodeTest.JSONTest("A0");
            //await OpcodeTest.JSONTest("A4");
            //await OpcodeTest.JSONTest("B4");
            //await OpcodeTest.JSONTest("AC");
            //await OpcodeTest.JSONTest("BC");

            //// LSR
            //await OpcodeTest.JSONTest("4A");
            //await OpcodeTest.JSONTest("46");
            //await OpcodeTest.JSONTest("56");
            //await OpcodeTest.JSONTest("4E");
            //await OpcodeTest.JSONTest("5E");

            //// NOP
            //await OpcodeTest.JSONTest("EA");

            //// ORA
            //await OpcodeTest.JSONTest("09");
            //await OpcodeTest.JSONTest("05");
            //await OpcodeTest.JSONTest("15");
            //await OpcodeTest.JSONTest("0D");
            //await OpcodeTest.JSONTest("1D");
            //await OpcodeTest.JSONTest("19");
            //await OpcodeTest.JSONTest("01");
            //await OpcodeTest.JSONTest("11");

            //// PHA
            //await OpcodeTest.JSONTest("48");

            //// PHP
            //await OpcodeTest.JSONTest("08");

            //// PLA
            //await OpcodeTest.JSONTest("68");

            //// PLP
            //await OpcodeTest.JSONTest("28");

            //// ROL
            //await OpcodeTest.JSONTest("2A");
            //await OpcodeTest.JSONTest("26");
            //await OpcodeTest.JSONTest("36");
            //await OpcodeTest.JSONTest("2E");
            //await OpcodeTest.JSONTest("3E");

            ////ROR
            //await OpcodeTest.JSONTest("6A");
            //await OpcodeTest.JSONTest("66");
            //await OpcodeTest.JSONTest("76");
            //await OpcodeTest.JSONTest("6E");
            //await OpcodeTest.JSONTest("7E");

            //// RTI
            //await OpcodeTest.JSONTest("40");

            //// RTS
            //await OpcodeTest.JSONTest("60");

            ////SBC
            //await OpcodeTest.JSONTest("E9");
            //await OpcodeTest.JSONTest("E5");
            //await OpcodeTest.JSONTest("F5");
            //await OpcodeTest.JSONTest("ED");
            //await OpcodeTest.JSONTest("FD");
            //await OpcodeTest.JSONTest("F9");
            //await OpcodeTest.JSONTest("E1");
            //await OpcodeTest.JSONTest("F1");

            //// SEC 
            //await OpcodeTest.JSONTest("38");

            //// SED 
            //await OpcodeTest.JSONTest("F8");

            //// SEI 
            //await OpcodeTest.JSONTest("78");

            //// STA
            //await OpcodeTest.JSONTest("85");
            //await OpcodeTest.JSONTest("95");
            //await OpcodeTest.JSONTest("8D");
            //await OpcodeTest.JSONTest("9D");
            //await OpcodeTest.JSONTest("99");
            //await OpcodeTest.JSONTest("81");
            //await OpcodeTest.JSONTest("91");

            //// STX
            //await OpcodeTest.JSONTest("86");
            //await OpcodeTest.JSONTest("96");
            //await OpcodeTest.JSONTest("8E");

            //// STY
            //await OpcodeTest.JSONTest("84");
            //await OpcodeTest.JSONTest("94");
            //await OpcodeTest.JSONTest("8C");

            //// TAX
            //await OpcodeTest.JSONTest("AA");

            //// TAY
            //await OpcodeTest.JSONTest("A8");

            //// TSX
            //await OpcodeTest.JSONTest("BA");

            //// TXA
            //await OpcodeTest.JSONTest("8A");

            //// TXS
            //await OpcodeTest.JSONTest("BA");

            //// TYA
            //await OpcodeTest.JSONTest("98");

            return;
            
        }
    }
}
