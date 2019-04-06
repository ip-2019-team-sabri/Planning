using System;
using System.Data;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace IP_Planning_Database
{
    public class DBConnect
    {
        public void selectAll() //voorbeeld select all
        {
            string connStr = "server=localhost;user=root;database=planning;port=3306;password=maxime";
            MySqlConnection conn = new MySqlConnection(connStr);
            try
            {
                Console.WriteLine("Connecting to MySQL...");
                conn.Open();

                string sql = "SELECT id FROM test";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                MySqlDataReader rdr = cmd.ExecuteReader();
               
                while (rdr.Read())
                {
                    Console.WriteLine(rdr[0]);
                }

                rdr.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            conn.Close();
            Console.WriteLine("Done.");
        }

        public void update() // voorbeeld update
        {
            string connStr = "server=localhost;user=root;database=planning;port=3306;password=maxime";
            MySqlConnection conn = new MySqlConnection(connStr);
            try
            {
                Console.WriteLine("Connecting to MySQL...");
                conn.Open();

                string sql = "UPDATE test SET id = 3 WHERE id = 2";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            conn.Close();
            Console.WriteLine("Done.");
        }

        public void insertWerknemer(string werknemerUUID, string calendarUUID)
        {
            string connStr = "server=localhost;user=root;database=planning;port=3306;password=maxime";
            MySqlConnection conn = new MySqlConnection(connStr);
            try
            {
                Console.WriteLine("Connecting to MySQL...");
                conn.Open();

                /*
                string sql = string.Format("INSERT INTO werknemer (WerknemerUUID, CalendarUUID) VALUES (" + werknemerUUID + "," + calendarUUID + ")");
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                cmd.ExecuteNonQuery();
                */
                MySqlCommand cmd = new MySqlCommand();

                cmd.Connection = conn;
                cmd.CommandText = "INSERT INTO werknemer (WerknemerUUID, CalendarUUID) VALUES (@werknemerUUID, @calendarUUID)";
                cmd.Parameters.AddWithValue("@werknemerUUID", werknemerUUID);
                cmd.Parameters.AddWithValue("@calendarUUID", calendarUUID);

                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            conn.Close();
            Console.WriteLine("Done.");
        }

        public void deleteWerknemer(string werknemerUUID)
        {
            string connStr = "server=localhost;user=root;database=planning;port=3306;password=maxime";
            MySqlConnection conn = new MySqlConnection(connStr);
            try
            {
                Console.WriteLine("Connecting to MySQL...");
                conn.Open();
                /*
                string sql = "DELETE FROM werknemer WHERE WerknemerUUID = " + werknemerUUID;
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                cmd.ExecuteNonQuery();
                */

                MySqlCommand cmd = new MySqlCommand();

                cmd.Connection = conn;
                cmd.CommandText = "DELETE FROM werknemer WHERE WerknemerUUID = @werknemerUUID";
                cmd.Parameters.AddWithValue("@werknemerUUID", werknemerUUID);

                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            conn.Close();
            Console.WriteLine("Done.");
        }

        public void insertTaak(string taakUUID, string eventUUID, string werknemerUUID)
        {
            string connStr = "server=localhost;user=root;database=planning;port=3306;password=maxime";
            MySqlConnection conn = new MySqlConnection(connStr);
            try
            {
                Console.WriteLine("Connecting to MySQL...");
                conn.Open();

                MySqlCommand cmd = new MySqlCommand();

                cmd.Connection = conn;
                cmd.CommandText = "INSERT INTO taak (TaakUUID, EventUUID, WerknemerUUID) VALUES (@taakUUID, @eventUUID, @werknemerUUID)";
                cmd.Parameters.AddWithValue("@taakUUID", taakUUID);
                cmd.Parameters.AddWithValue("@eventUUID", eventUUID);
                cmd.Parameters.AddWithValue("@werknemerUUID", werknemerUUID);

                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            conn.Close();
            Console.WriteLine("Done.");
        }

        public void deleteTaak(string taakUUID)
        {
            string connStr = "server=localhost;user=root;database=planning;port=3306;password=maxime";
            MySqlConnection conn = new MySqlConnection(connStr);
            try
            {
                Console.WriteLine("Connecting to MySQL...");
                conn.Open();

                MySqlCommand cmd = new MySqlCommand();

                cmd.Connection = conn;
                cmd.CommandText = "DELETE FROM taak WHERE TaakUUID = @taakUUID";
                cmd.Parameters.AddWithValue("@taakUUID", taakUUID);

                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            conn.Close();
            Console.WriteLine("Done.");
        }

        public void insertLocatie(string locatieUUID, string calendarUUID)
        {
            string connStr = "server=localhost;user=root;database=planning;port=3306;password=maxime";
            MySqlConnection conn = new MySqlConnection(connStr);
            try
            {
                Console.WriteLine("Connecting to MySQL...");
                conn.Open();

                MySqlCommand cmd = new MySqlCommand();

                cmd.Connection = conn;
                cmd.CommandText = "INSERT INTO locatie (LocatieUUID, CalendarUUID) VALUES (@locatieUUID, @calendarUUID)";
                cmd.Parameters.AddWithValue("@locatieUUID", locatieUUID);
                cmd.Parameters.AddWithValue("@calendarUUID", calendarUUID);

                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            conn.Close();
            Console.WriteLine("Done.");
        }

        public void deleteLocatie(string locatieUUID)
        {
            string connStr = "server=localhost;user=root;database=planning;port=3306;password=maxime";
            MySqlConnection conn = new MySqlConnection(connStr);
            try
            {
                Console.WriteLine("Connecting to MySQL...");
                conn.Open();

                MySqlCommand cmd = new MySqlCommand();

                cmd.Connection = conn;
                cmd.CommandText = "DELETE FROM locatie WHERE LocatieUUID = @locatieUUID";
                cmd.Parameters.AddWithValue("@locatieUUID", locatieUUID);

                cmd.ExecuteNonQuery();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            conn.Close();
            Console.WriteLine("Done.");
        }

        public void insertSessie(string sessieUUID, string eventUUID, string locatieUUID)
        {
            string connStr = "server=localhost;user=root;database=planning;port=3306;password=maxime";
            MySqlConnection conn = new MySqlConnection(connStr);
            try
            {
                Console.WriteLine("Connecting to MySQL...");
                conn.Open();

                MySqlCommand cmd = new MySqlCommand();

                cmd.Connection = conn;
                cmd.CommandText = "INSERT INTO sessie (SessieUUID, EventUUID, LocatieUUID) VALUES (@sessieUUID, @eventUUID, @locatieUUID)";
                cmd.Parameters.AddWithValue("@sessieUUID", sessieUUID);
                cmd.Parameters.AddWithValue("@eventUUID", eventUUID);
                cmd.Parameters.AddWithValue("@locatieUUID", locatieUUID);

                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            conn.Close();
            Console.WriteLine("Done.");
        }

        public void deleteSessie(string sessieUUID)
        {
            string connStr = "server=localhost;user=root;database=planning;port=3306;password=maxime";
            MySqlConnection conn = new MySqlConnection(connStr);
            try
            {
                Console.WriteLine("Connecting to MySQL...");
                conn.Open();

                MySqlCommand cmd = new MySqlCommand();

                cmd.Connection = conn;
                cmd.CommandText = "DELETE FROM sessie WHERE SessieUUID = @sessieUUID";
                cmd.Parameters.AddWithValue("@sessieUUID", sessieUUID);

                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            conn.Close();
            Console.WriteLine("Done.");
        }

        public void insertEvent(string evenementUUID, string eventUUID)
        {
            string connStr = "server=localhost;user=root;database=planning;port=3306;password=maxime";
            MySqlConnection conn = new MySqlConnection(connStr);
            try
            {
                Console.WriteLine("Connecting to MySQL...");
                conn.Open();

                MySqlCommand cmd = new MySqlCommand();

                cmd.Connection = conn;
                cmd.CommandText = "INSERT INTO event (EvenementUUID, EventUUID) VALUES (@evenementUUID, @eventUUID)";
                cmd.Parameters.AddWithValue("@evenementUUID", evenementUUID);
                cmd.Parameters.AddWithValue("@eventUUID", eventUUID);

                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            conn.Close();
            Console.WriteLine("Done.");
        }

        public void deleteEvent(string evenementUUID)
        {
            string connStr = "server=localhost;user=root;database=planning;port=3306;password=maxime";
            MySqlConnection conn = new MySqlConnection(connStr);
            try
            {
                Console.WriteLine("Connecting to MySQL...");
                conn.Open();

                MySqlCommand cmd = new MySqlCommand();

                cmd.Connection = conn;
                cmd.CommandText = "DELETE FROM event WHERE EvenementUUID = @evenementUUID";
                cmd.Parameters.AddWithValue("@evenementUUID", evenementUUID);

                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            conn.Close();
            Console.WriteLine("Done.");
        }

        public static void count()
        {
            string connStr = "server=localhost;user=root;database=world;port=3306;password=******";
            MySqlConnection conn = new MySqlConnection(connStr);
            try
            {
                Console.WriteLine("Connecting to MySQL...");
                conn.Open();

                string sql = "SELECT COUNT(*) FROM Country";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                object result = cmd.ExecuteScalar();
                if (result != null)
                {
                    int r = Convert.ToInt32(result);
                    Console.WriteLine("Number of countries in the world database is: " + r);
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            conn.Close();
            Console.WriteLine("Done.");
        }
    }
}

