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
        
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            lblWelcome.Content = "Welcome " + App.whoAmI + ", choose someone to talk!";
            ltbTalk.Items.Clear();

            for (int i =0; i < Convert.ToInt32(App.messageBetweenScreens[2]);i++)
            {
                ltbTalk.Items.Add(App.messageBetweenScreens[i+3]);                
            }
        }

        private void updateList()
        {
            AsynchronousClient.SendMessage("5§" + App.whoAmI + "<EOF>");
            string[] receivedMessage = AsynchronousClient.ReceiveMessage().Split('§');
            ltbTalk.Items.Clear();
            for (int i = 0; i < Convert.ToInt32(receivedMessage[1]); i++)
            {
                ltbTalk.Items.Add(receivedMessage[i + 2]);
            }
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            this.updateList();   
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            AsynchronousClient.SendMessage("-1§" + App.whoAmI + "<EOF>");
            string[] receivedMessage = AsynchronousClient.ReceiveMessage().Split('§');
            Console.WriteLine(receivedMessage[0]);
        }

        private void btnLogout_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnStartTalking_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string friendNickname = ltbTalk.SelectedItem.ToString();
                AsynchronousClient.SendMessage("2§" + App.whoAmI + "§" + friendNickname + "<EOF>");
                string[] receivedMessage = AsynchronousClient.ReceiveMessage().Split('§');
                App.messageBetweenScreens = receivedMessage;
                ChatWindow newWindow = new ChatWindow();
                this.Visibility = Visibility.Hidden;
                newWindow.Owner = this;
                newWindow.ShowDialog(); //waits until the dialog closes
                this.Visibility = Visibility.Visible;
                this.updateList();
            }
            catch
            {
                MessageBox.Show("Please select someone to chat!");
            }
            
        }
    }
}
