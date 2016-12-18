using System;
using System.Data.SqlClient;

namespace ServerZaptZupt
{

    class DBController
    {
        static SqlConnection connection;


        static string connString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\Database1.mdf;Integrated Security=True;Connect Timeout=30";

        // Constructor
        // String connection: Server=127.0.0.1;Port=5432;Database=myDataBase;User Id=myUsername;Password=myPassword;
        public DBController() { }

        public static void OpenConnection()
        {
            connection = new SqlConnection(connString);
            connection.Open();
        }

        public static void CloseConnection()
        {
            connection.Close();
        }

        //Get all User registers

            /// <summary>
            /// Execute an SQL Command
            /// </summary>
            /// <param name="operation"> 0 - SELECT ||| 1 - INSERT ||| 2 - UPDATE </param>
            /// <param name="fields">Select: fields to be shown ||| Insert: fields to be changed ||| Update: field=value,field=value,...</param>
            /// <param name="tables">Tables to execute the command</param>
            /// <param name="conditions">Select: conditions ||| Insert: values to be inserted ||| Update: conditions</param>
            /// <param name="result">Output variable: number of rows affected by Insert/Update, -1 otherwise</param>
            /// <returns></returns>
        public static SqlDataReader ExecuteQuery(int operation, string fields, string tables, string conditions, out int result)
        {
            string commandString = null;

            SqlCommand cmd;
            SqlDataReader dr = null;

            result = -1;
            // Define a query
            switch (operation)
            {
                case 0: //SELECT
                    if (conditions == "")
                        commandString = "SELECT " + fields + " FROM " + tables;
                    else
                        commandString = "SELECT " + fields + " FROM " + tables + " WHERE " + conditions;                    
                    cmd = new SqlCommand(commandString, connection);
                    // Execute the query and obtain a result set
                    dr = cmd.ExecuteReader();
                    if (dr != null)
                        result = 1;
                    break;
                case 1: //INSERT
                    commandString = "INSERT INTO " + tables + "(" + fields + ")" + " VALUES (" + conditions + ")";
                    cmd = new SqlCommand(commandString, connection);
                    result = cmd.ExecuteNonQuery();
                    break;
                case 2: //UPDATE
                    commandString = "UPDATE " + tables + " SET " + fields + " WHERE " + conditions;
                    cmd = new SqlCommand(commandString, connection);
                    result = cmd.ExecuteNonQuery();

                    break;
            }
            return dr;
        }

        //Get all Messages registers
        public static void GetAllMessages()
        {
            SqlConnection connection = new SqlConnection(connString);
            connection.Open();

            // Define a query
            SqlCommand cmd = new SqlCommand("SELECT * FROM messages", connection);

            // Execute the query and obtain a result set
            SqlDataReader dr = cmd.ExecuteReader();

            // Output rows
            while (dr.Read())
                Console.Write("Sender: {0}\t Receiver: {1}\t Date: {2}\t Text:{3}\n", dr[0], dr[1], dr[2], dr[3]);

            connection.Close();
        }


    }

}
