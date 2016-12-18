using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Collections;
using System.Data.SqlClient;
using System.Collections.Generic;

namespace ServerZaptZupt
{
    public class StateObject
    {
        // Client  socket.
        public Socket workSocket = null;
        // Size of receive buffer.
        public const int BufferSize = 1024;
        // Receive buffer.
        public byte[] buffer = new byte[BufferSize];
        // Received data string.
        public StringBuilder sb = new StringBuilder();
    }

    
    public class AsynchronousSocketListener
    {

        private static List<string> onlineUsers;

        private static Dictionary<string, List<string>> dictPendingMessages;

        // Thread signal.
        public static ManualResetEvent allDone = new ManualResetEvent(false);

        public AsynchronousSocketListener()
        {
        }

        public static void StartListening()
        {
            onlineUsers = new List<string>();

            dictPendingMessages = new Dictionary<string, List<string>>();
            // Data buffer for incoming data.
            byte[] bytes = new Byte[1024];

            //Open the connection to the database
            
            // Establish the local endpoint for the socket.
            // The DNS name of the computer
            // running the listener is "host.contoso.com".
            IPHostEntry ipHostInfo = Dns.Resolve(Dns.GetHostName());
            IPAddress ipAddress = ipHostInfo.AddressList[0];
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, 11000);

            // Create a TCP/IP socket.
            Socket listener = new Socket(AddressFamily.InterNetwork,
                SocketType.Stream, ProtocolType.Tcp);

            // Bind the socket to the local endpoint and listen for incoming connections.
            try
            {
                listener.Bind(localEndPoint);
                listener.Listen(100); //maximum number of connections in this socket (50 chats at the same time)

                while (true)
                {
                    // Set the event to nonsignaled state.
                    allDone.Reset();

                    // Start an asynchronous socket to listen for connections.
                    Console.WriteLine("Waiting for a connection...");
                    listener.BeginAccept(
                        new AsyncCallback(AcceptCallback),
                        listener);

                    // Wait until a connection is made before continuing.
                    allDone.WaitOne();
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }


            Console.WriteLine("\nPress ENTER to continue...");
            Console.Read();

        }

        public static void AcceptCallback(IAsyncResult ar)
        {
            // Signal the main thread to continue.
            allDone.Set();

            // Get the socket that handles the client request.
            Socket listener = (Socket)ar.AsyncState;
            Socket handler = listener.EndAccept(ar);

            // Create the state object.
            StateObject state = new StateObject();
            state.workSocket = handler;
            handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                new AsyncCallback(ReadCallback), state);
        }

