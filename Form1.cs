﻿using System;
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
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            CPUTest TestCPU = new CPUTest();
            OpcodeTest OpcodeTest = new OpcodeTest();
            // BRK -- TEST NEXT
            OpcodeTest.JSONTest("00");

        }
    }
}
