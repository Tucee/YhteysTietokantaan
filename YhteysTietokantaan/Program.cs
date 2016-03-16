using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Data;          
using System.Data.SqlClient;

namespace YhteysTietokantaan
{
    class Program
    {
        static void Main(string[] args)
        {
            string connectionString4NoUserIDNoPassword,
                 password, userName, SQLConnectionString;

            // Get most of the connection string from ConnectAndQuery_Example.exe.config
            // file, in the same directory where ConnectAndQuery_Example.exe resides.
            connectionString4NoUserIDNoPassword = Program.GetConnectionStringFromExeConfig
                ("ConnectionString4NoUserIDNoPassword");
            // Get the user name from keyboard input.
            Console.WriteLine("Otetaan yhteys Azure SQL Serverin tietokantaan\nAzure SQL Server käyttäjätiedot");
            Console.WriteLine("Anna käyttäjätunnus: ");
            userName = Console.ReadLine();
            // Get the password from keyboard input.
            password = Program.GatherPasswordFromConsole();

            SQLConnectionString = "Password=" + password + ';' +
                "User ID=" + userName + ";" + connectionString4NoUserIDNoPassword;

            // Create an SqlConnection from the provided connection string.
            using (SqlConnection connection = new SqlConnection(SQLConnectionString))
            {
                // Formulate the command.
                SqlCommand command = new SqlCommand();
                command.Connection = connection;

                // Specify the query to be executed.
                command.CommandType = CommandType.Text;

                command.CommandText = @"
                    SELECT * FROM TodoItems WHERE Complete = 0";

                // Open a connection to database.
                connection.Open();

                // Read data returned for the query.
                SqlDataReader reader = command.ExecuteReader();
                Console.WriteLine("\n---------------------\nTietokannassa olevat tiedot:");
                while (reader.Read())
                {
                    Console.WriteLine("Tiedot:  {0}, {1}",
                         reader[1], reader[4]);
                }
            }
            Console.WriteLine("\nPaina jotain näppäintä lopettaaksesi...");
            Console.ReadKey(true);
        }

        static string GetConnectionStringFromExeConfig(string connectionStringNameInConfig)
        {
            ConnectionStringSettings connectionStringSettings =
                ConfigurationManager.ConnectionStrings[connectionStringNameInConfig];

            if (connectionStringSettings == null)
            {
                throw new ApplicationException(String.Format
                    ("Error. Connection string not found for name '{0}'.",
                    connectionStringNameInConfig));
            }
            return connectionStringSettings.ConnectionString;
        }

        static string GatherPasswordFromConsole()
        {
            StringBuilder passwordBuilder = new StringBuilder(32);
            ConsoleKeyInfo key;
            Console.WriteLine("Anna salasana: ");
            do
            {
                key = Console.ReadKey(true);
                if (key.Key != ConsoleKey.Backspace)
                {
                    passwordBuilder.Append(key.KeyChar);
                    Console.Write("*");
                }
                else  // Backspace char was entered.
                {
                    // Retreat the cursor, overlay '*' with ' ', retreat again.
                    Console.Write("\b \b");
                    passwordBuilder.Length = passwordBuilder.Length - 1;
                }
            }
            while (key.Key != ConsoleKey.Enter); // Enter key will end the looping.
            Console.WriteLine(Environment.NewLine);
            return passwordBuilder.ToString();
        }
    }
}
