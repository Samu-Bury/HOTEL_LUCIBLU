using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HOTEL_LUCIBLU
{
    class Prenotazione
    {
        public int IdPrenotazione { get; set; }
        public string CodicePrenotazione { get; set; }   // es. "LB-2025-0847"
        public int IdUtente { get; set; }
        public int IdCamera { get; set; }
        public DateTime DataCheckIn { get; set; }
        public DateTime DataCheckOut { get; set; }
        public double PrezzoTotale { get; set; }
        public string StatoPagamento { get; set; }  // "Pagato", "In attesa", "Annullato"
        public string MetodoPagamento { get; set; }
        public DateTime DataPrenotazione { get; set; }
        public string Stato { get; set; }           // "Attiva", "CheckIn", "CheckOut", "Cancellata"

        // Proprietà calcolata, non salvata nel DB
        public int NumeroNotti => (DataCheckOut - DataCheckIn).Days;
    }
}