        public static void ReadCallback(IAsyncResult ar)
        {
            String content = String.Empty;

            // Retrieve the state object and the handler socket
            // from the asynchronous state object.
            StateObject state = (StateObject)ar.AsyncState;
            Socket handler = state.workSocket;

            // Read data from the client socket. 
            int bytesRead = handler.EndReceive(ar);

            if (bytesRead > 0)
            {
                // There  might be more data, so store the data received so far.
                state.sb.Append(Encoding.UTF8.GetString(
                    state.buffer, 0, bytesRead));

                // Check for end-of-file tag. If it is not there, read 
                // more data.
                content = state.sb.ToString();
                if (content.IndexOf("<EOF>") > -1)
                {
                    // All the data has been read from the 
                    // client. Display it on the console.

                    //Neste momento, a mensagem foi recebida por completo do cliente pelo servidor

                    string[] message = content.Substring(0,content.IndexOf("<EOF>")).Split('§');
                    //the item 0 is the action
                    //the second is the sender (client)
                    //the next ones are the further parameters (depending on the function)

                    content = HandleMessage(message);

                    // Echo the data back to the client.
                    Send(handler, content);
                }
                else
                {
                    // Not all data received. Get more.
                    handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                    new AsyncCallback(ReadCallback), state);
                }
            }
        }

        private static void Send(Socket handler, String data)
        {
            // Convert the string data to byte data using ASCII encoding.
            byte[] byteData = Encoding.UTF8.GetBytes(data);

            // Begin sending the data to the remote device.
            handler.BeginSend(byteData, 0, byteData.Length, 0,
                new AsyncCallback(SendCallback), handler);
        }

        private static void SendCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object.
                Socket handler = (Socket)ar.AsyncState;

                // Complete sending the data to the remote device.
                int bytesSent = handler.EndSend(ar);
                Console.WriteLine("Sent {0} bytes to client.", bytesSent);

                handler.Shutdown(SocketShutdown.Both);
                handler.Close();

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private static string HandleMessage(string[] message) //MENSAGEM JA DIVIDIDA LA EM CIMA
        {
            string serverResponse = "";
            int queryResult;
            SqlDataReader dr;
            DBController dbController = new DBController();
            dbController.OpenConnection();
            switch (message[0])
            {
                //(0,nickname,password) OK
                #region Login 
                case "0": //Login request   
                    Console.WriteLine("Login request from " + message[1]);
                    dr = dbController.ExecuteQuery(0, "*", "users", "nickname = '"+message[1]+"';", out queryResult);
                    if (dr.Read()) //the user exists
                    {
                        //the entered password matches the one on the database
                        if (dr[1].ToString() == message[2])
                        {
                            Console.WriteLine("User " + message[1] + " is online!");
                            //passing the online users through the response
                            serverResponse = "0§1§" + onlineUsers.Count.ToString();
                            foreach (string nick in onlineUsers)
                            {
                                serverResponse += "§" + nick;
                            }
                            onlineUsers.Add(message[1]);
                        }
                        else //incorrect password
                        {
                            Console.WriteLine("User " + message[1] + " unable to connect");
                            serverResponse = "0§-1";
                        }
                    }
                    else //the user doesn't exist yet
                    {
                        dr = dbController.ExecuteQuery(1,"nickname,password","users","'"+message[1]+"','"+message[2]+"';",out queryResult);
                        if (queryResult == 1)
                        {
                            
                            Console.WriteLine("User " + message[1] + " was successfully created");
                            Console.WriteLine("User " + message[1] + " is online!");
                            //passing the online users through the response
                            serverResponse = "0§1§" + onlineUsers.Count.ToString();
                            foreach (string nick in onlineUsers)
                            {
                                serverResponse += "§" + nick;
                            }
                            onlineUsers.Add(message[1]);
                        }
                    }                                         
                    break;
                #endregion

                //NOT IMPLEMENTED
                #region ChangeNickname
                //case "1": //Change nick request
                //    Console.WriteLine(message[1] + " requested to change their nickname");
                //    //message[2] is the new nick
                //    break;
                #endregion

                //(-1,nickname) OK
                #region Logout
                case "-1": //Logout request
                    Console.WriteLine(message[1] + " has logged out!");
                    onlineUsers.RemoveAt(onlineUsers.IndexOf(message[1]));
                    serverResponse = "-1§1";
                    //remover o nickname que acabou de sair da lista de usuários online
                    break;
                #endregion//OK

                //(2,nickname,friend_nickname) OK
                #region StartNewChat
                case "2":
                    Console.WriteLine(message[1] + " wants to start a new chat with" + message[2]);
                    if (onlineUsers.IndexOf(message[2]) == -1)
                    {
                        Console.WriteLine(message[1] + " was unable to start a chat with " + message[2]);
                        serverResponse = "2§-1";
                    }
                    //the conversation started successfully
                    else 
                    {                        
                        Console.WriteLine(message[1] + " started to talk with " + message[2]);

                        dr = dbController.ExecuteQuery(0, "COUNT(nickname)", "messages", "(sender = '" + message[1] +
                            "' AND receiver = '" + message[2] + "') OR (sender = '" + message[2] +
                            "' AND receiver = '" + message[1] + "');", out queryResult);

                        dr.Read();
                        serverResponse = "2§"+dr[0].ToString();

                        dr = dbController.ExecuteQuery(0, "*", "messages", "(sender = '" + message[1] +
                            "' AND receiver = '" + message[2] + "') OR (sender = '" + message[2] +
                            "' AND receiver = '" + message[1] + "');", out queryResult);

                        while (dr.Read()) //the user exists
                        {                            
                                              // sender | message | timestamp
                            serverResponse += "§" + dr[0].ToString() + "|" + dr[3].ToString() + "|" + dr[2].ToString(); 
                        }
                    }
                    break;
                #endregion

                #region SendMessage
                case "3":
                    Console.WriteLine(message[1] + " sent the following message to " + message[2] + ":");
                    Console.WriteLine(message[3]);

                    try
                    {
                        //dictionary that holds the messages to be sent to the receiver
                        dictPendingMessages[message[1] + "|" + message[2]].Add(message[3]);
                    }
                    catch
                    {
                        List<string> aux = new List<string>();
                        aux.Add(message[3]);
                        dictPendingMessages.Add(message[1] + "|" + message[2],aux);
                    }

                    dr = dbController.ExecuteQuery(1, "sender, receiver, msg", "messages", "'" + message[1] +
                        "','" + message[2] + "', '" + message[3] + "';",out queryResult);
                    
                    break;
                #endregion

                #region CheckPendingMessages
                //Check pending messages (4,nickname,friend_nickname)
                case "4":
                    //response: (4,message|message|...)
                    serverResponse = "4§";
                    foreach (string msg in dictPendingMessages[message[1]+"|"+message[2]])
                    {
                        serverResponse += msg + "|";
                    }
                    break;
               #endregion

            }

            dbController.CloseConnection();
            return serverResponse+"<EOF>"; //RETORNA A MENSAGEM QUE DEVE SER ENVIADA DO SERVIDOR PARA O CLIENTE

        }
    }
}
