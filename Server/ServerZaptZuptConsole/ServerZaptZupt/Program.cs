using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Data.SqlClient;

namespace ServerZaptZupt
{
    class Program
    {
        public static int Main(String[] args)
        {
            int queryResult;
            //DBController.OpenConnection();

            ////SqlDataReader dr = DBController.ExecuteQuery(0, "*", "users", "nickname = \'alexandre\'", out queryResult);


            ////"INSERT INTO users(nickname,password) VALUES ('lalala','lalala')"
            //SqlDataReader dr = DBController.ExecuteQuery(1, "nickname,password", "users", "'lalala','lalala'", out queryResult);


            //while (dr.Read())
            //    Console.WriteLine(dr[0].ToString() + dr[1].ToString());
            //DBController.CloseConnection();
            AsynchronousSocketListener.StartListening();
            return 0;

        }
    }
}

