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
        bool accesso = false;
        public Form1()
        {
            InitializeComponent();
            tabControl1.SelectedIndex = 2; //apro home iniziale
            //Controllo se è stato effettuato l'accesso
            ControlloAccesso();
            this.AutoScaleMode = AutoScaleMode.None;
        }

        #region Controllo Accesso
        public void ControlloAccesso()
        {
            if (!accesso)
            {
                button_login.Visible = true;
                button_account.Visible = false;
                button_prenota.Visible = false;
                button_prenotazioni.Visible = false;
            }
            else
            {
                button_login.Visible = false;
                button_account.Visible = true;
                button_prenota.Visible = true;
                button_prenotazioni.Visible = true;
            }
        }
        #endregion

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

        //Buttom Accedi (porta alla pagina home)
        private void button1_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex = 2;
            accesso = true;
            ControlloAccesso();
        }

        private void button_accedi_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex = 0;
            
        }

        private void button_prenota_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex = 3;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex = 6;
        }

        
    }
}
