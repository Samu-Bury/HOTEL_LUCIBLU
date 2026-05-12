using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;

namespace HOTEL_LUCIBLU
{
    public class Admin : Utente
    {
        private static readonly string ConnString =
            "Server=sql8.freesqldatabase.com;Port=3306;Database=sql8826545;Uid=sql8826545;Pwd=q2CdMiKtt9;";

        #region Gestione Camere

        public void AggiungiCamera(Camera c) => c.Salva();
        public void ModificaCamera(Camera c) => c.Aggiorna();
        public void EliminaCamera(int numero) => Camera.Elimina(numero);

        public List<Camera> GetTutteLeCamere() => Camera.GetTutte();

        #endregion

        #region Gestione Utenti

        public List<Utente> GetTuttiGliUtenti()
        {
            var lista = new List<Utente>();
            using var conn = new MySqlConnection(ConnString);
            conn.Open();
            using var cmd = new MySqlCommand(
                "SELECT email,nome,cognome,dataDiNascita,dataDiCreazione,tipo FROM utenti", conn);
            using var r = cmd.ExecuteReader();
            while (r.Read())
                lista.Add(new Utente
                {
                    Email = r["email"].ToString(),
                    Nome = r["nome"].ToString(),
                    Cognome = r["cognome"].ToString(),
                    DataNascita = r["dataDiNascita"] == DBNull.Value ? null : Convert.ToDateTime(r["dataDiNascita"]),
                    DataCreazione = Convert.ToDateTime(r["dataDiCreazione"]),
                    Tipo = r["tipo"].ToString()
                });
            return lista;
        }

        public bool AggiornaTipoUtente(string email, string nuovoTipo)
        {
            using var conn = new MySqlConnection(ConnString);
            conn.Open();
            using var cmd = new MySqlCommand(
                "UPDATE utenti SET tipo=@t WHERE email=@e", conn);
            cmd.Parameters.AddWithValue("@t", nuovoTipo);
            cmd.Parameters.AddWithValue("@e", email);
            return cmd.ExecuteNonQuery() > 0;
        }

        #endregion

        #region Gestione Prenotazioni

        public List<Prenotazione> GetTutteLePrenotazioni() =>
            Prenotazione.GetTutte();

        public bool CancellaPrenotazione(int codice) =>
            Prenotazione.Cancella(codice);

        public bool EliminaPrenotazione(int codice) =>
            Prenotazione.EliminaFisicamente(codice);

        #endregion
    }
}