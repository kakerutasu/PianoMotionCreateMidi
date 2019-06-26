using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace PianoMotionCreateMidi
{
    public partial class ProgressForm : Form
    {
        // getter/setter
        public string labelText
        {
            get { return this.label1.Text; }
            set { this.label1.Text = labelText; }
        }

        // initialize
        public ProgressForm()
        {
            InitializeComponent();
            this.progressBar1.Visible = true;
            this.progressBar1.Minimum = 0;
            this.progressBar1.Maximum = 100;
        }
        public void UpdateProgressBar(int iPos, string strMsg)
        {
            this.label1.Text = strMsg;
            this.progressBar1.Value = iPos;
            this.label1.Update();
            this.progressBar1.Update();
        }
    }
}
