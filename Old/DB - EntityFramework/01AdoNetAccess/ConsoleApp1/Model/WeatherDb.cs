using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirstDbApp.Model
{

    public class WeatherDb : IDisposable
    {
        readonly SqlConnection connection;
        readonly string connectionString;
        public WeatherDb(string connectionString)
        {
            this.connectionString = connectionString;
            connection = new SqlConnection(connectionString);
            connection.Open();
        }

        public void Dispose()
        {
            // Zur Demonstration, damit wir sehen, wann Dispose aufgerufen wird.
            Console.WriteLine("Dispose called.");
            connection.Close();
        }

        /// <summary>
        /// Liefert alle eingetragenen Stationen in der Datenbank.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Station> GetStations()
        {
            // SELECT * FROM Station
            SqlCommand command = new SqlCommand("SELECT * FROM Station", connection);
            using (SqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    yield return new Station
                    {
                        S_ID = (int)reader["S_ID"],
                        S_Location = (string)reader["S_Location"],
                        S_Height = reader["S_Height"] == System.DBNull.Value ? null : (int?)reader["S_Height"],
                    };
                }
            }
        }

        /// <summary>
        /// Liefert die Messwerte einer Station.
        /// </summary>
        /// <param name="id">Stations ID (Fremdschlüssel)</param>
        /// <returns></returns>
        public IEnumerable<Measurement> GetMeasurements(int id)
        {
            // SELECT * FROM Measurement WHERE M_Station = id
            SqlCommand command = new SqlCommand("SELECT M_Date,M_Station,M_Temperature  FROM Measurement WHERE M_Station=@S_ID", connection);
            command.Parameters.Add(new SqlParameter("@S_ID", id));
            using (SqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    yield return new Measurement
                    {
                        M_Date = (DateTime)reader["M_Date"],
                        M_Station = (int)reader["M_Station"],
                        M_Temperature = reader["M_Temperature"] == System.DBNull.Value ? null : (decimal?)reader["M_Temperature"]
                    };
                }
            }
        }
    }
}
