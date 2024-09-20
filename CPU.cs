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
    public partial class CPU
    //internal class CPU
    {



        // ---------------
        //    Registers   |  
        // ---------------

        public int a { get; private set; } = 0x00;         // Accumulator
        public int x { get; private set; } = 0x00;         // x register
        public int y { get; private set; } = 0x00;         // y register 
        public int s { get; private set; } = 0x00;         // Stack Pointer
        public int p { get; private set; } = 0x00;         // Status register

        public int pc { get; private set; } = 0x0000;    // Program counter, 2 bytes

        // public int cycleCount { get; private set; } = 0;   // Cycle counter

        private int currentOpcode = 0x00;

        public int workingData = 0x00;            // Holds the data read from address during each instruction
        public int effectiveAddress = 0x0000;    // Final address to be used by the instruction after addressing mode
                                                    //   operations have been applied

        private int tempLowOrderByte = 0x00;
        private int tempHighOrderByte = 0x00;
        private int tempAddress = 0x0000;
        private int tempResult = 0x0000;
        private int baseAddress;

        private int register = 0;
        
        public void SetRegisters(int a, int x, int y, int s, int p)
        {
            this.a = a;
            this.x = x;
            this.y = y;
            this.s = s;
            this.p = p;
        }

        public void SetPRegisterBit(int BitNumber, int value)  // No error checking on this:  TODO
        {
            if (value == 1)  {  p |= (1 << BitNumber); }  // Setting bit
            else { p &= ~(1 << BitNumber); }         // Clearing bit
        }

        public void ResetCPU()
        {
            SetRegisters(0x00, 0x00, 0x00, 0x00, 0x00);
            ResetPC();
        }

        public void SetPC(int pc)  { this.pc = pc; }

        private void ResetPC()  {  pc = 0x0000;  }

        private void IncrementPC()
        {
            if (pc == 65535) { ResetPC(); }     // Program counter wraparound
            else { pc++; }
        }

        private void ReadOpcode()
        { 
            Bus.cycleCount = 0;     // Reset cycle count for new instruction
            // get opcode stored at value in program counter
            currentOpcode = Bus.ReadBus(pc,true);
            IncrementPC();
        }    

    }


}
