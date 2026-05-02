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

        //Utenti
        private bool accesso = false;
        private string utenteEmail = "";
        private string utenteNome = "";
        private string utenteTipo = "";

        //Camere
        private int pianoCameraSelezionato = 1;
        private Camera cameraSelezionata = null;
        private Button bottoneSelezionato = null;

        #region Calendario
        //Calendario
        int anno = 2026;
        int meseCorrente;

        #endregion

        //Account
        bool posizioneHome = true; //Sono andato nella schermata account dalla Home o da ADMIN ? (true = home)

        #endregion

        public Form1()
        {
            InitializeComponent();

            #region Ripristina accesso salvato
            // Ripristina accesso salvato
            if (Properties.Settings.Default.AccessoSalvato)
            {
                accesso = true;
                // Usiamo i dati salvati
                utenteEmail = Properties.Settings.Default.UtenteEmail; // ← mancava!
                utenteNome = Properties.Settings.Default.UtenteNome;
                utenteTipo = Properties.Settings.Default.UtenteTipo;

                AggiornaVisibilitaBottoniHome();
                AggiornaDatiAccount();

                if (utenteTipo == "admin")
                    tabControl1.SelectedIndex = 8;
                else
                    tabControl1.SelectedIndex = 2;
            }
            else
            {
                tabControl1.SelectedIndex = 2; // ← solo se NON c'è accesso salvato
                AggiornaVisibilitaBottoniHome();
            }
            #endregion


            this.AutoScaleMode = AutoScaleMode.None; //Non deforma la form su altri dispositivi
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
            AggiornaDatiAccount();
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

                // Salva l'accesso
                Properties.Settings.Default.AccessoSalvato = true;
                Properties.Settings.Default.UtenteEmail = email;
                Properties.Settings.Default.UtenteNome = nome;
                Properties.Settings.Default.UtenteTipo = tipo;
                Properties.Settings.Default.Save(); // ← scrive su disco

                utenteEmail = email;
                utenteNome = nome;
                utenteTipo = tipo;

                accesso = true;
                AggiornaVisibilitaBottoniHome();
                AggiornaDatiAccount(); //aggiorna i dati nella pagina account

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

        #region CARICMAMENTO (visualizzazione) CAMERE NEI BOTTONI

        private void AggiornaBottoniCamere()
        {
            DatabaseHelper db = new DatabaseHelper();
            List<Camera> camere = db.GetCamere()
                .Where(c => c.Piano == pianoCameraSelezionato)
                .ToList();

            // Lista dei 15 bottoni in ordine
            Button[] bottonìCamere = {
                camera1,  camera2,  camera3,  camera4,  camera5,
                camera6,  camera7,  camera8,  camera9,  camera10,
                camera11, camera12, camera13, camera14, camera15
             };

            // Reset tutti i bottoni
            foreach (Button btn in bottonìCamere)
            {
                btn.Text = "—";
                btn.BackColor = Color.LightGray;
                btn.ForeColor = Color.DarkGray;
                btn.Enabled = false;
                btn.Tag = null;
            }

            // Popola i bottoni con le camere del piano
            for (int i = 0; i < camere.Count && i < bottonìCamere.Length; i++)
            {
                Camera c = camere[i];
                Button btn = bottonìCamere[i];

                btn.Text = $"{c.Numero}\n{c.Tipo}";
                btn.Tag = c;
                btn.Enabled = true;

                switch (c.Stato)
                {
                    case "disponibile":
                        btn.BackColor = Color.MediumSeaGreen;
                        btn.ForeColor = Color.White;
                        break;
                    case "occupata":
                        btn.BackColor = Color.IndianRed;
                        btn.ForeColor = Color.White;
                        break;
                    case "manutenzione":
                        btn.BackColor = Color.Goldenrod;
                        btn.ForeColor = Color.White;
                        break;
                }
            }

            // Evidenzia la camera precedentemente selezionata se è ancora visibile
            if (cameraSelezionata != null)
            {
                foreach (Button btn in bottonìCamere)
                {
                    if (btn.Tag is Camera c && c.Numero == cameraSelezionata.Numero)
                    {
                        btn.BackColor = Color.DodgerBlue;
                        btn.ForeColor = Color.White;
                        bottoneSelezionato = btn;
                        break;
                    }
                }
            }

            // Aggiorna label piano attivo sui bottoni piano
            button_pianoTerra.BackColor = pianoCameraSelezionato == 1 ? Color.DodgerBlue : SystemColors.Control;
            button_piano1.BackColor = pianoCameraSelezionato == 2 ? Color.DodgerBlue : SystemColors.Control;
            button_piano2.BackColor = pianoCameraSelezionato == 3 ? Color.DodgerBlue : SystemColors.Control;

            button_pianoTerra.ForeColor = pianoCameraSelezionato == 1 ? Color.White : Color.Black;
            button_piano1.ForeColor = pianoCameraSelezionato == 2 ? Color.White : Color.Black;
            button_piano2.ForeColor = pianoCameraSelezionato == 3 ? Color.White : Color.Black;
        }

        private void SelezionaCamera(Button btnPremuto)
        {
            if (btnPremuto.Tag == null) return;

            Camera c = btnPremuto.Tag as Camera;

            // Ripristina colore bottone precedente
            if (bottoneSelezionato != null && bottoneSelezionato != btnPremuto)
            {
                Camera vecchia = bottoneSelezionato.Tag as Camera;
                if (vecchia != null)
                {
                    switch (vecchia.Stato)
                    {
                        case "disponibile": bottoneSelezionato.BackColor = Color.MediumSeaGreen; break;
                        case "occupata": bottoneSelezionato.BackColor = Color.IndianRed; break;
                        case "manutenzione": bottoneSelezionato.BackColor = Color.Goldenrod; break;
                    }
                }
            }

            // Evidenzia il bottone selezionato
            btnPremuto.BackColor = Color.DodgerBlue;
            btnPremuto.ForeColor = Color.White;
            bottoneSelezionato = btnPremuto;
            cameraSelezionata = c;

            // Aggiorna le label con i dati della camera
            label_camera_tipo.Text = c.Tipo;
            label_camera_prezzo.Text = $"€ {c.PrezzoNotte:0.00} / notte";
            label_camera_piano.Text = $"Piano {c.Piano}";
            label_camera_servizio1.Text = string.IsNullOrEmpty(c.Servizio1) ? "—" : c.Servizio1;
            label_camera_servizio2.Text = string.IsNullOrEmpty(c.Servizio2) ? "—" : c.Servizio2;
        }

        

        #endregion

        #endregion

        #region ACCOUNT

        #region AZIONE BOTTONI

        //Home
        private void button1_Click(object sender, EventArgs e)
        {
            if (posizioneHome != true) // Sono partito dall HOME
            {
                tabControl1.SelectedIndex = 8; //Admin
            }
            else // Sono partito da Admin
            {
                tabControl1.SelectedIndex = 2; //Home
            }

        }
        //Logout
        private void button7_Click(object sender, EventArgs e)
        {
            // Cancella l'accesso salvato
            Properties.Settings.Default.AccessoSalvato = false;
            Properties.Settings.Default.UtenteEmail = "";
            Properties.Settings.Default.UtenteNome = "";
            Properties.Settings.Default.UtenteTipo = "";
            Properties.Settings.Default.Save();

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
            CaricaCamere();
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
            posizioneHome = false;
            AggiornaDatiAccount();
        }
        #endregion

        #region UTENTI

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

        #region Gestione Utenti (inserimento/rimozione/modifica)
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

        //Modifica utente (admin)
        private void button5_Click(object sender, EventArgs e)
        {
            if (listView_user.SelectedItems.Count == 0)
            {
                MessageBox.Show("Seleziona un utente da modificare.", "Attenzione",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string emailSelezionata = listView_user.SelectedItems[0].Text;
            DatabaseHelper db = new DatabaseHelper();
            Utente u = db.GetUtente(emailSelezionata);

            if (u == null) return;

            // Apre la stessa form ma in modalità modifica
            FormAggiungiUtente finestra = new FormAggiungiUtente(u);

            if (finestra.ShowDialog() == DialogResult.OK)
            {
                bool successo = db.ModificaUtente(
                    emailSelezionata,
                    finestra.EmailInserita,
                    finestra.NomeInserito,
                    finestra.CognomeInserito,
                    finestra.DataNascitaInserita,
                    finestra.PasswordInserita,  // vuota = non cambia password
                    finestra.TipoInserito
                );

                if (successo)
                {
                    MessageBox.Show("Utente modificato con successo!");
                    CaricaUtenti();
                }
                else
                {
                    MessageBox.Show("Errore durante la modifica.");
                }
            }
        }
        #endregion

        #endregion

        #region CAMERE

        private void CaricaCamere()
        {
            listView_camere.View = View.Details;
            listView_camere.FullRowSelect = true;
            listView_camere.GridLines = true;

            if (listView_camere.Columns.Count == 0)
            {
                listView_camere.Columns.Add("Numero", 70);
                listView_camere.Columns.Add("Piano", 60);
                listView_camere.Columns.Add("Tipo", 80);
                listView_camere.Columns.Add("Prezzo/Notte", 100);
                listView_camere.Columns.Add("Stato", 100);
                listView_camere.Columns.Add("Servizio 1", 130);
                listView_camere.Columns.Add("Servizio 2", 130);
            }

            // Popola ComboBox solo la prima volta
            if (comboBox_piano_admin.Items.Count == 0)
            {
                comboBox_piano_admin.Items.Add("Tutti");
                comboBox_piano_admin.Items.Add("1");
                comboBox_piano_admin.Items.Add("2");
                comboBox_piano_admin.Items.Add("3");
                comboBox_piano_admin.SelectedIndex = 0; // ← triggera FiltraCamere automaticamente
            }
            else
            {
                FiltraCamere(); // se ComboBox già popolato, aggiorna solo la lista
            }
        }

        //Aggiungi camera
        private void button10_Click(object sender, EventArgs e)
        {
            FormAggiungiCamera finestra = new FormAggiungiCamera();

            if (finestra.ShowDialog() == DialogResult.OK)
            {
                DatabaseHelper db = new DatabaseHelper();

                if (db.CameraEsiste(finestra.NumeroCamera))
                {
                    MessageBox.Show("Esiste già una camera con questo numero.", "Errore",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                Camera nuova = new Camera
                {
                    Numero = finestra.NumeroCamera,
                    Piano = finestra.PianoCamera,
                    Tipo = finestra.TipoCamera,
                    PrezzoNotte = finestra.PrezzoCamera,
                    Stato = finestra.StatoCamera,
                    Servizio1 = finestra.Servizio1,
                    Servizio2 = finestra.Servizio2,
                };

                db.AggiungiCamera(nuova);
                MessageBox.Show("Camera aggiunta con successo!");
                CaricaCamere();
            }
        }

        //Elimina camera
        private void button9_Click(object sender, EventArgs e)
        {
            if (listView_camere.SelectedItems.Count == 0)
            {
                MessageBox.Show("Seleziona una camera da eliminare.", "Attenzione",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int numeroCamera = int.Parse(listView_camere.SelectedItems[0].Text);

            DialogResult conferma = MessageBox.Show(
                $"Sei sicuro di voler eliminare la camera n° {numeroCamera}?",
                "Conferma eliminazione",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (conferma == DialogResult.Yes)
            {
                DatabaseHelper db = new DatabaseHelper();
                if (db.EliminaCamera(numeroCamera))
                {
                    MessageBox.Show("Camera eliminata con successo.");
                    CaricaCamere();
                }
                else
                {
                    MessageBox.Show("Errore durante l'eliminazione.");
                }
            }
        }

        //Filtro per piano (admin)
        private void comboBox_piano_admin_SelectedIndexChanged(object sender, EventArgs e)
        {
            FiltraCamere();
        }

        private void FiltraCamere()
        {
            string filtro = comboBox_piano_admin.SelectedItem?.ToString() ?? "Tutti";

            listView_camere.Items.Clear();

            DatabaseHelper db = new DatabaseHelper();
            List<Camera> camere = db.GetCamere();

            foreach (Camera c in camere)
            {
                // Se "Tutti" mostra tutto, altrimenti filtra per piano
                if (filtro != "Tutti" && c.Piano.ToString() != filtro)
                    continue;

                ListViewItem item = new ListViewItem(c.Numero.ToString());
                item.SubItems.Add(c.Piano.ToString());
                item.SubItems.Add(c.Tipo);
                item.SubItems.Add($"€ {c.PrezzoNotte:0.00}");
                item.SubItems.Add(c.Stato);
                item.SubItems.Add(c.Servizio1);
                item.SubItems.Add(c.Servizio2);

                switch (c.Stato)
                {
                    case "disponibile": item.ForeColor = Color.Green; break;
                    case "occupata": item.ForeColor = Color.OrangeRed; break;
                    case "manutenzione": item.ForeColor = Color.Gray; break;
                }

                listView_camere.Items.Add(item);
            }
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

        //ACCOUNT
        #region ACCOUNT

        #region Aggiornamento dei dati nella sezione account
        private void AggiornaDatiAccount()
        {
            DatabaseHelper db = new DatabaseHelper();
            Utente u = db.GetUtente(utenteEmail);

            if (u == null) return;

            // Label con i dati
            label_nome_account.Text = u.Nome;
            label_cognome_account.Text = u.Cognome;
            label_email_account.Text = u.Email;
            label_dataNascita_account.Text = u.DataNascita.HasValue
                ? u.DataNascita.Value.ToString("dd/MM/yyyy") : "—";
            label_dataCreazione_account.Text = u.DataCreazione.ToString("dd/MM/yyyy HH:mm");

            // Precompila le TextBox di modifica
            textBox_nome_account.Text = u.Nome;
            textBox_cognome_account.Text = u.Cognome;
            textBox_email_account.Text = u.Email;
            textBox_password_account.Text = "";
            dateTimePicker_account.Value = u.DataNascita ?? DateTime.Today;
        }
        #endregion

        #region Salvataggio dei nuovi dati modificati
        //Salvataggio modifiche account
        private void button_salva_account_Click(object sender, EventArgs e)
        {
            string nuovaEmail = textBox_email_account.Text.Trim();
            string nome = textBox_nome_account.Text.Trim();
            string cognome = textBox_cognome_account.Text.Trim();
            string password = textBox_password_account.Text;

            if (string.IsNullOrEmpty(nome) || string.IsNullOrEmpty(cognome) || string.IsNullOrEmpty(nuovaEmail))
            {
                MessageBox.Show("Nome, cognome ed email sono obbligatori.", "Attenzione",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DatabaseHelper db = new DatabaseHelper();

            bool successo = db.ModificaUtente(
                utenteEmail,                    // email originale
                nuovaEmail,                     // nuova email
                nome,
                cognome,
                dateTimePicker_account.Value,
                password                        // vuota = non cambia la password
            );

            if (successo)
            {
                // Aggiorna variabili locali e settings
                utenteEmail = nuovaEmail;
                utenteNome = nome;
                Properties.Settings.Default.UtenteEmail = nuovaEmail;
                Properties.Settings.Default.UtenteNome = nome;
                Properties.Settings.Default.Save();

                MessageBox.Show("Modifiche salvate con successo!");
                AggiornaDatiAccount();
            }
            else
            {
                MessageBox.Show("Errore durante il salvataggio.");
            }
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