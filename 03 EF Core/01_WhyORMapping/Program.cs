// *************************************************************************************************
// ACHTUNG: DIESER CODE KANN IHRE GESUNDHEIT GEFÄHRDEN!!!
// Vorher die Testdatenbank in den Ausgabeordner kopieren
// *************************************************************************************************

using System;
using System.Data.SQLite;   // NuGet: Install-Package System.Data.SQLite.Core
namespace DbDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            string nameBeginsWith = "A";
            SQLiteConnection conn = new SQLiteConnection("DataSource=Tests.db");
            conn.Open();

            // Einladung für SQL Injections...
            SQLiteCommand command = new SQLiteCommand("SELECT * FROM Pupil WHERE P_Lastname LIKE '" + nameBeginsWith + "%'", conn);
            SQLiteDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                // Wir casten auf "gut Glück", denn alles ist ein object.
                long id = (long)reader["P_ID"];
                string lastname = reader["P_Lastname"] == DBNull.Value ? null : (string)reader["P_Lastname"];
                Console.WriteLine($"{id}: {lastname}");
            }

            // Wer schließt reader und conn?? Es wurde kein using verwendet.
        }
    }
}
