using System;
using System.Windows;

namespace ClientZaptZupt
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : Window
    {
        public Login()
        {
            InitializeComponent();
        }

        private void btnJoinServer_Click(object sender, RoutedEventArgs e)
        {
            AsynchronousClient.SendMessage("0§" + txtNickName.Text + "§" + txtPassword.Password+"<EOF>");
            string[] receivedMessage = AsynchronousClient.ReceiveMessage().Split('§');
            AsynchronousClient.SendMessage("0§" + "alexandre" + "§" + "1111"+ "<EOF>");
            receivedMessage = AsynchronousClient.ReceiveMessage().Split('§');
            if (receivedMessage[0] == "0")
            {
                switch (receivedMessage[1])
                {
                    case "-1":
                        MessageBox.Show("User already exists and the password is incorrect.");
                        txtNickName.Clear();
                        txtPassword.Clear();
                        txtNickName.Focus();                        
                        break;
                    case "1":
                        App.messageBetweenScreens = receivedMessage;
                        ListOfUsers newWindow = new ListOfUsers();
                        newWindow.Show();
                        Close();
                        break;
                }
            }
        }

        private void Window_Activated(object sender, EventArgs e)
        {
            
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            AsynchronousClient.ShutdownClient();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                AsynchronousClient.StartClient();
            }
            catch
            {
                MessageBox.Show("Could not connect to the server, closing the application...");
                this.Close();
            }
        }
    }
}
