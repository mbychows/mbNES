using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace mbNES
{
    public partial class DebugWindow : Form
    {

        public int[] memoryContents;
        public DebugWindow(mbNESmain mbNESmain)
        {
            InitializeComponent();
            
            
        }

        public int[] getMemoryContents()
        {
            memoryContents = Bus.RAM;
            return memoryContents;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            memoryContentsRichTextBox.Clear();
            int offset = 0;
            string formattedMemoryContents = "";

            getMemoryContents();

            StringBuilder sbFull = new StringBuilder();
            StringBuilder sbLine = new StringBuilder();
            

            for (int i = 0; i < memoryContents.Length; i++)
            {
                if (i % 16 == 0)                                                            // Add the offset to the sb line
                {
                    sbLine.Append("0x" + offset.ToString("X4") + " - ");
                    //formattedMemoryContents += "0x" + offset.ToString("X4") + " - ";
                    offset += 16;
                }

                sbLine.Append(memoryContents[i].ToString("X2").ToUpperInvariant() + " ");   // Add the next memory value to sb line
                // formattedMemoryContents += memoryContents[i].ToString("X2").ToUpperInvariant() + " ";
                
                if ( ((i+1) % 16 == 0) && (i != 0) )                                        // If we're done with a line
                {
                    sbLine.Append("\n");
                    sbFull.Append(sbLine.ToString());                                       // Generate a string for the line and add it to the full SB object
                    sbLine.Clear();                                                         // Clear out the line
                    //formattedMemoryContents += Environment.NewLine;
                }
                
            }



            memoryContentsRichTextBox.AppendText(sbFull.ToString());                        // Generate the full string and put it in the memory contents box
            
        }

        private void DebugWindow_Load(object sender, EventArgs e)
        {
            logRichTextBox.Clear();                         
            logRichTextBox.AppendText(Log.GetLog());                                    // Populate the log box
            registersTextBox.Clear();
            registersTextBox.AppendText(CPU.nesCPU.getRegistersFormatted());            // Populate the registers box
            
        }

        private void refreshLogButton_Click(object sender, EventArgs e)
        {
            logRichTextBox.Clear();
            logRichTextBox.AppendText(Log.GetLog());
        }

        private void refreshRegistersButton_Click(object sender, EventArgs e)
        {
            

            registersTextBox.Clear();
            registersTextBox.AppendText(CPU.nesCPU.getRegistersFormatted());
        }
    }
}
