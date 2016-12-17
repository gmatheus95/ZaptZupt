using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace ServerZaptZupt
{
    class Program
    {
        

// State object for reading client data asynchronously
        


            public static int Main(String[] args)
            {
                AsynchronousSocketListener.StartListening();
                return 0;
            }
        }

    }
}

