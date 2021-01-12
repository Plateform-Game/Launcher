using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.Diagnostics;

namespace Plateform_Launcher
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            this.button1.Click += Button1_Click;
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            this.button1.Enabled = false;
            this.label1.Text = "Status : Check for JRE installation...";
            this.progressBar1.Style = ProgressBarStyle.Marquee;

            if (Environment.GetEnvironmentVariable("JAVA") != null)
            {

            } 
            else
            {

            }
    }
}
