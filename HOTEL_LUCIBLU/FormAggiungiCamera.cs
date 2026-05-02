using System;
using System.Windows.Forms;

namespace HOTEL_LUCIBLU
{
    public partial class FormAggiungiCamera : Form
    {
        public int NumeroCamera => (int)numericUpDown_numero.Value;
        public int PianoCamera => int.Parse(comboBox_piano.SelectedItem.ToString());
        public string TipoCamera => comboBox_tipo.SelectedItem?.ToString() ?? "";
        public decimal PrezzoCamera => numericUpDown_prezzoNotte.Value;
        public string StatoCamera => comboBox_stato.SelectedItem?.ToString() ?? "disponibile";
        public string Servizio1 => comboBox_servizio1.SelectedItem?.ToString() ?? "";
        public string Servizio2 => comboBox_servizio2.SelectedItem?.ToString() ?? "";

        private string[] servizi = {
            "",
            "Aria condizionata",
            "Riscaldamento",
            "Wi-Fi gratuito",
            "Cassaforte",
            "Vasca idromassaggio",
            "TV smart"
        };

        public FormAggiungiCamera()
        {
            InitializeComponent();

            comboBox_piano.Items.AddRange(new string[] { "1", "2", "3" });
            comboBox_piano.SelectedIndex = 0;

            comboBox_tipo.Items.AddRange(new string[] { "Singola", "Doppia", "Suite" });
            comboBox_tipo.SelectedIndex = 0;

            comboBox_stato.Items.AddRange(new string[] { "disponibile", "occupata", "manutenzione" });
            comboBox_stato.SelectedIndex = 0;

            comboBox_servizio1.Items.AddRange(servizi);
            comboBox_servizio2.Items.AddRange(servizi);

            comboBox_servizio1.SelectedIndex = 0;
            comboBox_servizio2.SelectedIndex = 0;

            numericUpDown_numero.Minimum = 1;
            numericUpDown_numero.Maximum = 9999;
            numericUpDown_prezzoNotte.Minimum = 0;
            numericUpDown_prezzoNotte.Maximum = 99999;
            numericUpDown_prezzoNotte.DecimalPlaces = 2;
        }

      
        //AGGIUNGI
        private void button1_Click_1(object sender, EventArgs e)
        {
            if (comboBox_tipo.SelectedItem == null || comboBox_piano.SelectedItem == null)
            {
                MessageBox.Show("Compila tutti i campi obbligatori.", "Attenzione",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        //ANNULLA
        private void button2_Click_1(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}