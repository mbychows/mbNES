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
        public DebugWindow()
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
            int offset = 0;
            getMemoryContents();
            for (int i = 0; i < memoryContents.Length; i++)
            {
                if (i % 16 == 0)  // print the offset
                {
                    richTextBox1.AppendText("0x" + offset.ToString("X4") + " - ");
                    offset += 16;
                }
                    
                richTextBox1.AppendText(memoryContents[i].ToString("X2").ToUpperInvariant() + " ");
                
                if ( ((i+1) % 16 == 0) && (i != 0) )
                {
                    richTextBox1.AppendText(Environment.NewLine);
                }
                
            }
            
        }
    }
}
