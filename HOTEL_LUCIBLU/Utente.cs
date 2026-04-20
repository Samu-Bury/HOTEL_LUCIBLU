using System;

namespace HOTEL_LUCIBLU
{
    #region Classe Utente
    public class Utente
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string Nome { get; set; }
        public string Cognome { get; set; }
        public DateTime DataNascita { get; set; }
        public DateTime DataRegistrazione { get; set; }
        public string Ruolo { get; set; }   // "utente" oppure "admin"


        public void Login()
        {

        }

        public void ModificaProfilo()
        {

        }

        public void CancellaPrenotazione(int ID_Prenotazione)
        {

        }






    }
    #endregion
}