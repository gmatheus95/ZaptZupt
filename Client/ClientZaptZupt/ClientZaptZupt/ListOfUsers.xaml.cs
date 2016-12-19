using System;
using System.Windows;
using System.Timers;

namespace ClientZaptZupt
{
    /// <summary>
    /// Interaction logic for ListOfUsers.xaml
    /// </summary>
    public partial class ListOfUsers : Window
    {
        public ListOfUsers()
        {
            InitializeComponent();
        }



        private static Timer aTimer;

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            lblWelcome.Content = "Welcome " + App.whoAmI + ", choose someone to talk!";
            ltbTalk.Items.Clear();
            AsynchronousClient.SendMessage("5§" + App.whoAmI + "<EOF>");
            string[] receivedMessage = AsynchronousClient.ReceiveMessage().Split('§');

            //AsynchronousClient.SendMessage("-2§0<EOF>");
            //receivedMessage = AsynchronousClient.ReceiveMessage().Split('§');

            Console.WriteLine(receivedMessage[0]);
            
            //AsynchronousClient.SendMessage("5§" + App.whoAmI + "<EOF>");

            for (int i =0; i < Convert.ToInt32(receivedMessage[1]);i++)
            {
                ltbTalk.Items.Add(receivedMessage[i+2]);
                
            }
            //aTimer = new Timer();
            //aTimer.Interval = 2000;
            //// Hook up the Elapsed event for the timer. 
            //aTimer.Elapsed += OnTimedEvent;
            //// Have the timer fire repeated events (true is the default)
            //aTimer.AutoReset = true;
            //// Start the timer
            //aTimer.Enabled = true;
        }

        //private static void OnTimedEvent(Object source, System.Timers.ElapsedEventArgs e)
        //{
        //    //MessageBox.Show("Refresh!");
        //    //send message to the server
            

        //}

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            AsynchronousClient.SendMessage("5§" + App.whoAmI + "<EOF>");
            string[] receivedMessage = AsynchronousClient.ReceiveMessage().Split('§');
            //AsynchronousClient.SendMessage("5§" + App.whoAmI + "<EOF>");
            //receivedMessage = AsynchronousClient.ReceiveMessage().Split('§');
            ltbTalk.Items.Clear();
            for (int i = 0; i < Convert.ToInt32(receivedMessage[1]); i++)
            {
                ltbTalk.Items.Add(receivedMessage[i + 2]);
            }
            Console.WriteLine(receivedMessage[0]);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            AsynchronousClient.SendMessage("-1§" + App.whoAmI + "<EOF>");
            string[] receivedMessage = AsynchronousClient.ReceiveMessage().Split('§');
            Console.WriteLine(receivedMessage[0]);
            App.Current.Shutdown();
        }

        private void btnLogout_Click(object sender, RoutedEventArgs e)
        {
            AsynchronousClient.SendMessage("-1§" + App.whoAmI + "<EOF>");
            string[] receivedMessage = AsynchronousClient.ReceiveMessage().Split('§');
            Console.WriteLine(receivedMessage[0]);
            App.Current.Shutdown();
        }

    }
}
