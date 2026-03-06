using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HOTEL_LUCIBLU
{
    class Camera
    {
        public int IdCamera { get; set; }
        public int Numero { get; set; }
        public int Piano { get; set; }
        public string Tipo { get; set; }        // "Singola", "Doppia", "Suite"
        public int NumeroLetti { get; set; }
        public int NumeroBagni { get; set; }
        public double PrezzoNotte { get; set; }
        public string Stato { get; set; }       // "Libera", "Occupata", "Manutenzione"
        public string Descrizione { get; set; }
        public string Servizi { get; set; }     // es. "WiFi,TV"
        public string FotoPath { get; set; }    //foto camera
    }
}
