using System;

namespace HOTEL_LUCIBLU
{
    #region Classe Prenotazione
    public class Prenotazione
    {
        public string CodicePrenotazione { get; set; }
        public string Email { get; set; }
        public string NumeroCamera { get; set; }
        public DateTime DataCheckIn { get; set; }
        public DateTime DataCheckOut { get; set; }
        public int Notti => (int)(DataCheckOut - DataCheckIn).TotalDays;
        public decimal PrezzoTotale { get; set; }
        public string MetodoPagamento { get; set; }
        public DateTime DataPrenotazione { get; set; }
        public string Stato { get; set; }   // Confermata, Cancellata, Completata


       
    }
    #endregion
}