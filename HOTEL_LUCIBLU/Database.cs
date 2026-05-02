using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;

namespace HOTEL_LUCIBLU
{
    public class DatabaseHelper
    {
        private string connectionString = "Server=sql7.freesqldatabase.com;Port=3306;Database=sql7824799;Uid=sql7824799;Pwd=Nb5nrBwXVC;";

        #region GESTIONE ACCESSO E I VARI UTENTI 


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
        public bool Registra(string email, string password, string nome, string cognome, DateTime dataNascita, string tipo = "utente")
        {
            if (EmailEsiste(email)) return false;

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string query = @"INSERT INTO utenti (email, password, nome, cognome, dataDiNascita, tipo) 
                 VALUES (@email, @password, @nome, @cognome, @data, @tipo)";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@email", email);
                    cmd.Parameters.AddWithValue("@password", HashPassword(password));
                    cmd.Parameters.AddWithValue("@nome", nome);
                    cmd.Parameters.AddWithValue("@cognome", cognome);
                    cmd.Parameters.AddWithValue("@data", dataNascita);
                    cmd.Parameters.AddWithValue("@tipo", tipo);
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

        // OTTIENI SINGOLO UTENTE
        public Utente GetUtente(string email)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT email, nome, cognome, dataDiNascita, dataDiCreazione, tipo FROM utenti WHERE email = @email";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@email", email);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Utente
                            {
                                Email = reader["email"].ToString(),
                                Nome = reader["nome"].ToString(),
                                Cognome = reader["cognome"].ToString(),
                                DataNascita = reader["dataDiNascita"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["dataDiNascita"]),
                                DataCreazione = Convert.ToDateTime(reader["dataDiCreazione"]),
                                Tipo = reader["tipo"].ToString()
                            };
                        }
                    }
                }
            }
            return null;
        }

        // MODIFICA UTENTE
        public bool ModificaUtente(string emailOriginale, string nuovaEmail, string nome, string cognome, DateTime? dataNascita, string nuovaPassword = "")
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();

                string query;

                if (!string.IsNullOrEmpty(nuovaPassword))
                {
                    query = @"UPDATE utenti SET email=@email, nome=@nome, cognome=@cognome, 
                      dataDiNascita=@data, password=@password WHERE email=@emailOriginale";
                }
                else
                {
                    query = @"UPDATE utenti SET email=@email, nome=@nome, cognome=@cognome, 
                      dataDiNascita=@data WHERE email=@emailOriginale";
                }

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@emailOriginale", emailOriginale);
                    cmd.Parameters.AddWithValue("@email", nuovaEmail);
                    cmd.Parameters.AddWithValue("@nome", nome);
                    cmd.Parameters.AddWithValue("@cognome", cognome);
                    cmd.Parameters.AddWithValue("@data", dataNascita.HasValue ? (object)dataNascita.Value : DBNull.Value);

                    if (!string.IsNullOrEmpty(nuovaPassword))
                        cmd.Parameters.AddWithValue("@password", HashPassword(nuovaPassword));

                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        public bool ModificaUtente(string emailOriginale, string nuovaEmail, string nome,
    string cognome, DateTime? dataNascita, string nuovaPassword = "", string tipo = "")
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();

                string query;

                bool cambiaPassword = !string.IsNullOrEmpty(nuovaPassword);
                bool cambiaTipo = !string.IsNullOrEmpty(tipo);

                if (cambiaPassword && cambiaTipo)
                    query = @"UPDATE utenti SET email=@email, nome=@nome, cognome=@cognome, 
                      dataDiNascita=@data, password=@password, tipo=@tipo 
                      WHERE email=@emailOriginale";
                else if (cambiaPassword)
                    query = @"UPDATE utenti SET email=@email, nome=@nome, cognome=@cognome, 
                      dataDiNascita=@data, password=@password 
                      WHERE email=@emailOriginale";
                else if (cambiaTipo)
                    query = @"UPDATE utenti SET email=@email, nome=@nome, cognome=@cognome, 
                      dataDiNascita=@data, tipo=@tipo 
                      WHERE email=@emailOriginale";
                else
                    query = @"UPDATE utenti SET email=@email, nome=@nome, cognome=@cognome, 
                      dataDiNascita=@data 
                      WHERE email=@emailOriginale";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@emailOriginale", emailOriginale);
                    cmd.Parameters.AddWithValue("@email", nuovaEmail);
                    cmd.Parameters.AddWithValue("@nome", nome);
                    cmd.Parameters.AddWithValue("@cognome", cognome);
                    cmd.Parameters.AddWithValue("@data", dataNascita.HasValue ? (object)dataNascita.Value : DBNull.Value);

                    if (cambiaPassword)
                        cmd.Parameters.AddWithValue("@password", HashPassword(nuovaPassword));
                    if (cambiaTipo)
                        cmd.Parameters.AddWithValue("@tipo", tipo);

                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        #endregion

        #region GESTIONE CAMERE
        // OTTIENI TUTTE LE CAMERE
        public List<Camera> GetCamere()
        {
            List<Camera> camere = new List<Camera>();

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT numero, piano, tipo, prezzoNotte, stato, servizio1, servizio2 FROM camere";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        camere.Add(new Camera
                        {
                            Numero = Convert.ToInt32(reader["numero"]),
                            Piano = Convert.ToInt32(reader["piano"]),
                            Tipo = reader["tipo"].ToString(),
                            PrezzoNotte = Convert.ToDecimal(reader["prezzoNotte"]),
                            Stato = reader["stato"].ToString(),
                            Servizio1 = reader["servizio1"] == DBNull.Value ? "" : reader["servizio1"].ToString(),
                            Servizio2 = reader["servizio2"] == DBNull.Value ? "" : reader["servizio2"].ToString()
                        });
                    }
                }
            }
            return camere;
        }

        // AGGIUNGI CAMERA
        public bool AggiungiCamera(Camera c)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string query = @"INSERT INTO camere (numero, piano, tipo, prezzoNotte, stato, servizio1, servizio2)
                         VALUES (@numero, @piano, @tipo, @prezzo, @stato, @s1, @s2)";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@numero", c.Numero);
                    cmd.Parameters.AddWithValue("@piano", c.Piano);
                    cmd.Parameters.AddWithValue("@tipo", c.Tipo);
                    cmd.Parameters.AddWithValue("@prezzo", c.PrezzoNotte);
                    cmd.Parameters.AddWithValue("@stato", c.Stato);
                    cmd.Parameters.AddWithValue("@s1", string.IsNullOrEmpty(c.Servizio1) ? (object)DBNull.Value : c.Servizio1);
                    cmd.Parameters.AddWithValue("@s2", string.IsNullOrEmpty(c.Servizio2) ? (object)DBNull.Value : c.Servizio2);
                    cmd.ExecuteNonQuery();
                }
            }
            return true;
        }

        // ELIMINA CAMERA
        public bool EliminaCamera(int numero)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string query = "DELETE FROM camere WHERE numero = @numero";
                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@numero", numero);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        // CONTROLLA SE NUMERO CAMERA ESISTE GIA
        public bool CameraEsiste(int numero)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT COUNT(*) FROM camere WHERE numero = @numero";
                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@numero", numero);
                    return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
                }
            }
        }

        #endregion

        #region GESTIONE PRENOTAZIONI

        // AGGIUNGI PRENOTAZIONE
        public bool AggiungiPrenotazione(Prenotazione p)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string query = @"INSERT INTO prenotazioni (email, numCamera, dataCheckIn, dataCheckOut, notti, prezzoTot, metPagamento, stato)
                         VALUES (@email, @camera, @checkin, @checkout, @notti, @prezzo, @metodo, 'in attesa')";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@email", p.Email);
                    cmd.Parameters.AddWithValue("@camera", p.NumeroCamera);
                    cmd.Parameters.AddWithValue("@checkin", p.DataCheckIn);
                    cmd.Parameters.AddWithValue("@checkout", p.DataCheckOut);
                    cmd.Parameters.AddWithValue("@notti", p.Notti);
                    cmd.Parameters.AddWithValue("@prezzo", p.PrezzoTotale);
                    cmd.Parameters.AddWithValue("@metodo", p.MetodoPagamento);
                    cmd.ExecuteNonQuery();
                }
            }
            return true;
        }

        // OTTIENI PRENOTAZIONI UTENTE
        public List<Prenotazione> GetPrenotazioniUtente(string email)
        {
            List<Prenotazione> lista = new List<Prenotazione>();

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT * FROM prenotazioni WHERE email = @email ORDER BY dataPrenotazione DESC";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@email", email);
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            DateTime checkIn = Convert.ToDateTime(reader["dataCheckIn"]);
                            DateTime checkOut = Convert.ToDateTime(reader["dataCheckOut"]);
                            string statoDb = reader["stato"].ToString();

                            lista.Add(new Prenotazione
                            {
                                CodicePrenotazione = reader["codicePrenotazione"].ToString(),
                                Email = reader["email"].ToString(),
                                NumeroCamera = reader["numCamera"].ToString(),
                                DataCheckIn = checkIn,
                                DataCheckOut = checkOut,
                                PrezzoTotale = Convert.ToDecimal(reader["prezzoTot"]),
                                MetodoPagamento = reader["metPagamento"].ToString(),
                                DataPrenotazione = Convert.ToDateTime(reader["dataPrenotazione"]),
                                Stato = CalcolaStato(checkIn, checkOut, statoDb)
                            });
                        }
                    }
                }
            }
            return lista;
        }

        private string CalcolaStato(DateTime checkIn, DateTime checkOut, string statoDb)
        {
            // Se è stata cancellata manualmente, mantieni cancellata
            if (statoDb == "cancellata") return "cancellata";

            DateTime oggi = DateTime.Today;

            if (oggi < checkIn)
                return "in attesa";
            else if (oggi >= checkIn && oggi <= checkOut)
                return "confermata";
            else
                return "conclusa";
        }

        // OTTIENI TUTTE LE PRENOTAZIONI (ADMIN)
        public List<Prenotazione> GetTuttePrenotazioni()
        {
            List<Prenotazione> lista = new List<Prenotazione>();

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT * FROM prenotazioni ORDER BY dataPrenotazione DESC";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        DateTime checkIn = Convert.ToDateTime(reader["dataCheckIn"]);
                        DateTime checkOut = Convert.ToDateTime(reader["dataCheckOut"]);
                        string statoDb = reader["stato"].ToString();

                        lista.Add(new Prenotazione
                        {
                            CodicePrenotazione = reader["codicePrenotazione"].ToString(),
                            Email = reader["email"].ToString(),
                            NumeroCamera = reader["numCamera"].ToString(),
                            DataCheckIn = checkIn,
                            DataCheckOut = checkOut,
                            PrezzoTotale = Convert.ToDecimal(reader["prezzoTot"]),
                            MetodoPagamento = reader["metPagamento"].ToString(),
                            DataPrenotazione = Convert.ToDateTime(reader["dataPrenotazione"]),
                            Stato = CalcolaStato(checkIn, checkOut, statoDb)
                        });
                    }
                }
            }
            return lista;
        }

        // CANCELLA PRENOTAZIONE
        public bool CancellaPrenotazione(int codice)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string query = "UPDATE prenotazioni SET stato='cancellata' WHERE codicePrenotazione = @codice";
                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@codice", codice);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        // ELIMINA PRENOTAZIONE (rimozione fisica)
        public bool EliminaPrenotazione(int codice)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string query = "DELETE FROM prenotazioni WHERE codicePrenotazione = @codice";
                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@codice", codice);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        #endregion


    }
}