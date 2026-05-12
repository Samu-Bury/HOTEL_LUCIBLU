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

        //Utenti
        private bool accesso = false;
        private string utenteEmail = "";
        private string utenteNome = "";
        private string utenteTipo = "";

        //Camere
        private int pianoCameraSelezionato = 1;
        private Camera cameraSelezionata = null;
        private Button bottoneSelezionato = null;

        //Prenotazione
        private DateTime? dataCheckIn = null;
        private DateTime? dataCheckOut = null;
        private string metodoPagamento = "";

        #region Calendario
        int anno = 2026;
        int meseCorrente;
        #endregion

        //Account
        bool posizioneHome = true;

        #endregion

        public Form1()
        {
            InitializeComponent();

            #region Ripristina accesso salvato
            if (Properties.Settings.Default.AccessoSalvato)
            {
                accesso = true;
                utenteEmail = Properties.Settings.Default.UtenteEmail;
                utenteNome = Properties.Settings.Default.UtenteNome;
                utenteTipo = Properties.Settings.Default.UtenteTipo;

                AggiornaVisibilitaBottoniHome();
                AggiornaDatiAccount();

                if (utenteTipo == "admin")
                {
                    tabControl1.SelectedIndex = 8;
                    CaricaUtenti();
                    CaricaCamere();
                    CaricaPrenotazioni();
                }
                else
                    tabControl1.SelectedIndex = 2;
            }
            else
            {
                tabControl1.SelectedIndex = 2;
                AggiornaVisibilitaBottoniHome();
            }
            #endregion

            #region Bordi pannelli
            var panels = new List<Panel> {
                panel8, panel1, panel3, panel24, panel26, panel25,
                panel18, panel29, panel28, panel30,
                panel14, panel13, panel47, panel7
            };
            foreach (var p in panels)
                if (p != null) p.BorderStyle = BorderStyle.FixedSingle;
            #endregion

            this.AutoScaleMode = AutoScaleMode.None;
            AggiornaVisibilitaBottoniHome();

            #region Setting TabControl
            tabControl1.Appearance = TabAppearance.FlatButtons;
            tabControl1.ItemSize = new Size(0, 1);
            tabControl1.SizeMode = TabSizeMode.Fixed;
            tabControl2.Appearance = TabAppearance.FlatButtons;
            tabControl2.ItemSize = new Size(0, 1);
            tabControl2.SizeMode = TabSizeMode.Fixed;
            #endregion

            #region Setting bottoni Home
            Button[] bottoni = {
                button_accedi_home, button_account_home,
                button_prenotazioni_home, button_prenota_home
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

            #region Caricamento calendario
            meseCorrente = (DateTime.Now.Year == 2026) ? DateTime.Now.Month : 1;
            AggiornaCalendario();
            #endregion
        }

        #region GESTIONE SCHERMATE

        // HOME
        #region HOME

        private void AggiornaVisibilitaBottoniHome()
        {
            button_accedi_home.Visible = !accesso;
            button_account_home.Visible = accesso;
            button_prenota_home.Visible = accesso;
            button_prenotazioni_home.Visible = accesso;
        }

        private void button_accedi_home_Click(object sender, EventArgs e)
            => tabControl1.SelectedIndex = 0;

        private void button_prenota_home_Click(object sender, EventArgs e)
            => tabControl1.SelectedIndex = 3;

        private void button_prenotazioni_home_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex = 5;
            CaricaMiePrenotazioni();
        }

        private void button_account_home_Click(object sender, EventArgs e)
        {
            AggiornaDatiAccount();
            tabControl1.SelectedIndex = 4;
            posizioneHome = true;
        }

        #endregion

        // LOGIN
        #region LOGIN

        private void button_login_Click(object sender, EventArgs e)
        {
            string email = textBox_email_login.Text.Trim();
            string password = textBox_password_login.Text;

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Inserisci email e password.");
                return;
            }

            if (Utente.Login(email, password, out string nome, out string tipo))
            {
                Properties.Settings.Default.AccessoSalvato = true;
                Properties.Settings.Default.UtenteEmail = email;
                Properties.Settings.Default.UtenteNome = nome;
                Properties.Settings.Default.UtenteTipo = tipo;
                Properties.Settings.Default.Save();

                utenteEmail = email;
                utenteNome = nome;
                utenteTipo = tipo;
                accesso = true;

                AggiornaVisibilitaBottoniHome();
                AggiornaDatiAccount();

                tabControl1.SelectedIndex = (tipo == "admin") ? 8 : 2;
                MessageBox.Show($"Benvenuto, {nome}");
            }
            else
            {
                MessageBox.Show("Email o password errata.");
            }
        }

        private void checkBox_mostraPassword_CheckedChanged(object sender, EventArgs e)
            => textBox_password_login.UseSystemPasswordChar = !checkBox_mostraPassword.Checked;

        private void label_registrati_login_Click(object sender, EventArgs e)
            => tabControl1.SelectedIndex = 1;

        #endregion

        // REGISTER
        #region REGISTER

        private void button_register_Click(object sender, EventArgs e)
        {
            bool successo = Utente.Registra(
                textBox_email_register.Text.Trim(),
                textBox_password_register.Text,
                textBox_nome_register.Text.Trim(),
                textBox_cognome_register.Text.Trim(),
                dateTimePicker_data_register.Value
            );

            if (successo)
            {
                MessageBox.Show("Registrazione completata");
                tabControl1.SelectedIndex = 2;
            }
            else
            {
                MessageBox.Show("Email già in uso");
            }
        }

        private void checkBox_mostraPassword_register_CheckStateChanged(object sender, EventArgs e)
            => textBox_password_register.UseSystemPasswordChar = !checkBox_mostraPassword_register.Checked;

        private void label_accedi_register_Click(object sender, EventArgs e)
            => tabControl1.SelectedIndex = 0;

        #endregion

        // PRENOTA-DATA
        #region PRENOTA DATA

        private void button_prosegui_prenotaDate_Click(object sender, EventArgs e)
        {
            if (dataCheckIn == null || dataCheckOut == null)
            {
                MessageBox.Show("Seleziona le date di check-in e check-out.", "Attenzione",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            tabControl1.SelectedIndex = 6;
            pianoCameraSelezionato = 1;
            AggiornaBottoniCamere();
        }

        private void button2_Click(object sender, EventArgs e)
            => tabControl1.SelectedIndex = 2;

        #region Calendario

        Control TrovaControllo(Control parent, string nome)
        {
            foreach (Control c in parent.Controls)
            {
                if (c.Name == nome) return c;
                Control trovato = TrovaControllo(c, nome);
                if (trovato != null) return trovato;
            }
            return null;
        }

        void AggiornaCalendario()
        {
            Panel panel = TrovaControllo(tabPage3, "panel_calendario") as Panel;
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

                if (i - 1 >= start && giorno <= giorniNelMese)
                {
                    DateTime dataBtn = new DateTime(anno, meseCorrente, giorno);
                    btn.Visible = true;
                    btn.Text = giorno.ToString();
                    btn.Enabled = dataBtn >= DateTime.Today;
                    btn.BackColor = dataBtn < DateTime.Today ? Color.LightGray :
                                    dataBtn == DateTime.Today ? Color.LightGreen : Color.White;
                    btn.ForeColor = dataBtn < DateTime.Today ? Color.DarkGray : Color.Black;

                    btn.Click -= DataCalendario_Click;
                    btn.Click += DataCalendario_Click;
                    giorno++;
                }
                else
                {
                    btn.Visible = false;
                    btn.Text = "";
                    btn.Enabled = false;
                }
            }
            Colora_Range_Calendario();
        }

        private void Colora_Range_Calendario()
        {
            Panel panel = TrovaControllo(tabPage3, "panel_calendario") as Panel;
            if (panel == null) return;

            for (int i = 1; i <= 42; i++)
            {
                Button btn = TrovaControllo(panel, "data" + i) as Button;
                if (btn == null || !btn.Visible || !btn.Enabled) continue;
                if (!int.TryParse(btn.Text, out int giorno)) continue;

                DateTime dataBtn = new DateTime(anno, meseCorrente, giorno);

                // Reset colore base
                btn.BackColor = dataBtn < DateTime.Today ? Color.LightGray :
                                dataBtn == DateTime.Today ? Color.LightGreen : Color.White;
                btn.ForeColor = dataBtn < DateTime.Today ? Color.DarkGray : Color.Black;

                // Colora selezione
                if (dataCheckIn.HasValue && dataBtn == dataCheckIn.Value)
                { btn.BackColor = Color.SteelBlue; btn.ForeColor = Color.White; }
                else if (dataCheckOut.HasValue && dataBtn == dataCheckOut.Value)
                { btn.BackColor = Color.SteelBlue; btn.ForeColor = Color.White; }
                else if (dataCheckIn.HasValue && dataCheckOut.HasValue &&
                         dataBtn > dataCheckIn.Value && dataBtn < dataCheckOut.Value)
                { btn.BackColor = Color.LightSteelBlue; btn.ForeColor = Color.Black; }
            }
        }

        private void DataCalendario_Click(object sender, EventArgs e)
        {
            if (!(sender is Button btn) || !int.TryParse(btn.Text, out int giorno)) return;

            DateTime dataSelezionata = new DateTime(anno, meseCorrente, giorno);
            if (dataSelezionata < DateTime.Today) return;

            if (dataCheckIn == null || dataCheckOut != null)
            {
                dataCheckIn = dataSelezionata;
                dataCheckOut = null;
            }
            else
            {
                if (dataSelezionata <= dataCheckIn.Value)
                { dataCheckIn = dataSelezionata; dataCheckOut = null; }
                else
                    dataCheckOut = dataSelezionata;
            }

            label_checkin.Text = dataCheckIn.HasValue ? dataCheckIn.Value.ToString("dd/MM/yyyy") : "—";
            label_checkout.Text = dataCheckOut.HasValue ? dataCheckOut.Value.ToString("dd/MM/yyyy") : "—";

            if (dataCheckIn.HasValue && dataCheckOut.HasValue)
            {
                int notti = (int)(dataCheckOut.Value - dataCheckIn.Value).TotalDays;
                label_notti.Text = $"{notti} nott{(notti == 1 ? "e" : "i")}";
            }
            else
                label_notti.Text = "—";

            Colora_Range_Calendario();
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

        #endregion

        // PRENOTA-CAMERA
        #region PRENOTA CAMERA

        private void button_prosegui_prenotaCamere_Click(object sender, EventArgs e)
        {
            if (cameraSelezionata == null)
            {
                MessageBox.Show("Seleziona una camera.", "Attenzione",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (cameraSelezionata.Stato != "disponibile")
            {
                MessageBox.Show("La camera selezionata non è disponibile.", "Attenzione",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int notti = (int)(dataCheckOut.Value - dataCheckIn.Value).TotalDays;
            decimal totale = cameraSelezionata.PrezzoNotte * notti;

            label_checkin_pagamento.Text = dataCheckIn.Value.ToString("dd/MM/yyyy");
            label_checkout_pagamento.Text = dataCheckOut.Value.ToString("dd/MM/yyyy");
            label_notti_pagamento.Text = $"{notti} nott{(notti == 1 ? "e" : "i")}";
            label_numCamera_pagamento.Text = $"Camera {cameraSelezionata.Numero} ({cameraSelezionata.Tipo})";
            label_prezzoNotte_pagamento.Text = $"€ {cameraSelezionata.PrezzoNotte:0.00} x {notti} notti";
            label_totale_pagamento.Text = $"€ {totale:0.00}";
            metodoPagamento = "";

            tabControl1.SelectedIndex = 7;
        }

        private void button4_Click(object sender, EventArgs e) => tabControl1.SelectedIndex = 3;
        private void button3_Click(object sender, EventArgs e) => tabControl1.SelectedIndex = 2;

        private void button_pianoTerra_Click(object sender, EventArgs e)
        { pianoCameraSelezionato = 1; cameraSelezionata = null; AggiornaBottoniCamere(); }

        private void button_piano1_Click(object sender, EventArgs e)
        { pianoCameraSelezionato = 2; cameraSelezionata = null; AggiornaBottoniCamere(); }

        private void button_piano2_Click(object sender, EventArgs e)
        { pianoCameraSelezionato = 3; cameraSelezionata = null; AggiornaBottoniCamere(); }

        private void AggiornaBottoniCamere()
        {
            // ← USA Camera.GetTutte() invece di DatabaseHelper
            List<Camera> camere = Camera.GetTutte()
                .Where(c => c.Piano == pianoCameraSelezionato)
                .ToList();

            Button[] bottonìCamere = {
                camera1,  camera2,  camera3,  camera4,  camera5,
                camera6,  camera7,  camera8,  camera9,  camera10,
                camera11, camera12, camera13, camera14, camera15
            };

            foreach (Button btn in bottonìCamere)
            {
                btn.Text = "—";
                btn.BackColor = Color.LightGray;
                btn.ForeColor = Color.DarkGray;
                btn.Enabled = false;
                btn.Tag = null;
            }

            for (int i = 0; i < camere.Count && i < bottonìCamere.Length; i++)
            {
                Camera c = camere[i];
                Button btn = bottonìCamere[i];

                btn.Text = $"{c.Numero}\n{c.Tipo}";
                btn.Tag = c;
                btn.Enabled = true;

                switch (c.Stato)
                {
                    case "disponibile": btn.BackColor = Color.MediumSeaGreen; btn.ForeColor = Color.White; break;
                    case "occupata": btn.BackColor = Color.IndianRed; btn.ForeColor = Color.White; break;
                    case "manutenzione": btn.BackColor = Color.Goldenrod; btn.ForeColor = Color.White; break;
                }
            }

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

            if (bottoneSelezionato != null && bottoneSelezionato != btnPremuto)
            {
                Camera vecchia = bottoneSelezionato.Tag as Camera;
                if (vecchia != null)
                    switch (vecchia.Stato)
                    {
                        case "disponibile": bottoneSelezionato.BackColor = Color.MediumSeaGreen; break;
                        case "occupata": bottoneSelezionato.BackColor = Color.IndianRed; break;
                        case "manutenzione": bottoneSelezionato.BackColor = Color.Goldenrod; break;
                    }
            }

            btnPremuto.BackColor = Color.DodgerBlue;
            btnPremuto.ForeColor = Color.White;
            bottoneSelezionato = btnPremuto;
            cameraSelezionata = c;

            label_camera_tipo.Text = c.Tipo;
            label_camera_prezzo.Text = $"€ {c.PrezzoNotte:0.00} / notte";
            label_camera_piano.Text = $"{c.Piano}";
            label_camera_servizio1.Text = $"1) {(string.IsNullOrEmpty(c.Servizio1) ? "—" : c.Servizio1)}";
            label_camera_servizio2.Text = $"2) {(string.IsNullOrEmpty(c.Servizio2) ? "—" : c.Servizio2)}";
            label_camera_numero.Text = $"🛏 Camera {c.Numero}";
            label_stato_camere.Text = c.Stato;
        }

        private void camera10_Click(object sender, EventArgs e)
            => SelezionaCamera(sender as Button);

        #endregion

        // ACCOUNT
        #region ACCOUNT

        private void button1_Click(object sender, EventArgs e)
            => tabControl1.SelectedIndex = posizioneHome ? 2 : 8;

        private void button7_Click(object sender, EventArgs e)
        {
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

        // PRENOTA-PAGAMENTO
        #region PRENOTA PAGAMENTO

        private void button35_Click(object sender, EventArgs e) => tabControl1.SelectedIndex = 2;
        private void button41_Click(object sender, EventArgs e) => tabControl1.SelectedIndex = 6;

        private void button_carta_Click(object sender, EventArgs e)
        { metodoPagamento = "Carta di Credito"; label_metodo_pagamento.Text = $"M.Pagamento = {metodoPagamento}"; }

        private void button_paypal_Click(object sender, EventArgs e)
        { metodoPagamento = "PayPal"; label_metodo_pagamento.Text = $"M.Pagamento = {metodoPagamento}"; }

        private void button_bonifico_Click(object sender, EventArgs e)
        { metodoPagamento = "Bonifico"; label_metodo_pagamento.Text = $"M.Pagamento = {metodoPagamento}"; }

        private void button_hotel_Click(object sender, EventArgs e)
        { metodoPagamento = "In Hotel"; label_metodo_pagamento.Text = $"M.Pagamento = {metodoPagamento}"; }

        private void button_conferma_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(metodoPagamento))
            {
                MessageBox.Show("Seleziona un metodo di pagamento.", "Attenzione",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int notti = (int)(dataCheckOut.Value - dataCheckIn.Value).TotalDays;
            decimal totale = cameraSelezionata.PrezzoNotte * notti;

            Prenotazione p = new Prenotazione
            {
                Email = utenteEmail,
                NumeroCamera = cameraSelezionata.Numero.ToString(),
                DataCheckIn = dataCheckIn.Value,
                DataCheckOut = dataCheckOut.Value,
                PrezzoTotale = totale,
                MetodoPagamento = metodoPagamento
            };

            Cliente cliente = new Cliente { Email = utenteEmail };

            if (cliente.EffettuaPrenotazione(p))
            {
                MessageBox.Show(
                    $"Prenotazione confermata!\n\nCamera {cameraSelezionata.Numero} — {notti} notti\nTotale: € {totale:0.00}",
                    "Prenotazione effettuata", MessageBoxButtons.OK, MessageBoxIcon.Information);

                dataCheckIn = null;
                dataCheckOut = null;
                cameraSelezionata = null;
                bottoneSelezionato = null;
                metodoPagamento = "";

                tabControl1.SelectedIndex = 2;
                AggiornaCalendario();
            }
            else
            {
                MessageBox.Show("Errore durante la prenotazione.", "Errore",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

        // GESTIONE ADMIN
        #region GESTIONE ADMIN

        private void button45_Click(object sender, EventArgs e) => tabControl2.SelectedIndex = 0;

        private void button44_Click(object sender, EventArgs e)
        { tabControl2.SelectedIndex = 0; CaricaPrenotazioni(); }

        private void button46_Click(object sender, EventArgs e)
        { tabControl2.SelectedIndex = 1; CaricaCamere(); }

        private void button47_Click(object sender, EventArgs e)
        { tabControl2.SelectedIndex = 2; CaricaUtenti(); }

        private void button8_Click(object sender, EventArgs e)
        { tabControl1.SelectedIndex = 4; posizioneHome = false; AggiornaDatiAccount(); }

        #region UTENTI

        private void CaricaUtenti()
        {
            listView_user.Items.Clear();
            listView_user.View = View.Details;
            listView_user.FullRowSelect = true;
            listView_user.GridLines = true;

            if (listView_user.Columns.Count == 0)
            {
                listView_user.Columns.Add("Email", 200);
                listView_user.Columns.Add("Nome", 100);
                listView_user.Columns.Add("Cognome", 100);
                listView_user.Columns.Add("Data Nascita", 100);
                listView_user.Columns.Add("Registrato il", 130);
                listView_user.Columns.Add("Tipo", 70);
            }

            // ← USA Admin.GetTuttiGliUtenti() invece di DatabaseHelper
            Admin admin = new Admin();
            List<Utente> utenti = admin.GetTuttiGliUtenti();

            foreach (Utente u in utenti)
            {
                ListViewItem item = new ListViewItem(u.Email);
                item.SubItems.Add(u.Nome);
                item.SubItems.Add(u.Cognome);
                item.SubItems.Add(u.DataNascita.HasValue ? u.DataNascita.Value.ToString("dd/MM/yyyy") : "—");
                item.SubItems.Add(u.DataCreazione.ToString("dd/MM/yyyy HH:mm"));
                item.SubItems.Add(u.Tipo);
                if (u.Tipo == "admin") item.ForeColor = Color.RoyalBlue;
                listView_user.Items.Add(item);
            }
        }

        private void button54_Click(object sender, EventArgs e)
        {
            if (listView_user.SelectedItems.Count == 0)
            {
                MessageBox.Show("Seleziona un utente da eliminare.", "Attenzione",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string emailSelezionata = listView_user.SelectedItems[0].Text;

            if (MessageBox.Show($"Sei sicuro di voler eliminare l'utente:\n{emailSelezionata}?",
                "Conferma eliminazione", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                // ← USA Utente.Elimina() invece di DatabaseHelper
                Utente u = new Utente { Email = emailSelezionata };
                if (u.Elimina())
                { MessageBox.Show("Utente eliminato con successo."); CaricaUtenti(); }
                else
                    MessageBox.Show("Errore durante l'eliminazione.");
            }
        }

        private void button55_Click(object sender, EventArgs e)
        {
            FormAggiungiUtente finestra = new FormAggiungiUtente();
            if (finestra.ShowDialog() != DialogResult.OK) return;

            if (Utente.EmailEsiste(finestra.EmailInserita))
            {
                MessageBox.Show("Email già in uso.", "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // ← USA Utente.Registra() invece di DatabaseHelper
            if (Utente.Registra(finestra.EmailInserita, finestra.PasswordInserita,
                finestra.NomeInserito, finestra.CognomeInserito,
                finestra.DataNascitaInserita, finestra.TipoInserito))
            { MessageBox.Show("Utente aggiunto con successo!"); CaricaUtenti(); }
            else
                MessageBox.Show("Errore durante l'aggiunta.");
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (listView_user.SelectedItems.Count == 0)
            {
                MessageBox.Show("Seleziona un utente da modificare.", "Attenzione",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string emailSelezionata = listView_user.SelectedItems[0].Text;

            // ← USA Utente.GetUtente() invece di DatabaseHelper
            Utente u = Utente.GetUtente(emailSelezionata);
            if (u == null) return;

            FormAggiungiUtente finestra = new FormAggiungiUtente(u);
            if (finestra.ShowDialog() != DialogResult.OK) return;

            // ← USA u.Modifica() invece di DatabaseHelper
            u.Nome = finestra.NomeInserito;
            u.Cognome = finestra.CognomeInserito;
            u.DataNascita = finestra.DataNascitaInserita;
            u.Tipo = finestra.TipoInserito;

            if (u.Modifica(finestra.EmailInserita, finestra.PasswordInserita))
            { MessageBox.Show("Utente modificato con successo!"); CaricaUtenti(); }
            else
                MessageBox.Show("Errore durante la modifica.");
        }

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

            if (comboBox_piano_admin.Items.Count == 0)
            {
                comboBox_piano_admin.Items.AddRange(new[] { "Tutti", "1", "2", "3" });
                comboBox_piano_admin.SelectedIndex = 0;
            }
            else
                FiltraCamere();
        }

        private void button10_Click(object sender, EventArgs e)
        {
            FormAggiungiCamera finestra = new FormAggiungiCamera();
            if (finestra.ShowDialog() != DialogResult.OK) return;

            // ← USA Camera.Esiste() e Camera.Salva() invece di DatabaseHelper
            if (Camera.Esiste(finestra.NumeroCamera))
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
                Servizio2 = finestra.Servizio2
            };
            nuova.Salva();
            MessageBox.Show("Camera aggiunta con successo!");
            CaricaCamere();
        }

        private void button9_Click(object sender, EventArgs e)
        {
            if (listView_camere.SelectedItems.Count == 0)
            {
                MessageBox.Show("Seleziona una camera da eliminare.", "Attenzione",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int numero = int.Parse(listView_camere.SelectedItems[0].Text);

            if (MessageBox.Show($"Sei sicuro di voler eliminare la camera n° {numero}?",
                "Conferma eliminazione", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                // ← USA Camera.Elimina() invece di DatabaseHelper
                if (Camera.Elimina(numero))
                { MessageBox.Show("Camera eliminata con successo."); CaricaCamere(); }
                else
                    MessageBox.Show("Errore durante l'eliminazione.");
            }
        }

        private void comboBox_piano_admin_SelectedIndexChanged(object sender, EventArgs e)
            => FiltraCamere();

        private void FiltraCamere()
        {
            string filtro = comboBox_piano_admin.SelectedItem?.ToString() ?? "Tutti";
            listView_camere.Items.Clear();

            // ← USA Camera.GetTutte() invece di DatabaseHelper
            foreach (Camera c in Camera.GetTutte())
            {
                if (filtro != "Tutti" && c.Piano.ToString() != filtro) continue;

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

        #region PRENOTAZIONI

        private void CaricaPrenotazioni()
        {
            listView_prenotazioni.Items.Clear();
            listView_prenotazioni.View = View.Details;
            listView_prenotazioni.FullRowSelect = true;
            listView_prenotazioni.GridLines = true;

            if (listView_prenotazioni.Columns.Count == 0)
            {
                listView_prenotazioni.Columns.Add("Codice", 60);
                listView_prenotazioni.Columns.Add("Email", 180);
                listView_prenotazioni.Columns.Add("Camera", 65);
                listView_prenotazioni.Columns.Add("Check-In", 90);
                listView_prenotazioni.Columns.Add("Check-Out", 90);
                listView_prenotazioni.Columns.Add("Notti", 55);
                listView_prenotazioni.Columns.Add("Totale", 80);
                listView_prenotazioni.Columns.Add("Pagamento", 120);
                listView_prenotazioni.Columns.Add("Stato", 90);
                listView_prenotazioni.Columns.Add("Prenotato il", 130);
            }

            // ← USA Prenotazione.GetTutte() invece di DatabaseHelper
            foreach (Prenotazione p in Prenotazione.GetTutte())
            {
                ListViewItem item = new ListViewItem(p.CodicePrenotazione);
                item.SubItems.Add(p.Email);
                item.SubItems.Add(p.NumeroCamera);
                item.SubItems.Add(p.DataCheckIn.ToString("dd/MM/yyyy"));
                item.SubItems.Add(p.DataCheckOut.ToString("dd/MM/yyyy"));
                item.SubItems.Add(p.Notti.ToString());
                item.SubItems.Add($"€ {p.PrezzoTotale:0.00}");
                item.SubItems.Add(p.MetodoPagamento);
                item.SubItems.Add(p.Stato);
                item.SubItems.Add(p.DataPrenotazione.ToString("dd/MM/yyyy HH:mm"));

                switch (p.Stato)
                {
                    case "confermata": item.ForeColor = Color.Green; break;
                    case "in attesa": item.ForeColor = Color.DarkOrange; break;
                    case "cancellata": item.ForeColor = Color.Gray; break;
                }
                listView_prenotazioni.Items.Add(item);
            }
        }

        private void button48_Click(object sender, EventArgs e)
        {
            if (listView_prenotazioni.SelectedItems.Count == 0)
            {
                MessageBox.Show("Seleziona una prenotazione.", "Attenzione",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string codiceStr = listView_prenotazioni.SelectedItems[0].Text;
            string statoAttuale = listView_prenotazioni.SelectedItems[0].SubItems[8].Text;
            int codice = int.Parse(codiceStr);

            if (statoAttuale == "cancellata")
            {
                if (MessageBox.Show("La prenotazione è già cancellata.\nVuoi eliminarla definitivamente?",
                    "Elimina definitivamente", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    // ← USA Prenotazione.EliminaFisicamente() invece di DatabaseHelper
                    if (Prenotazione.EliminaFisicamente(codice))
                    { MessageBox.Show("Prenotazione eliminata definitivamente."); CaricaPrenotazioni(); }
                }
                return;
            }

            if (MessageBox.Show($"Sei sicuro di voler cancellare la prenotazione #{codiceStr}?",
                "Conferma cancellazione", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                // ← USA Prenotazione.Cancella() invece di DatabaseHelper
                if (Prenotazione.Cancella(codice))
                { MessageBox.Show("Prenotazione cancellata."); CaricaPrenotazioni(); }
                else
                    MessageBox.Show("Errore durante la cancellazione.");
            }
        }

        #endregion

        #endregion

        // LE MIE PRENOTAZIONI
        #region LE MIE PRENOTAZIONI

        private void button42_Click(object sender, EventArgs e)
            => tabControl1.SelectedIndex = 2;

        private void CaricaMiePrenotazioni()
        {
            listView_mieprenotazioni.Items.Clear();
            listView_mieprenotazioni.View = View.Details;
            listView_mieprenotazioni.FullRowSelect = true;
            listView_mieprenotazioni.GridLines = true;

            if (listView_mieprenotazioni.Columns.Count == 0)
            {
                listView_mieprenotazioni.Columns.Add("Codice", 60);
                listView_mieprenotazioni.Columns.Add("Camera", 65);
                listView_mieprenotazioni.Columns.Add("Check-In", 90);
                listView_mieprenotazioni.Columns.Add("Check-Out", 90);
                listView_mieprenotazioni.Columns.Add("Notti", 55);
                listView_mieprenotazioni.Columns.Add("Totale", 80);
                listView_mieprenotazioni.Columns.Add("Pagamento", 120);
                listView_mieprenotazioni.Columns.Add("Stato", 90);
                listView_mieprenotazioni.Columns.Add("Prenotato il", 130);
            }

            // ← USA Cliente.VisualizzaStorico() invece di DatabaseHelper
            Cliente cliente = new Cliente { Email = utenteEmail };

            foreach (Prenotazione p in cliente.VisualizzaStorico())
            {
                ListViewItem item = new ListViewItem(p.CodicePrenotazione);
                item.SubItems.Add(p.NumeroCamera);
                item.SubItems.Add(p.DataCheckIn.ToString("dd/MM/yyyy"));
                item.SubItems.Add(p.DataCheckOut.ToString("dd/MM/yyyy"));
                item.SubItems.Add(p.Notti.ToString());
                item.SubItems.Add($"€ {p.PrezzoTotale:0.00}");
                item.SubItems.Add(p.MetodoPagamento);
                item.SubItems.Add(p.Stato);
                item.SubItems.Add(p.DataPrenotazione.ToString("dd/MM/yyyy HH:mm"));

                switch (p.Stato)
                {
                    case "confermata": item.ForeColor = Color.Green; break;
                    case "in attesa": item.ForeColor = Color.DarkOrange; break;
                    case "cancellata": item.ForeColor = Color.Gray; break;
                }
                listView_mieprenotazioni.Items.Add(item);
            }
        }

        private void button43_Click(object sender, EventArgs e)
        {
            if (listView_mieprenotazioni.SelectedItems.Count == 0)
            {
                MessageBox.Show("Seleziona una prenotazione da annullare.", "Attenzione",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string codiceStr = listView_mieprenotazioni.SelectedItems[0].Text;
            string statoAttuale = listView_mieprenotazioni.SelectedItems[0].SubItems[7].Text;

            if (statoAttuale != "in attesa")
            {
                MessageBox.Show("Puoi annullare solo le prenotazioni in attesa.", "Operazione non consentita",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (MessageBox.Show($"Sei sicuro di voler annullare la prenotazione #{codiceStr}?",
                "Conferma annullamento", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                // ← USA Cliente.AnnullaPrenotazione() invece di DatabaseHelper
                Cliente cliente = new Cliente { Email = utenteEmail };
                if (cliente.AnnullaPrenotazione(int.Parse(codiceStr)))
                { MessageBox.Show("Prenotazione annullata con successo."); CaricaMiePrenotazioni(); }
                else
                    MessageBox.Show("Errore durante l'annullamento");
            }
        }

        #endregion

        // ACCOUNT UTENTE
        #region ACCOUNT UTENTE

        private void AggiornaDatiAccount()
        {
            // ← USA Utente.GetUtente() invece di DatabaseHelper
            Utente u = Utente.GetUtente(utenteEmail);
            if (u == null) return;

            label_nome_account.Text = u.Nome;
            label_cognome_account.Text = u.Cognome;
            label_email_account.Text = u.Email;
            label_dataNascita_account.Text = u.DataNascita.HasValue
                ? u.DataNascita.Value.ToString("dd/MM/yyyy") : "—";
            label_dataCreazione_account.Text = u.DataCreazione.ToString("dd/MM/yyyy HH:mm");

            textBox_nome_account.Text = u.Nome;
            textBox_cognome_account.Text = u.Cognome;
            textBox_email_account.Text = u.Email;
            textBox_password_account.Text = "";
            dateTimePicker_account.Value = u.DataNascita ?? DateTime.Today;
        }

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

            // ← USA u.Modifica() invece di DatabaseHelper
            Utente u = new Utente
            {
                Email = utenteEmail,
                Nome = nome,
                Cognome = cognome,
                DataNascita = dateTimePicker_account.Value
            };

            if (u.Modifica(nuovaEmail, password))
            {
                utenteEmail = nuovaEmail;
                utenteNome = nome;
                Properties.Settings.Default.UtenteEmail = nuovaEmail;
                Properties.Settings.Default.UtenteNome = nome;
                Properties.Settings.Default.Save();

                MessageBox.Show("Modifiche salvate con successo");
                AggiornaDatiAccount();
            }
            else
                MessageBox.Show("Errore durante il salvataggio.");
        }

        #endregion

        #endregion
    }
}