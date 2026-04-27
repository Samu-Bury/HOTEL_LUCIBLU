using System;
using System.Windows.Forms;

namespace HOTEL_LUCIBLU
{
    public partial class FormAggiungiUtente : Form
    {
        // Proprietà pubbliche per recuperare i dati dalla Form1
        public string EmailInserita => textBox_email_aggiungi.Text.Trim();
        public string PasswordInserita => textBox_password_aggiungi.Text;
        public string NomeInserito => textBox_nome_aggiungi.Text.Trim();
        public string CognomeInserito => textBox_cognome_aggiungi.Text.Trim();
        public DateTime DataNascitaInserita => dateTimePicker_data_aggiungi.Value;
        public string TipoInserito => comboBox_tipo_aggiungi.SelectedItem?.ToString() ?? "utente";

        public FormAggiungiUtente()
        {
            InitializeComponent();
            comboBox_tipo_aggiungi.Items.AddRange(new string[] { "utente", "admin" });
            comboBox_tipo_aggiungi.SelectedIndex = 0;
        }


        //AGGIUNGI
        private void button1_Click(object sender, EventArgs e)
        {
            // Validazione campi
            if (string.IsNullOrEmpty(EmailInserita) ||
                string.IsNullOrEmpty(PasswordInserita) ||
                string.IsNullOrEmpty(NomeInserito) ||
                string.IsNullOrEmpty(CognomeInserito))
            {
                MessageBox.Show("Compila tutti i campi obbligatori.", "Attenzione",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}