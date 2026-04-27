using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;

namespace HOTEL_LUCIBLU  // ← deve essere lo stesso namespace di Form1.cs
{
    public class DatabaseHelper
    {
        private string connectionString = "Server=localhost;Database=luciblu_hotel;Uid=root;Pwd=root;";

        // LOGIN
        public bool Login(string email, string password, out string nome, out string tipo)
        {
            nome = "";
            tipo = "";

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT nome, tipo FROM utenti WHERE email = @email AND password = @password";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@email", email);
                    cmd.Parameters.AddWithValue("@password", HashPassword(password));

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            nome = reader["nome"].ToString();
                            tipo = reader["tipo"].ToString();
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        // REGISTRAZIONE
        public bool Registra(string email, string password, string nome, string cognome, DateTime dataNascita)
        {
            if (EmailEsiste(email)) return false;

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string query = @"INSERT INTO utenti (email, password, nome, cognome, dataDiNascita) 
                                 VALUES (@email, @password, @nome, @cognome, @data)";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@email", email);
                    cmd.Parameters.AddWithValue("@password", HashPassword(password));
                    cmd.Parameters.AddWithValue("@nome", nome);
                    cmd.Parameters.AddWithValue("@cognome", cognome);
                    cmd.Parameters.AddWithValue("@data", dataNascita);
                    cmd.ExecuteNonQuery();
                }
            }
            return true;
        }

        // CONTROLLA EMAIL
        public bool EmailEsiste(string email)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT COUNT(*) FROM utenti WHERE email = @email";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@email", email);
                    return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
                }
            }
        }

        // HASH PASSWORD SHA256
        private string HashPassword(string password)
        {
            using (var sha = System.Security.Cryptography.SHA256.Create())
            {
                byte[] bytes = sha.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return BitConverter.ToString(bytes).Replace("-", "").ToLower();
            }
        }

        // OTTIENI TUTTI GLI UTENTI (VISUALIZZAZIONE NELLA LISTVIEW DI ADMIN)
        public List<Utente> GetUtenti()
        {
            List<Utente> utenti = new List<Utente>();

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT email, nome, cognome, dataDiNascita, dataDiCreazione, tipo FROM utenti";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        utenti.Add(new Utente
                        {
                            Email = reader["email"].ToString(),
                            Nome = reader["nome"].ToString(),
                            Cognome = reader["cognome"].ToString(),
                            DataNascita = reader["dataDiNascita"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["dataDiNascita"]),
                            DataCreazione = Convert.ToDateTime(reader["dataDiCreazione"]),
                            Tipo = reader["tipo"].ToString()
                        });
                    }
                }
            }
            return utenti;
        }

        // ELIMINA UTENTE
        public bool EliminaUtente(string email)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string query = "DELETE FROM utenti WHERE email = @email";
                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@email", email);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }



    }
}