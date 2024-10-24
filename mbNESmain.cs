using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace mbNES
{
    public partial class mbNESmain : Form
    {

        //CPUTest TestCPU = new CPUTest();
        // OpcodeTest OpcodeTest = new OpcodeTest();
        //public CPU CPU = new CPU();


        public mbNESmain()
        {
            InitializeComponent();
            

        }

        private void debugWindowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            debugWindowToolStripMenuItem.Enabled = false;                                   // Disables the Debug window menu item
            DebugWindow debugWindow = new DebugWindow(this);                                // New debug window

            debugWindow.FormClosed += new FormClosedEventHandler(DebugWindow_FormClosed);   // Event handler for when the window is closed (re-enable menu item)
            debugWindow.Show();                                                             // Show the debug window
        }

        void DebugWindow_FormClosed(object sender, FormClosedEventArgs e)
        {
            debugWindowToolStripMenuItem.Enabled = true;                                    // Re-enable menu item when debug window closes
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            OpcodeTestSuite testSuite = new OpcodeTestSuite();
            await System.Threading.Tasks.Task.Run(() => testSuite.Start());
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Open the ROM file
            OpenFileDialog openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            openFileDialog1.DefaultExt = "nes";
            openFileDialog1.Filter = "nes files (*.nes)|*.nes|All files (*.*)|*.*";
            openFileDialog1.CheckFileExists = true;
            openFileDialog1.CheckPathExists = true;
            openFileDialog1.ShowDialog();
            Console.WriteLine(openFileDialog1.FileName);
            string loadResults = Bus.LoadROM(openFileDialog1.FileName);

            //richTextBox1.AppendText("ROM File Name = " + openFileDialog1.FileName + Environment.NewLine);
            //richTextBox1.AppendText("NES<eof> = " + Bus.romBytes[0].ToString("X2") + " " + Bus.romBytes[1].ToString("X2") + " " + Bus.romBytes[2].ToString("X2") + " " + Bus.romBytes[3].ToString("X2") + Environment.NewLine);
            //richTextBox1.AppendText("PRG ROM Size = " + Bus.romBytes[4].ToString("D2") + " x 16 KB" + Environment.NewLine);
            //richTextBox1.AppendText("CHR ROM Size = " + Bus.romBytes[5].ToString("D2") + " x 8 KB" + Environment.NewLine);
            //richTextBox1.AppendText("Flags 6 = " + Convert.ToString(Bus.romBytes[6], 2).PadLeft(8, '0') + Environment.NewLine);
            //richTextBox1.AppendText("Flags 7 = " + Convert.ToString(Bus.romBytes[7], 2).PadLeft(8, '0') + Environment.NewLine);
            //richTextBox1.AppendText("Flags 8 = " + Convert.ToString(Bus.romBytes[8], 2).PadLeft(8, '0') + Environment.NewLine);
            //richTextBox1.AppendText("Flags 9 = " + Convert.ToString(Bus.romBytes[9], 2).PadLeft(8, '0') + Environment.NewLine);
            //richTextBox1.AppendText("Flags 10 = " + Convert.ToString(Bus.romBytes[10], 2).PadLeft(8, '0') + Environment.NewLine);

            richTextBox1.AppendText(loadResults);

            CPU.nesCPU.ExecuteInstruction();
            

         }  

        
    }
}
