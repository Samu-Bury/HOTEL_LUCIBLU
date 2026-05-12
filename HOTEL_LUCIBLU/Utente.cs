using System;
using MySql.Data.MySqlClient;

namespace HOTEL_LUCIBLU
{
    public class Utente
    {
        #region Proprietà
        public string Email { get; set; }
        public string Nome { get; set; }
        public string Cognome { get; set; }
        public DateTime? DataNascita { get; set; }
        public DateTime DataCreazione { get; set; }
        public string Tipo { get; set; }
        #endregion

        private static readonly string ConnString =
            "Server=sql8.freesqldatabase.com;Port=3306;Database=sql8826545;Uid=sql8826545;Pwd=q2CdMiKtt9;";

        #region Metodi statici

        public static bool Login(string email, string password, out string nome, out string tipo)
        {
            nome = ""; tipo = "";
            using var conn = new MySqlConnection(ConnString);
            conn.Open();
            using var cmd = new MySqlCommand(
                "SELECT nome, tipo FROM utenti WHERE email=@e AND password=@p", conn);
            cmd.Parameters.AddWithValue("@e", email);
            cmd.Parameters.AddWithValue("@p", HashPassword(password));
            using var r = cmd.ExecuteReader();
            if (!r.Read()) return false;
            nome = r["nome"].ToString();
            tipo = r["tipo"].ToString();
            return true;
        }

        public static bool Registra(string email, string password, string nome,
            string cognome, DateTime dataNascita, string tipo = "utente")
        {
            if (EmailEsiste(email)) return false;
            using var conn = new MySqlConnection(ConnString);
            conn.Open();
            using var cmd = new MySqlCommand(
                @"INSERT INTO utenti (email,password,nome,cognome,dataDiNascita,tipo)
                  VALUES (@e,@p,@n,@c,@d,@t)", conn);
            cmd.Parameters.AddWithValue("@e", email);
            cmd.Parameters.AddWithValue("@p", HashPassword(password));
            cmd.Parameters.AddWithValue("@n", nome);
            cmd.Parameters.AddWithValue("@c", cognome);
            cmd.Parameters.AddWithValue("@d", dataNascita);
            cmd.Parameters.AddWithValue("@t", tipo);
            cmd.ExecuteNonQuery();
            return true;
        }

        public static bool EmailEsiste(string email)
        {
            using var conn = new MySqlConnection(ConnString);
            conn.Open();
            using var cmd = new MySqlCommand(
                "SELECT COUNT(*) FROM utenti WHERE email=@e", conn);
            cmd.Parameters.AddWithValue("@e", email);
            return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
        }

        public static Utente GetUtente(string email)
        {
            using var conn = new MySqlConnection(ConnString);
            conn.Open();
            using var cmd = new MySqlCommand(
                "SELECT email,nome,cognome,dataDiNascita,dataDiCreazione,tipo FROM utenti WHERE email=@e", conn);
            cmd.Parameters.AddWithValue("@e", email);
            using var r = cmd.ExecuteReader();
            if (!r.Read()) return null;
            return new Utente
            {
                Email = r["email"].ToString(),
                Nome = r["nome"].ToString(),
                Cognome = r["cognome"].ToString(),
                DataNascita = r["dataDiNascita"] == DBNull.Value ? null : Convert.ToDateTime(r["dataDiNascita"]),
                DataCreazione = Convert.ToDateTime(r["dataDiCreazione"]),
                Tipo = r["tipo"].ToString()
            };
        }

        #endregion

        #region Metodi di istanza

        // Salva le modifiche dell'utente corrente sul DB
        public bool Modifica(string nuovaEmail, string nuovaPassword = "")
        {
            using var conn = new MySqlConnection(ConnString);
            conn.Open();

            bool cambiaPassword = !string.IsNullOrEmpty(nuovaPassword);
            string set = "email=@e, nome=@n, cognome=@c, dataDiNascita=@d";
            if (cambiaPassword) set += ", password=@p";

            using var cmd = new MySqlCommand(
                $"UPDATE utenti SET {set} WHERE email=@orig", conn);
            cmd.Parameters.AddWithValue("@orig", Email);
            cmd.Parameters.AddWithValue("@e", nuovaEmail);
            cmd.Parameters.AddWithValue("@n", Nome);
            cmd.Parameters.AddWithValue("@c", Cognome);
            cmd.Parameters.AddWithValue("@d", DataNascita.HasValue ? (object)DataNascita.Value : DBNull.Value);
            if (cambiaPassword)
                cmd.Parameters.AddWithValue("@p", HashPassword(nuovaPassword));

            bool ok = cmd.ExecuteNonQuery() > 0;
            if (ok) Email = nuovaEmail; // aggiorna l'istanza
            return ok;
        }

        public bool Elimina()
        {
            using var conn = new MySqlConnection(ConnString);
            conn.Open();
            using var cmd = new MySqlCommand("DELETE FROM utenti WHERE email=@e", conn);
            cmd.Parameters.AddWithValue("@e", Email);
            return cmd.ExecuteNonQuery() > 0;
        }

        #endregion

        protected static string HashPassword(string password)
        {
            using var sha = System.Security.Cryptography.SHA256.Create();
            byte[] bytes = sha.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            return BitConverter.ToString(bytes).Replace("-", "").ToLower();
        }
    }
}