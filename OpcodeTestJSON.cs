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

        public void JSONTest(string opcode)
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

}
