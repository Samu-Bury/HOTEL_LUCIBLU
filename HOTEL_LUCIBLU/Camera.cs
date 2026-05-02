using System;

namespace HOTEL_LUCIBLU
{
    #region Classe Camera
    public class Camera
    {
        public int Numero { get; set; }
        public int Piano { get; set; }            // 0=Terra, 1=Primo, 2=Secondo, 3=Terzo
        public string Tipo { get; set; }          // Singola, Doppia, Suite
        public decimal PrezzoNotte { get; set; }
        public string Stato { get; set; }         // Disponibile, Occupata, Manutenzione
        public string Servizio1 { get; set; }
        public string Servizio2 { get; set; }

    }
    #endregion
}