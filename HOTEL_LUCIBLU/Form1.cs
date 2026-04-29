using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace HOTEL_LUCIBLU
{
    public partial class Form1 : Form
    {
        #region Variabili Globali

        private bool accesso = false;

        #region Calendario
        //Calendario
        int anno = 2026;
        int meseCorrente;

        #endregion

        //Account
        bool posizioneHome = true; //Sono andato nella schermata account dalla Home o da ADMIN (true = home)

        #endregion

        public Form1()
        {
            InitializeComponent();
            
            this.AutoScaleMode = AutoScaleMode.None; //Non deforma la form su altri dispositivi
            tabControl1.SelectedIndex = 2; // Home come pagina iniziale
            AggiornaVisibilitaBottoniHome(); //Visualizzazione bottoni in base all'accesso se presente o meno

            #region Setting TabControl
            //Nasconde il menu dei TabControl
            //TabControl1 (APP)
            tabControl1.Appearance = TabAppearance.FlatButtons;
            tabControl1.ItemSize = new Size(0, 1);
            tabControl1.SizeMode = TabSizeMode.Fixed;
            //TabControl2 (ADMIN)
            tabControl2.Appearance = TabAppearance.FlatButtons;
            tabControl2.ItemSize = new Size(0, 1);
            tabControl2.SizeMode = TabSizeMode.Fixed;

            #endregion

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
                meseCorrente = 1;

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
            string email = textBox_email_login.Text.Trim();
            string password = textBox_password_login.Text;

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Inserisci email e password.");
                return;
            }

            DatabaseHelper db = new DatabaseHelper();

            if (db.Login(email, password, out string nome, out string tipo))
            {
                accesso = true;
                AggiornaVisibilitaBottoniHome();

                if (tipo == "admin")
                {
                    tabControl1.SelectedIndex = 8; // Pannello Admin
                }
                else
                {
                    tabControl1.SelectedIndex = 2; // Home utente normale
                }

                MessageBox.Show($"Benvenuto, {nome}!");
            }
            else
            {
                MessageBox.Show("Email o password errata.");
            }





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
            DatabaseHelper db = new DatabaseHelper();

            bool successo = db.Registra(
                textBox_email_register.Text.Trim(),
                textBox_password_register.Text,
                textBox_nome_register.Text.Trim(),
                textBox_cognome_register.Text.Trim(),
                dateTimePicker_data_register.Value
            );

            if (successo)
            {
                MessageBox.Show("Registrazione completata!");
                tabControl1.SelectedIndex = 2; // Home
            }
            else
            {
                MessageBox.Show("Email già in uso.");
            }

                
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
            CaricaUtenti();
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


        #region Carica Utenti Sezione Admin
        private void CaricaUtenti()
        {
            listView_user.Items.Clear();
            listView_user.View = View.Details;
            listView_user.FullRowSelect = true;
            listView_user.GridLines = true;

            // Colonne (solo la prima volta)
            if (listView_prenotazione.Columns.Count == 0)
            {
                listView_user.Columns.Add("Email", 200);
                listView_user.Columns.Add("Nome", 100);
                listView_user.Columns.Add("Cognome", 100);
                listView_user.Columns.Add("Data Nascita", 100);
                listView_user.Columns.Add("Registrato il", 130);
                listView_user.Columns.Add("Tipo", 70);
            }

            DatabaseHelper db = new DatabaseHelper();
            List<Utente> utenti = db.GetUtenti();

            foreach (Utente u in utenti)
            {
                ListViewItem item = new ListViewItem(u.Email);
                item.SubItems.Add(u.Nome);
                item.SubItems.Add(u.Cognome);
                item.SubItems.Add(u.DataNascita.HasValue ? u.DataNascita.Value.ToString("dd/MM/yyyy") : "—");
                item.SubItems.Add(u.DataCreazione.ToString("dd/MM/yyyy HH:mm"));
                item.SubItems.Add(u.Tipo);

                // Colora gli admin in blu
                if (u.Tipo == "admin")
                    item.ForeColor = Color.RoyalBlue;

                listView_user.Items.Add(item);
            }
        }


        #endregion

        //Elimina utente (admin)
        private void button54_Click(object sender, EventArgs e)
        {
            if (listView_user.SelectedItems.Count == 0)
            {
                MessageBox.Show("Seleziona un utente da eliminare.", "Attenzione",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string emailSelezionata = listView_user.SelectedItems[0].Text;

            DialogResult conferma = MessageBox.Show(
                $"Sei sicuro di voler eliminare l'utente:\n{emailSelezionata}?",
                "Conferma eliminazione",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (conferma == DialogResult.Yes)
            {
                DatabaseHelper db = new DatabaseHelper();
                if (db.EliminaUtente(emailSelezionata))
                {
                    MessageBox.Show("Utente eliminato con successo.");
                    CaricaUtenti(); // Aggiorna la lista
                }
                else
                {
                    MessageBox.Show("Errore durante l'eliminazione.");
                }
            }
        }

        //Aggiungi nuovo utente
        private void button55_Click(object sender, EventArgs e)
        {
            FormAggiungiUtente finestra = new FormAggiungiUtente();

            if (finestra.ShowDialog() == DialogResult.OK)
            {
                DatabaseHelper db = new DatabaseHelper();

                // Controlla email duplicata
                if (db.EmailEsiste(finestra.EmailInserita))
                {
                    MessageBox.Show("Email già in uso.", "Errore",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                bool successo = db.Registra(
                    finestra.EmailInserita,
                    finestra.PasswordInserita,
                    finestra.NomeInserito,
                    finestra.CognomeInserito,
                    finestra.DataNascitaInserita,
                    finestra.TipoInserito
                );

                if (successo)
                {
                    MessageBox.Show("Utente aggiunto con successo!");
                    CaricaUtenti(); // Aggiorna la lista
                }
                else
                {
                    MessageBox.Show("Errore durante l'aggiunta.");
                }
            }
        }
    }
}