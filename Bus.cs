using System;
using System.Collections.Generic;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace mbNES
{
    internal static class Bus
    {
        /*  Address range   Size    Device
            $0000–$07FF 	$0800 	2 KB internal RAM
            $0800–$0FFF 	$0800 	|
            $1000–$17FF 	$0800   | - Mirrors of $0000–$07FF
            $1800–$1FFF 	$0800   |
            $2000–$2007 	$0008 	NES PPU registers
            $2008–$3FFF 	$1FF8   Mirrors of $2000–$2007 (repeats every 8 bytes)
            $4000–$4017 	$0018 	NES APU and I/O registers
            $4018–$401F 	$0008 	APU and I/O functionality that is normally disabled.See CPU Test Mode.
            $4020–$FFFF     $BFE0   Unmapped.  Available for cartridge use.
            $6000–$7FFF     $2000   Usually cartridge RAM, when present.
            $8000–$FFFF     $8000 	Usually cartridge ROM and mapper registers.

           From https://www.nesdev.org/wiki/CPU_memory_map
        */



        // 2 KiB of internal RAM available to CPU, mapped to $0000-$2000
        // Only 9 of the 11 pins/bits are needed to address the 2 KiB,
        // and the remaining two most significant bits are ignored,
        // resulting in mirroring of memory $0000-$07FF to three more ranges in table above

        // Array of signed 32-bit integers equivalent to 64K of RAM - need ints for bitwise operations
        // 0xFFFF = 65535
        public static int[] RAM = new int[0xFFFF+10]; // Must be +1 to make $FFFF inside the bounds of the array
                                                      // set larger for now for testing without throwing exceptions
        
        public static byte[] romBytes;                  // For opening a ROM

        public static int cycleCount;

        public static int resetVector;

        static Bus()
        {
            // Set all RAM values to 0x00
            for (int i = 0; i < RAM.Length; i++) { RAM[i] = 0; }
        }

        public static void ResetBus()
        {
            // Set all RAM values to 0x00
            for (int i = 0; i < RAM.Length; i++) { RAM[i] = 0; }
        }


        // Check memory contents and increment cycleCount
        // TODO: check the mirroring
        public static int ReadBus(int address, bool increment)
        {
            // If the address provided is in the range $0000-$1FFF, the two most siginificant bits
            // should be discarded so we're only ever actually writing to $0000-$07FF
            int strippedAddress = 0x0000;
            cycleCount++;
            //if ( (0x0000 <= address) && (address <= 0x1FFF) )
            //{
            //    strippedAddress = (int)(address & 0b111111111);  // 9 bits

            //}
            // Return the data stored at memory location [address]
            try  { return RAM[address]; }
            catch (Exception e) { Console.WriteLine(e.Message); return 0; }
            
        }

        // Used for testing
        public static int ReadBus(int address)
        {
            // Increment variable isn't actually used

            // Return the data stored at memory location [address]
            try { return RAM[address]; }
            catch (Exception e) { Console.WriteLine(e.Message); return 0; }

        }

        public static void WriteBus(int address, int data)
        {
            //  as WriteBus
            int strippedAddress = 0x0000;

           // if ((0x0000 <= address) && (address <= 0x1FFF))
            //{
            //    strippedAddress = (int)(address & 0b111111111);  // 9 bits

            //}
            
            // Write the byte of data [data] to memory at location [address]
            RAM[address] = data;
            //cycleCount++;
        }

        // Write memory contents and increment cycleCount
        public static void WriteBus(int address, int data, bool increment)
        {
            RAM[address] = data;
            cycleCount++;

        }

        public static string LoadROM(string romFilePath)
        {
            string loadResults = "";
            romBytes = System.IO.File.ReadAllBytes(romFilePath);                // Read in the file
            Console.WriteLine(romBytes[0].ToString("X2") + " " + romBytes[1].ToString("X2") + " " + romBytes[2].ToString("X2") + " " + romBytes[3].ToString("X2"));
            Log.AddToLog("ROM File Name = " + romFilePath + Environment.NewLine);
            Log.AddToLog("NES<eof> = " + Bus.romBytes[0].ToString("X2") + " " + Bus.romBytes[1].ToString("X2") + " " + Bus.romBytes[2].ToString("X2") + " " + Bus.romBytes[3].ToString("X2") + Environment.NewLine);
            Log.AddToLog("PRG ROM Size = " + Bus.romBytes[4].ToString("D2") + " x 16 KB" + Environment.NewLine);
            Log.AddToLog("CHR ROM Size = " + Bus.romBytes[5].ToString("D2") + " x 8 KB" + Environment.NewLine);
            Log.AddToLog("Flags 6 = " + Convert.ToString(Bus.romBytes[6], 2).PadLeft(8, '0') + Environment.NewLine);
            Log.AddToLog("Flags 7 = " + Convert.ToString(Bus.romBytes[7], 2).PadLeft(8, '0') + Environment.NewLine);
            Log.AddToLog("Flags 8 = " + Convert.ToString(Bus.romBytes[8], 2).PadLeft(8, '0') + Environment.NewLine);
            Log.AddToLog("Flags 9 = " + Convert.ToString(Bus.romBytes[9], 2).PadLeft(8, '0') + Environment.NewLine);
            Log.AddToLog("Flags 10 = " + Convert.ToString(Bus.romBytes[10], 2).PadLeft(8, '0') + Environment.NewLine);

            // MAPPER 0
            // Load PRG ROM into $8000 - $FFFF
            int prgRomSize = Bus.romBytes[4] * 16384;       // Calculate PRG ROM size
            int romBytesOffset = 16;                        // PRG ROM starts in byte 16 of romBytes byte array
            int nesRAMOffset = 0x8000;                      // PRG ROM from .nes file should be copied to NES RAM starting at $8000

            Log.AddToLog("PRG ROM Size is " + prgRomSize + " bytes." + Environment.NewLine);
            for (int i = 0; i < prgRomSize; i++)            // Copy PRG ROM
            {
                Bus.RAM[i + nesRAMOffset] = romBytes[i + romBytesOffset];       
            }
            Log.AddToLog("PRG ROM copied to $8000 - $FFFF" + Environment.NewLine);

            resetVector = (Bus.RAM[0xFFFD] << 8) + Bus.RAM[0xFFFC];           // Reset vector is at $FFFC, $FFFD
            Log.AddToLog("Reset vector = $" + resetVector.ToString("x4") + Environment.NewLine);

            CPU.nesCPU.SetPC(resetVector);
            

            //byte[] nesText; = 
            return loadResults;
        }
    }
}
