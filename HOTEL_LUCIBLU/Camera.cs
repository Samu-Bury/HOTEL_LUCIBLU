using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;

namespace HOTEL_LUCIBLU
{
    public class Camera
    {
        #region Proprietà
        public int Numero { get; set; }
        public int Piano { get; set; }
        public string Tipo { get; set; }
        public decimal PrezzoNotte { get; set; }
        public string Stato { get; set; }
        public string Servizio1 { get; set; }
        public string Servizio2 { get; set; }
        #endregion

        private static readonly string ConnString =
            "Server=sql8.freesqldatabase.com;Port=3306;Database=sql8826545;Uid=sql8826545;Pwd=q2CdMiKtt9;";

        // Inserisce questa camera nel DB
        public void Salva()
        {
            using var conn = new MySqlConnection(ConnString);
            conn.Open();
            using var cmd = new MySqlCommand(
                @"INSERT INTO camere (numero,piano,tipo,prezzoNotte,stato,servizio1,servizio2)
                  VALUES (@num,@pi,@ti,@pr,@st,@s1,@s2)", conn);
            BindParams(cmd);
            cmd.ExecuteNonQuery();
        }

        // Aggiorna questa camera nel DB
        public void Aggiorna()
        {
            using var conn = new MySqlConnection(ConnString);
            conn.Open();
            using var cmd = new MySqlCommand(
                @"UPDATE camere SET piano=@pi,tipo=@ti,prezzoNotte=@pr,
                  stato=@st,servizio1=@s1,servizio2=@s2 WHERE numero=@num", conn);
            BindParams(cmd);
            cmd.ExecuteNonQuery();
        }

        // Elimina una camera per numero
        public static bool Elimina(int numero)
        {
            using var conn = new MySqlConnection(ConnString);
            conn.Open();
            using var cmd = new MySqlCommand(
                "DELETE FROM camere WHERE numero=@n", conn);
            cmd.Parameters.AddWithValue("@n", numero);
            return cmd.ExecuteNonQuery() > 0;
        }

        public static bool Esiste(int numero)
        {
            using var conn = new MySqlConnection(ConnString);
            conn.Open();
            using var cmd = new MySqlCommand(
                "SELECT COUNT(*) FROM camere WHERE numero=@n", conn);
            cmd.Parameters.AddWithValue("@n", numero);
            return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
        }

        public static List<Camera> GetTutte()
        {
            var lista = new List<Camera>();
            using var conn = new MySqlConnection(ConnString);
            conn.Open();
            using var cmd = new MySqlCommand(
                "SELECT numero,piano,tipo,prezzoNotte,stato,servizio1,servizio2 FROM camere", conn);
            using var r = cmd.ExecuteReader();
            while (r.Read())
                lista.Add(DaReader(r));
            return lista;
        }

        // Costruisce una Camera da un DataReader
        public static Camera DaReader(MySqlDataReader r) => new Camera
        {
            Numero = Convert.ToInt32(r["numero"]),
            Piano = Convert.ToInt32(r["piano"]),
            Tipo = r["tipo"].ToString(),
            PrezzoNotte = Convert.ToDecimal(r["prezzoNotte"]),
            Stato = r["stato"].ToString(),
            Servizio1 = r["servizio1"] == DBNull.Value ? "" : r["servizio1"].ToString(),
            Servizio2 = r["servizio2"] == DBNull.Value ? "" : r["servizio2"].ToString()
        };

        private void BindParams(MySqlCommand cmd)
        {
            cmd.Parameters.AddWithValue("@num", Numero);
            cmd.Parameters.AddWithValue("@pi", Piano);
            cmd.Parameters.AddWithValue("@ti", Tipo);
            cmd.Parameters.AddWithValue("@pr", PrezzoNotte);
            cmd.Parameters.AddWithValue("@st", Stato);
            cmd.Parameters.AddWithValue("@s1", string.IsNullOrEmpty(Servizio1) ? (object)DBNull.Value : Servizio1);
            cmd.Parameters.AddWithValue("@s2", string.IsNullOrEmpty(Servizio2) ? (object)DBNull.Value : Servizio2);
        }

        // Aggiunge alla classe Camera:

        public static List<Camera> GetTutteConDisponibilita(DateTime checkIn, DateTime checkOut)
        {
            var lista = new List<Camera>();
            using var conn = new MySqlConnection(ConnString);
            conn.Open();

            // Recupera tutti i numeri camera che hanno prenotazioni attive
            using var cmdOccupate = new MySqlCommand(
                @"SELECT DISTINCT numCamera FROM prenotazioni
          WHERE stato != 'cancellata'
          AND dataCheckIn < @co
          AND dataCheckOut > @ci", conn);
            cmdOccupate.Parameters.AddWithValue("@ci", checkIn);
            cmdOccupate.Parameters.AddWithValue("@co", checkOut);

            var camereOccupate = new HashSet<int>();
            using (var r = cmdOccupate.ExecuteReader())
                while (r.Read())
                    camereOccupate.Add(Convert.ToInt32(r["numCamera"]));

            using var cmd = new MySqlCommand(
                "SELECT numero,piano,tipo,prezzoNotte,stato,servizio1,servizio2 FROM camere", conn);
            using var rCamere = cmd.ExecuteReader();
            while (rCamere.Read())
            {
                Camera c = DaReader(rCamere);
                // Sovrascrive lo stato con "occupata" se la camera ha prenotazioni
                if (camereOccupate.Contains(c.Numero))
                    c.Stato = "occupata";
                lista.Add(c);
            }
            return lista;
        }
    }
}