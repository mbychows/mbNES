using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public static int[] RAM = new int[0xFFFF];

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

        // TODO: check the mirroring
        public static int ReadBus(int address)
        {
            // If the address provided is in the range $0000-$1FFF, the two most siginificant bits
            // should be discarded so we're only ever actually writing to $0000-$07FF
            int strippedAddress = 0x0000;

            if ( (0x0000 <= address) && (address <= 0x1FFF) )
            {
                strippedAddress = (int)(address & 0b111111111);  // 9 bits
                
            }
            // Return the data stored at memory location [address]
            return RAM[strippedAddress];
        }

        public static void WriteBus(int address, int data)
        {
            //  as WriteBus
            int strippedAddress = 0x0000;

            if ((0x0000 <= address) && (address <= 0x1FFF))
            {
                strippedAddress = (int)(address & 0b111111111);  // 9 bits

            }
            
            // Write the byte of data [data] to memory at location [address]
            RAM[strippedAddress] = data;
        }

    }
}
