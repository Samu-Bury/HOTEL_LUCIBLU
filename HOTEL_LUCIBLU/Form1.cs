using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace HOTEL_LUCIBLU
{
    public partial class Form1 : Form
    {
        #region Variabili Globali

        private bool accesso = false;

        #endregion

        public Form1()
        {
            InitializeComponent();
            this.AutoScaleMode = AutoScaleMode.None;
            tabControl1.SelectedIndex = 2; // Home
        }

        // HOME
        #region HOME

        #region Bottoni Home Aggiornamento (in base all'accesso)

        private void AggiornaVisibilitaBottoniHome()
        {
            if (!accesso)
            {
                button_accedi_home.Visible = true;
                button_account_home.Visible = false;
                button_prenota_home.Visible = false;
                button_prenotazioni_home.Visible = false;
            }
            else
            {
                button_accedi_home.Visible = false;
                button_account_home.Visible = true;
                button_prenota_home.Visible = true;
                button_prenotazioni_home.Visible = true;
            }
        }



        #endregion

        #region AZIONI BOTTONI
        //Accedi (utente non registrato)
        private void button_accedi_home_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex = 0; // Login
        }

        //Prenota Ora
        private void button_prenota_home_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex = 3; // Prenota-Date
        }

        //Le mie prenotazioni
        private void button_prenotazioni_home_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex = 5;
        }

        //Account
        private void button_account_home_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex = 4;
        }
        #endregion

        #endregion

        // LOGIN
        #region LOGIN

        #region AZIONI BOTTONI

        //Accedi
        private void button_login_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex = 2; // Home
        }

        //Registrati (non registrato)
        private void label_registrati_login_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex = 1; // Register
        }
        #endregion

        #endregion

        // REGISTER
        #region REGISTER

        #region AZIONI BOTTONI

        //Registrati
        private void button_register_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex = 2; // Home
        }

        //Accedi (gia registrato)
        private void label_accedi_register_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex = 0; // Login
        }



        #endregion

        #endregion

        // PRENOTA-DATA
        #region PRENOTA DATA

        #region AZIONE BOTTONI
        //Prosegui (Prenota-Camere)
        private void button_prosegui_prenotaDate_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex = 6;
        }

        //Home
        private void button2_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex = 2;
        }

        #endregion

        #endregion

        // PRENOTA-CAMERA
        #region PRENOTA CAMERA

        #region AZIONE BOTTONI
        //Conferma e vai al pagamento
        private void button_prosegui_prenotaCamere_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex = 7;
        }

        //Date
        private void button4_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex = 3;
        }

        //Home
        private void button3_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex = 2;
        }


        #endregion

        #endregion

        // ACCOUNT
        #region ACCOUNT

        #region AZIONE BOTTONI

        //Home
        private void button1_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex = 2;
        }




        #endregion

        #endregion

        // PRENOTA-PAGAMENTO
        #region PRENOTA PAGAMENTO

        #region AZIONE BOTTONI
        //Home
        private void button35_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex = 2;
        }

        //Camere
        private void button41_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex = 7;
        }
        #endregion

        #endregion


    }
}