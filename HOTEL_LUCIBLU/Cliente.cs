using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;

namespace HOTEL_LUCIBLU
{
    public class Cliente : Utente
    {
        private static readonly string ConnString =
            "Server=sql8.freesqldatabase.com;Port=3306;Database=sql8826545;Uid=sql8826545;Pwd=q2CdMiKtt9;";

        // Crea una nuova prenotazione per questo cliente
        public bool EffettuaPrenotazione(Prenotazione p)
        {
            using var conn = new MySqlConnection(ConnString);
            conn.Open();
            using var cmd = new MySqlCommand(
                @"INSERT INTO prenotazioni (email,numCamera,dataCheckIn,dataCheckOut,notti,prezzoTot,metPagamento,stato)
                  VALUES (@e,@cam,@ci,@co,@n,@tot,@met,'in attesa')", conn);
            cmd.Parameters.AddWithValue("@e", p.Email);
            cmd.Parameters.AddWithValue("@cam", p.NumeroCamera);
            cmd.Parameters.AddWithValue("@ci", p.DataCheckIn);
            cmd.Parameters.AddWithValue("@co", p.DataCheckOut);
            cmd.Parameters.AddWithValue("@n", p.Notti);
            cmd.Parameters.AddWithValue("@tot", p.PrezzoTotale);
            cmd.Parameters.AddWithValue("@met", p.MetodoPagamento);
            cmd.ExecuteNonQuery();
            return true;
        }

        // Legge le prenotazioni di questo cliente
        public List<Prenotazione> VisualizzaStorico()
        {
            var lista = new List<Prenotazione>();
            using var conn = new MySqlConnection(ConnString);
            conn.Open();
            using var cmd = new MySqlCommand(
                "SELECT * FROM prenotazioni WHERE email=@e ORDER BY dataPrenotazione DESC", conn);
            cmd.Parameters.AddWithValue("@e", Email);
            using var r = cmd.ExecuteReader();
            while (r.Read())
                lista.Add(Prenotazione.DaReader(r));
            return lista;
        }

        // Annulla una prenotazione "in attesa"
        public bool AnnullaPrenotazione(int codice)
        {
            using var conn = new MySqlConnection(ConnString);
            conn.Open();
            using var cmd = new MySqlCommand(
                "UPDATE prenotazioni SET stato='cancellata' WHERE codicePrenotazione=@cod AND email=@e", conn);
            cmd.Parameters.AddWithValue("@cod", codice);
            cmd.Parameters.AddWithValue("@e", Email);
            return cmd.ExecuteNonQuery() > 0;
        }
    }
}