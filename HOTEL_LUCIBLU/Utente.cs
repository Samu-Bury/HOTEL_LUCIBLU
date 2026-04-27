using System;

namespace HOTEL_LUCIBLU
{
    #region Classe Utente
    public class Utente
    {
        public string Email { get; set; }
        public string Nome { get; set; }
        public string Cognome { get; set; }
        public DateTime? DataNascita { get; set; }      // nullable perché può essere vuota nel DB
        public DateTime DataCreazione { get; set; }     // rinominata per corrispondere al DB e a DatabaseHelper
        public string Tipo { get; set; }                // "utente" oppure "admin" — rinominato da Ruolo
    }
    #endregion
}