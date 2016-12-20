using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Timers;
using System.Windows.Threading;

namespace ClientZaptZupt
{
    /// <summary>
    /// Interaction logic for MainProgram.xaml
    /// </summary>
    public partial class MainProgram : Window
    {
        public MainProgram()
        {
            InitializeComponent();
        }

        #region ListOfUsers
        private void LoU_updateList()
        {
            AsynchronousClient.SendMessage("5§" + App.whoAmI + "<EOF>");
            string[] receivedMessage = AsynchronousClient.ReceiveMessage().Split('§');
            ltbFriends.Items.Clear();
            for (int i = 0; i < Convert.ToInt32(receivedMessage[1]); i++)
            {
                if (!receivedMessage[i+2].Equals(App.whoAmI))
                    ltbFriends.Items.Add(receivedMessage[i + 2]);
            }

        }

        private void LoU_btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            this.LoU_updateList();
        }

        

        private void LoU_btnStartTalking_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string friendNickname = ltbFriends.SelectedItem.ToString();
                AsynchronousClient.SendMessage("2§" + App.whoAmI + "§" + friendNickname + "<EOF>");
                string[] receivedMessage = AsynchronousClient.ReceiveMessage().Split('§');
                App.messageBetweenScreens = receivedMessage;
                g_ListOfUsers.Visibility = Visibility.Hidden;
                g_ChatWindow.Visibility = Visibility.Visible;
                startChat(friendNickname);
            }
            catch
            {
                MessageBox.Show("Please select someone to chat!");
            }

        }
        #endregion

        #region ChatWindow

        private void startChat(string friendName)
        {

            Console.WriteLine("Press the Enter key to exit the program at any time... ");
            Console.ReadLine();
            //TIMER THINGS

            lblFriendName.Content = friendName;

            ltbTalk.Items.Clear();
            string[] parts;

            for (int i = 0; i < Convert.ToInt32(App.messageBetweenScreens[1]); i++)
            {
                parts = App.messageBetweenScreens[i + 2].Split('|');

                if (parts[0].Equals(App.whoAmI))
                {
                    addMyMessage(parts[1]);
                }
                else
                {
                    addMyFriendsMessage(parts[1]);
                }

            }
        }

        private void btnListOfFriends_Click(object sender, RoutedEventArgs e)
        {
            ltbFriends.Items.Clear();
            g_ChatWindow.Visibility = Visibility.Hidden;
            g_ListOfUsers.Visibility = Visibility.Visible;
            LoU_updateList();
            //this.Close();
        }
        #endregion

        #region commonFunctions

        private void btnLogout_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            AsynchronousClient.SendMessage("-1§" + App.whoAmI + "<EOF>");
            string[] receivedMessage = AsynchronousClient.ReceiveMessage().Split('§');
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            lblWelcome.Content = "Welcome " + App.whoAmI + ", choose someone to talk!";
            ltbFriends.Items.Clear();

            for (int i = 0; i < Convert.ToInt32(App.messageBetweenScreens[2]); i++)
            {
                if(!App.messageBetweenScreens[i + 3].Equals(App.whoAmI))
                    ltbFriends.Items.Add(App.messageBetweenScreens[i + 3]);
            }
        }

        private void btnSend_Click(object sender, RoutedEventArgs e)
        {
            AsynchronousClient.SendMessage("3§" + App.whoAmI +"§"+ lblFriendName.Content+
                "§"+txtMessageToBeSent.Text+"<EOF>");
            string[] receivedMessage = AsynchronousClient.ReceiveMessage().Split('§');
            addMyMessage(txtMessageToBeSent.Text);
            txtMessageToBeSent.Clear();
            txtMessageToBeSent.Focus();
        }

        private void addMyMessage(string msg)
        {


            ListBoxItem blueItem = new ListBoxItem();
            blueItem.HorizontalAlignment = HorizontalAlignment.Right;
            blueItem.Background = Brushes.DarkBlue;
            blueItem.Content = msg;
            ltbTalk.Items.Add(blueItem);
        }

        private void addMyFriendsMessage(string msg)
        {
            ListBoxItem redItem = new ListBoxItem();
            redItem.HorizontalAlignment = HorizontalAlignment.Left;
            redItem.Background = Brushes.DarkRed;
            redItem.Content = msg;
            ltbTalk.Items.Add(redItem);
        }

        private void ltbTalk_MouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {

        }

        #endregion

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            AsynchronousClient.SendMessage("2§" + App.whoAmI + "§" + lblFriendName.Content + "<EOF>");
            string[] receivedMessage = AsynchronousClient.ReceiveMessage().Split('§');
            App.messageBetweenScreens = receivedMessage;
            startChat(lblFriendName.Content.ToString());
        }
    }

}
