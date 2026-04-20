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

        //Calendario
        int anno = 2026;
        int meseCorrente;

        //Account
        bool posizioneHome = true; //Sono andato nella schermata account dalla Home o da ADMIN (true = home)

        #endregion

        public Form1()
        {
            InitializeComponent();
            this.AutoScaleMode = AutoScaleMode.None;
            tabControl1.SelectedIndex = 2; // Home
            AggiornaVisibilitaBottoniHome(); //Visualizzazione bottoni in base all'accesso se presente o meno

            //Nasconde il menu dei TabControl
            //TabControl1 (SOFTWARE)
            tabControl1.Appearance = TabAppearance.FlatButtons;
            tabControl1.ItemSize = new Size(0, 1);
            tabControl1.SizeMode = TabSizeMode.Fixed;
            //TabControl2 (ADMIN)
            tabControl2.Appearance = TabAppearance.FlatButtons;
            tabControl2.ItemSize = new Size(0, 1);
            tabControl2.SizeMode = TabSizeMode.Fixed;

            #region SETTING BOTTONI HOME (RIMOZIONE SFONDO E ALTRO.)
            Button[] bottoni = {
            button_accedi_home,
            button_account_home,
            button_prenotazioni_home,
            button_prenota_home
            };

            foreach (Button btn in bottoni)
            {
                btn.FlatStyle = FlatStyle.Flat;
                btn.FlatAppearance.BorderSize = 0;
                btn.FlatAppearance.MouseDownBackColor = Color.Transparent;
                btn.FlatAppearance.MouseOverBackColor = Color.Transparent;
                btn.BackColor = Color.Transparent;
                btn.TabStop = false;
            }

            #endregion


            #region CARICAMENTO DATE CALENDARIO (Chiama Funzione)

            if (DateTime.Now.Year == 2026)
                meseCorrente = DateTime.Now.Month;
            else
                meseCorrente = 1; // Gennaio

            AggiornaCalendario(); //Carico i giorni

            #endregion
        }

        #region GESTIONE SCHERMATE

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
            posizioneHome = true;
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
            accesso = true;
            AggiornaVisibilitaBottoniHome();
        }

        //Registrati (non registrato)
        private void label_registrati_login_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex = 1; // Register
        }

        //Test Admin
        private void button6_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex = 8;
            posizioneHome = false;
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
            if (posizioneHome == true) // Sono partito dall HOME
            {
                tabControl1.SelectedIndex = 2; //Home
            }
            else // Sono partito da Admin
            {
                tabControl1.SelectedIndex = 8; //Admin
            }
                
        }
        //Logout
        private void button7_Click(object sender, EventArgs e)
        {
            accesso = false;
            tabControl1.SelectedIndex = 2;
            AggiornaVisibilitaBottoniHome();
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
            tabControl1.SelectedIndex = 6;
        }

        #endregion

        #endregion

        // GESTIONE ADMIN
        #region GESTIONE ADMIN

        #region AZIONE BOTTTONI
        //Dashboard
        private void button45_Click(object sender, EventArgs e)
        {
            tabControl2.SelectedIndex = 0;
        }

        //Prenotazioni
        private void button44_Click(object sender, EventArgs e)
        {
            tabControl2.SelectedIndex = 1;
        }

        //Gestione camere
        private void button46_Click(object sender, EventArgs e)
        {
            tabControl2.SelectedIndex = 2;
        }

        //Clienti/admin
        private void button47_Click(object sender, EventArgs e)
        {
            tabControl2.SelectedIndex = 3;
        }

        //Account
        private void button8_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex = 4;
        }
        #endregion

        #endregion

        // LE MIE PRENOTAZIONI
        #region LE MIE PRENOTAZIONI

        #region AZIONI BOTTONI
        //Home
        private void button42_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex = 2;
        }

        #endregion

        #endregion

        #endregion


        #region CARICAMENTO DATE CALENDARIO
        //Trova il tabcontrol1 e il panel per individuare i bottoni del calendario
        Control TrovaControllo(Control parent, string nome)
        {
            foreach (Control c in parent.Controls)
            {
                if (c.Name == nome)
                    return c;

                Control trovato = TrovaControllo(c, nome);
                if (trovato != null)
                    return trovato;
            }
            return null;
        }

        void AggiornaCalendario()
        {
            TabPage tab = tabPage3;
            Panel panel = TrovaControllo(tab, "panel_calendario") as Panel; //Trova il panel

            if (panel == null) return;

            DateTime primoGiorno = new DateTime(anno, meseCorrente, 1);
            int giorniNelMese = DateTime.DaysInMonth(anno, meseCorrente);

            label_nomeMese.Text = primoGiorno.ToString("MMMM yyyy");

            int start = ((int)primoGiorno.DayOfWeek + 6) % 7;

            int giorno = 1;

            for (int i = 1; i <= 42; i++)
            {
                Button btn = TrovaControllo(panel, "data" + i) as Button;

                if (btn == null) continue;

                int index = i - 1;

                if (index >= start && giorno <= giorniNelMese)
                {
                    btn.Visible = true;
                    btn.Text = giorno.ToString();

                    DateTime dataBtn = new DateTime(anno, meseCorrente, giorno);

                    if (dataBtn < DateTime.Today)
                    {
                        btn.Enabled = false;
                        btn.BackColor = Color.LightGray;
                        btn.ForeColor = Color.DarkGray;
                    }
                    else
                    {
                        btn.Enabled = true;
                        btn.BackColor = Color.White;
                        btn.ForeColor = Color.Black;
                    }

                    if (dataBtn == DateTime.Today)
                    {
                        btn.BackColor = Color.LightGreen;
                    }

                    giorno++;
                }
                else
                {
                    // Celle inutilizzate nascoste
                    btn.Visible = false;
                    btn.Text = "";
                    btn.Enabled = false;
                }
            }
        }

        private void button_avanti_Click(object sender, EventArgs e)
        {
            if (meseCorrente < 12) meseCorrente++;
            AggiornaCalendario();
        }

        private void button_indietro_Click(object sender, EventArgs e)
        {
            if (meseCorrente > 1) meseCorrente--;
            AggiornaCalendario();
        }









        #endregion

        
    }
}