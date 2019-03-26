/* 
 * LocalDbLoader
 * Version 2.5.2017, Michael Schletz
 * Aufruf: localDbLoader sqlFile [dbFile]
 * Beispiel: localDbLoader test.sql [C:\Temp\mydb.mdf]
 * 
 * 1) Der Datenbankname wird aus dem Dateinamen gebildet (Pfad\Name.mdf)
 * 2) Die angegebene Datenbank wird - wenn angegeben - gelöscht.
 * 3) Die angegebene Datenbank wird - wenn angegeben - in der angegebenen Datei erstellt.
 * 4) Der Inhalt von test.sql wird in diese Datenbank eingespielt. Das USING wird automatisch
 *    nach dem Erstellen der Db abgesetzt.
 *    
 * Wird keine Datenbank angegeben, so wird der SQL Dump 1:1 ausgeführt. Durch das USING muss
 * dann aber die Datenbank im Skript gewählt werden.
 * GO Anweisungen werden nicht unterstützt.
 */

using System;
using System.Data.SqlClient;
using System.IO;
using System.Text.RegularExpressions;

namespace LocalDbLoader
{

    class Program
    {
        const string connStr = @"Data Source=(LocalDB)\MSSQLLocalDB; Integrated Security=True; MultipleActiveResultSets=True";
        static int Main(string[] args)
        {
            string dbFile = "", dbName = "";
            try
            {
                /* DB Filename wurde übergeben? Den Namen aus dem Pfad extrahieren. */
                if (args.Length >= 2)
                {
                    dbFile = args[1];
                    dbName = Path.GetFileNameWithoutExtension(dbFile);
                    if (dbName == "") { throw new Exception(@"dbFilename entspricht nicht dem Aufbau Pfad\Name.mdf"); }
                }
                if (args.Length >= 1)
                {
                    string commands = File.ReadAllText(args[0]);
                    int rowCount = 0;

                    using (SqlConnection conn = new SqlConnection(connStr))
                    {
                        conn.Open();
                        SqlCommand cmd = new SqlCommand("", conn);

                        if (dbName != "")
                        {
                            /* DROP Database liefert einen Fehler wenn sie noch nicht existiert. */
                            try
                            {
                                cmd.CommandText = "DROP DATABASE {dbName};".SetParam("dbName", dbName);
                                cmd.ExecuteNonQuery();
                                Console.WriteLine("Datenbank {0} gelöscht.", dbName);
                            }
                            catch { }

                            /* Die Datenbank mit dem extrahierten Namen in der angegebene Datei erstellen. */
                            cmd.CommandText = "CREATE DATABASE {dbName} ON (NAME= '{dbName}', FILENAME= '{dbFile}');"
                                .SetParam("dbName", dbName)
                                .SetParam("dbFile", dbFile);

                            cmd.ExecuteNonQuery();
                            Console.WriteLine("Datenbank {0} erstellt.", dbName);

                            cmd.CommandText = "USE {dbName}".SetParam("dbName", dbName);
                            cmd.ExecuteNonQuery();
                        }

                        cmd.CommandText = commands;
                        rowCount = cmd.ExecuteNonQuery();
                    }
                    Console.WriteLine("{0} Zeile(n) in {1} importiert.", rowCount, dbFile);
                }
                else
                {
                    throw new Exception(@"localDbLoader sqlFile [dbFile]");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                if (e.InnerException != null) Console.WriteLine(e.InnerException.Message);
                return 1;
            }
            return 0;
        }
    }
}

public static class MyExtensions
{
    public static string SetParam(this string str, string paramName, string paramVal)
    {
        return str.Replace("{" + paramName + "}", paramVal);
    }
}