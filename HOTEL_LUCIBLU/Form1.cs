using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HOTEL_LUCIBLU
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        //Registrati (Open Form)
        private void label6_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex = 1;
        }

        //Accedi (Open Form)
        private void label7_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex = 0;
        }
    }
}
