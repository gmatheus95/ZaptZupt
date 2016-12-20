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

        private void joinServer()
        {
            AsynchronousClient.SendMessage("0§" + txtNickName.Text + "§" + txtPassword.Password + "<EOF>");
            string[] receivedMessage = AsynchronousClient.ReceiveMessage().Split('§');

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
                        App.whoAmI = txtNickName.Text;
                        App.messageBetweenScreens = receivedMessage;
                        MainProgram newWindow = new MainProgram();
                        this.Visibility = Visibility.Hidden;
                        newWindow.Owner = this;
                        newWindow.ShowDialog(); //waits until the dialog closes
                        this.Visibility = Visibility.Visible;
                        this.txtNickName.Text = "";
                        this.txtPassword.Password = "";
                        this.txtNickName.Focus();
                        break;
                }
            }
        }

        private void btnJoinServer_Click(object sender, RoutedEventArgs e)
        {
            joinServer();
        }

        private void Window_Activated(object sender, EventArgs e)
        {
            
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            AsynchronousClient.ShutdownClient();
            App.Current.Shutdown();
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

        private void txtPassword_TouchEnter(object sender, System.Windows.Input.TouchEventArgs e)
        {
            joinServer();
        }
    }
}
