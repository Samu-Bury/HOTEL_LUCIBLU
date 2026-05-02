using System;
using System.Windows.Forms;

namespace HOTEL_LUCIBLU
{
    public partial class FormAggiungiUtente : Form
    {
        private bool modalitaModifica = false;

        // Proprietà pubbliche
        public string EmailInserita => textBox_email_aggiungi.Text.Trim();
        public string PasswordInserita => textBox_password_aggiungi.Text;
        public string NomeInserito => textBox_nome_aggiungi.Text.Trim();
        public string CognomeInserito => textBox_cognome_aggiungi.Text.Trim();
        public DateTime DataNascitaInserita => dateTimePicker_data_aggiungi.Value;
        public string TipoInserito => comboBox_tipo_aggiungi.SelectedItem?.ToString() ?? "utente";

        // Costruttore AGGIUNGI (come prima)
        public FormAggiungiUtente()
        {
            InitializeComponent();
            comboBox_tipo_aggiungi.Items.AddRange(new string[] { "utente", "admin" });
            comboBox_tipo_aggiungi.SelectedIndex = 0;
        }

        // Costruttore MODIFICA (precompila i campi)
        public FormAggiungiUtente(Utente u)
        {
            InitializeComponent();
            comboBox_tipo_aggiungi.Items.AddRange(new string[] { "utente", "admin" });

            // Precompila con i dati esistenti
            textBox_email_aggiungi.Text = u.Email;
            textBox_nome_aggiungi.Text = u.Nome;
            textBox_cognome_aggiungi.Text = u.Cognome;
            textBox_password_aggiungi.Text = "";
            dateTimePicker_data_aggiungi.Value = u.DataNascita ?? DateTime.Today;
            comboBox_tipo_aggiungi.SelectedItem = u.Tipo;

            // Cambia titolo e modalità
            modalitaModifica = true;
            this.Text = "Modifica Utente";
            button1.Text = "Salva Modifiche";
        }

        // AGGIUNGI / SALVA
        private void button1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(EmailInserita) ||
                string.IsNullOrEmpty(NomeInserito) ||
                string.IsNullOrEmpty(CognomeInserito))
            {
                MessageBox.Show("Compila tutti i campi obbligatori.", "Attenzione",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // In modifica la password può essere vuota (= non cambiarla)
            if (!modalitaModifica && string.IsNullOrEmpty(PasswordInserita))
            {
                MessageBox.Show("Inserisci una password.", "Attenzione",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        // ANNULLA
        private void button2_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}