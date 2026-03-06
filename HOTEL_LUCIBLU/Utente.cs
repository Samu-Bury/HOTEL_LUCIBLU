using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HOTEL_LUCIBLU
{
    class Utente
    {
        //Attributi e Proprietà
        public int IdUtente { get; set; }
        public string Nome { get; set; }
        public string Cognome { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Telefono { get; set; }
        public DateTime DataNascita { get; set; }
        public string Indirizzo { get; set; }
        public DateTime DataRegistrazione { get; set; }
        public bool IsAdmin { get; set; }
    }
}
