using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace HOTEL_LUCIBLU
{
    public partial class Form1 : Form
    {
        bool accesso = false;

        #region CALENDARIO VARIABILI
        // ============================================================
        //  CALENDARIO - variabili di stato
        // ============================================================
        private DateTime meseCorrente;
        private HashSet<DateTime> dateDisabilitate = new HashSet<DateTime>();

        private DateTime? dataInizio = null;
        private DateTime? dataFine = null;

        // Colori
        private readonly Color CELESTE_SELEZIONE = Color.FromArgb(0, 174, 239);   // data cliccata
        private readonly Color CELESTE_CHIARO = Color.FromArgb(179, 229, 252);  // intervallo
        private readonly Color ROSSO_SFONDO = Color.FromArgb(220, 80, 80);   // disabilitato
        private readonly Color BLU_INTESTAZIONE = Color.FromArgb(0, 120, 215);    // header giorni
        private readonly Color BLU_WEEKEND = Color.FromArgb(173, 216, 230);  // sab/dom

        private const int CAL_X = 20;
        private const int CAL_Y = 80;   // abbassato
        private const int CAL_LARGHEZZA = 630;

        #endregion

        public Form1()
        {
            InitializeComponent();
            tabControl1.SelectedIndex = 2;
            ControlloAccesso();
            this.AutoScaleMode = AutoScaleMode.None;

            #region Calendario setting date
            meseCorrente = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);

            // ============================================================
            //  IMPOSTA QUI LE DATE NON DISPONIBILI
            //
            //  Giorno singolo:
            //      DisabilitaData(new DateTime(2025, 3, 12));
            // ============================================================
            DisabilitaData(new DateTime(2025, 3, 12));
            DisabilitaData(new DateTime(2025, 3, 13));
            // ============================================================

            CreaCalendario();

            #endregion

            tabControl1.SelectedIndex = 0;
        }

        #region CALENDARIO SETTING BASE
        // ============================================================
        //  API PUBBLICA - gestione date da codice
        // ============================================================
        public void DisabilitaData(DateTime data)
        {
            dateDisabilitate.Add(data.Date);
        }

        public void DisabilitaRange(DateTime inizio, DateTime fine)
        {
            for (DateTime d = inizio.Date; d <= fine.Date; d = d.AddDays(1))
                dateDisabilitate.Add(d);
        }

        public void AbilitaData(DateTime data)
        {
            dateDisabilitate.Remove(data.Date);
        }

        public void AbilitaTutte()
        {
            dateDisabilitate.Clear();
        }

        // ============================================================
        //  CALENDARIO - disegno principale
        // ============================================================
        private void CreaCalendario()
        {
            TabPage pagina = tabControl1.TabPages[3];

            // Rimuovi vecchi controlli calendario
            pagina.Controls.Cast<Control>()
                .Where(c => c.Tag != null && c.Tag.ToString() == "CAL")
                .ToList()
                .ForEach(c => pagina.Controls.Remove(c));

            int x = CAL_X;
            int y = CAL_Y;
            int w = CAL_LARGHEZZA;
            int colW = w / 7;
            int rigaH = 36;
            int intestH = 44;

            // ---- Navigazione mese ----
            Button btnPrec = CreaBottoneNav("◄", new Point(x, y + 4));
            btnPrec.Click += (s, e) => { meseCorrente = meseCorrente.AddMonths(-1); CreaCalendario(); };
            pagina.Controls.Add(btnPrec);

            string titolo = meseCorrente.ToString("MMMM yyyy", new System.Globalization.CultureInfo("it-IT"));
            titolo = char.ToUpper(titolo[0]) + titolo.Substring(1);
            Label lblTitolo = new Label
            {
                Text = titolo,
                Location = new Point(x + 36, y),
                Size = new Size(w - 72, 44),
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Arial", 14, FontStyle.Bold),
                Tag = "CAL"
            };
            pagina.Controls.Add(lblTitolo);

            Button btnSucc = CreaBottoneNav("►", new Point(x + w - 36, y + 4));
            btnSucc.Click += (s, e) => { meseCorrente = meseCorrente.AddMonths(1); CreaCalendario(); };
            pagina.Controls.Add(btnSucc);

            // ---- Intestazioni giorni ----
            string[] giorni = { "Lun", "Mar", "Mer", "Gio", "Ven", "Sab", "Dom" };
            for (int i = 0; i < 7; i++)
            {
                Button btnG = new Button
                {
                    Text = giorni[i],
                    Location = new Point(x + i * colW, y + intestH),
                    Size = new Size(colW - 2, rigaH),
                    BackColor = BLU_INTESTAZIONE,
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat,
                    Font = new Font("Arial", 10, FontStyle.Bold),
                    Tag = "CAL",
                    Enabled = false
                };
                btnG.FlatAppearance.BorderColor = Color.LightGray;
                pagina.Controls.Add(btnG);
            }

            // ---- Calcola range valido ----
            DateTime? rangeStart = null;
            DateTime? rangeEnd = null;
            if (dataInizio.HasValue && dataFine.HasValue)
            {
                rangeStart = dataInizio < dataFine ? dataInizio : dataFine;
                rangeEnd = dataInizio < dataFine ? dataFine : dataInizio;

                for (DateTime d = rangeStart.Value.AddDays(1); d < rangeEnd.Value; d = d.AddDays(1))
                {
                    if (dateDisabilitate.Contains(d.Date))
                    {
                        rangeStart = null;
                        rangeEnd = null;
                        break;
                    }
                }
            }

            // ---- Griglia giorni ----
            int primoGiorno = (int)meseCorrente.DayOfWeek;
            primoGiorno = primoGiorno == 0 ? 6 : primoGiorno - 1;

            int giorniNelMese = DateTime.DaysInMonth(meseCorrente.Year, meseCorrente.Month);
            int col = primoGiorno;
            int riga = 0;

            for (int giorno = 1; giorno <= giorniNelMese; giorno++)
            {
                DateTime data = new DateTime(meseCorrente.Year, meseCorrente.Month, giorno);
                bool disabilitata = dateDisabilitate.Contains(data.Date);
                bool isInizio = dataInizio.HasValue && data.Date == dataInizio.Value.Date;
                bool isFine = dataFine.HasValue && data.Date == dataFine.Value.Date;
                bool isSelezionata = isInizio || isFine;
                bool isNelRange = rangeStart.HasValue && rangeEnd.HasValue &&
                                    data.Date > rangeStart.Value.Date &&
                                    data.Date < rangeEnd.Value.Date;
                bool sabDom = data.DayOfWeek == DayOfWeek.Saturday ||
                                    data.DayOfWeek == DayOfWeek.Sunday;

                Button btn = new Button
                {
                    Text = giorno.ToString(),
                    Location = new Point(x + col * colW, y + intestH + rigaH + 2 + riga * rigaH),
                    Size = new Size(colW - 2, rigaH - 2),
                    FlatStyle = FlatStyle.Flat,
                    Font = new Font("Arial", 10),
                    Tag = "CAL",
                    Cursor = disabilitata ? Cursors.No : Cursors.Hand
                };

                // Colori — priorità: disabilitato > selezionato > range > weekend > bianco
                if (disabilitata)
                {
                    btn.BackColor = ROSSO_SFONDO;
                    btn.ForeColor = Color.White;
                    btn.Font = new Font("Arial", 10, FontStyle.Bold);
                    btn.FlatAppearance.BorderColor = Color.DarkRed;
                }
                else if (isSelezionata)
                {
                    btn.BackColor = CELESTE_SELEZIONE;
                    btn.ForeColor = Color.White;
                    btn.Font = new Font("Arial", 10, FontStyle.Bold);
                    btn.FlatAppearance.BorderColor = Color.DeepSkyBlue;
                }
                else if (isNelRange)
                {
                    btn.BackColor = CELESTE_CHIARO;
                    btn.ForeColor = Color.Black;
                    btn.FlatAppearance.BorderColor = Color.SkyBlue;
                }
                else
                {
                    btn.BackColor = Color.White;
                    btn.ForeColor = Color.Black;
                    btn.FlatAppearance.BorderColor = Color.LightGray;
                }

                // Click - logica range
                DateTime dataCopia = data;
                btn.Click += (s, e) =>
                {
                    if (disabilitata) return;

                    if (!dataInizio.HasValue || dataFine.HasValue)
                    {
                        // Nessuna selezione attiva oppure range già completo → ricomincia
                        dataInizio = dataCopia;
                        dataFine = null;
                    }
                    else
                    {
                        // Seconda selezione: verifica che non passi per disabilitate
                        DateTime s2 = dataInizio.Value < dataCopia ? dataInizio.Value : dataCopia;
                        DateTime e2 = dataInizio.Value < dataCopia ? dataCopia : dataInizio.Value;

                        bool bloccato = false;
                        for (DateTime dd = s2.AddDays(1); dd < e2; dd = dd.AddDays(1))
                        {
                            if (dateDisabilitate.Contains(dd.Date)) { bloccato = true; break; }
                        }

                        if (bloccato)
                        {
                            // Il range attraversa una data disabilitata → ricomincia da qui
                            dataInizio = dataCopia;
                            dataFine = null;
                        }
                        else
                        {
                            dataFine = dataCopia;
                        }
                    }

                    CreaCalendario();
                };

                pagina.Controls.Add(btn);

                col++;
                if (col >= 7) { col = 0; riga++; }
            }

            // ---- Aggiorna label checkin / checkout / notti ----
            TabPage pg = tabControl1.TabPages[3];

            if (!dataInizio.HasValue)
            {
                label_checkin.Text = "-";
                label_checkout.Text = "-";
                label_notti.Text = "-";
            }
            else if (!dataFine.HasValue)
            {
                label_checkin.Text = dataInizio.Value.ToString("dd/MM/yyyy");
                label_checkout.Text = "-";
                label_notti.Text = "-";
            }
            else
            {
                DateTime s3 = dataInizio < dataFine ? dataInizio.Value : dataFine.Value;
                DateTime e3 = dataInizio < dataFine ? dataFine.Value : dataInizio.Value;
                int notti = (int)(e3 - s3).TotalDays;
                label_checkin.Text = s3.ToString("dd/MM/yyyy");
                label_checkout.Text = e3.ToString("dd/MM/yyyy");
                label_notti.Text = notti.ToString();
            }

            // ---- Pulsante Reset ----
            int righe2 = ((primoGiorno + giorniNelMese - 1) / 7) + 1;
            int calAltezza2 = intestH + rigaH + 2 + righe2 * rigaH;

            Button btnReset = new Button
            {
                Text = "✕ Reset",
                Location = new Point(x + w - 110, y + calAltezza2 + 8),
                Size = new Size(110, 28),
                BackColor = Color.White,
                ForeColor = Color.Gray,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Arial", 9),
                Tag = "CAL",
                Cursor = Cursors.Hand
            };
            btnReset.FlatAppearance.BorderColor = Color.LightGray;
            btnReset.Click += (s, e) =>
            {
                dataInizio = null;
                dataFine = null;
                CreaCalendario();
            };
            pagina.Controls.Add(btnReset);
        }

        private Button CreaBottoneNav(string testo, Point pos)
        {
            Button b = new Button
            {
                Text = testo,
                Location = pos,
                Size = new Size(36, 36),
                BackColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Arial", 10, FontStyle.Bold),
                Tag = "CAL",
                Cursor = Cursors.Hand
            };
            b.FlatAppearance.BorderColor = Color.Gray;
            return b;
        }
#endregion


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

        private void label6_Click(object sender, EventArgs e) { tabControl1.SelectedIndex = 1; }
        private void label7_Click(object sender, EventArgs e) { tabControl1.SelectedIndex = 0; }

        private void button_login_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex = 2;
            accesso = true;
            ControlloAccesso();
        }

        private void button_register_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex = 2;
            accesso = true;
            ControlloAccesso();
        }

        private void button4_Click(object sender, EventArgs e) { tabControl1.SelectedIndex = 6; }
        private void button_prenota_Click_1(object sender, EventArgs e) { tabControl1.SelectedIndex = 3; }
    }
}