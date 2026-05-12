using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;

namespace HOTEL_LUCIBLU
{
    public class Prenotazione
    {
        #region Proprietà
        public string CodicePrenotazione { get; set; }
        public string Email { get; set; }
        public string NumeroCamera { get; set; }
        public DateTime DataCheckIn { get; set; }
        public DateTime DataCheckOut { get; set; }
        public int Notti => (int)(DataCheckOut - DataCheckIn).TotalDays;
        public decimal PrezzoTotale { get; set; }
        public string MetodoPagamento { get; set; }
        public DateTime DataPrenotazione { get; set; }
        public string Stato { get; set; }
        #endregion

        private static readonly string ConnString =
            "Server=localhost;Database=luciblu_hotel;Uid=root;Pwd=root;";

        public static List<Prenotazione> GetTutte()
        {
            var lista = new List<Prenotazione>();
            using var conn = new MySqlConnection(ConnString);
            conn.Open();
            using var cmd = new MySqlCommand(
                "SELECT * FROM prenotazioni ORDER BY dataPrenotazione DESC", conn);
            using var r = cmd.ExecuteReader();
            while (r.Read()) lista.Add(DaReader(r));
            return lista;
        }

        public static bool Cancella(int codice)
        {
            using var conn = new MySqlConnection(ConnString);
            conn.Open();
            using var cmd = new MySqlCommand(
                "UPDATE prenotazioni SET stato='cancellata' WHERE codicePrenotazione=@c", conn);
            cmd.Parameters.AddWithValue("@c", codice);
            return cmd.ExecuteNonQuery() > 0;
        }

        public static bool EliminaFisicamente(int codice)
        {
            using var conn = new MySqlConnection(ConnString);
            conn.Open();
            using var cmd = new MySqlCommand(
                "DELETE FROM prenotazioni WHERE codicePrenotazione=@c", conn);
            cmd.Parameters.AddWithValue("@c", codice);
            return cmd.ExecuteNonQuery() > 0;
        }

        // Costruisce una Prenotazione da un DataReader con stato calcolato
        public static Prenotazione DaReader(MySqlDataReader r)
        {
            DateTime ci = Convert.ToDateTime(r["dataCheckIn"]);
            DateTime co = Convert.ToDateTime(r["dataCheckOut"]);
            string statoDb = r["stato"].ToString();
            return new Prenotazione
            {
                CodicePrenotazione = r["codicePrenotazione"].ToString(),
                Email = r["email"].ToString(),
                NumeroCamera = r["numCamera"].ToString(),
                DataCheckIn = ci,
                DataCheckOut = co,
                PrezzoTotale = Convert.ToDecimal(r["prezzoTot"]),
                MetodoPagamento = r["metPagamento"].ToString(),
                DataPrenotazione = Convert.ToDateTime(r["dataPrenotazione"]),
                Stato = CalcolaStato(ci, co, statoDb)
            };
        }

        private static string CalcolaStato(DateTime ci, DateTime co, string statoDb)
        {
            if (statoDb == "cancellata") return "cancellata";
            DateTime oggi = DateTime.Today;
            if (oggi < ci) return "in attesa";
            if (oggi >= ci && oggi <= co) return "confermata";
            return "conclusa";
        }
    }
}