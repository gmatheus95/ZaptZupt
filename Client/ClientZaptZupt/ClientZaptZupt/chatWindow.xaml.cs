using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ClientZaptZupt
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class ChatWindow : Window
    {
        public ChatWindow()
        {
            InitializeComponent();
        }
        ListBoxItem redItem;
        ListBoxItem blueItem;

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            redItem = new ListBoxItem();
            redItem.HorizontalAlignment = HorizontalAlignment.Left;
            redItem.Background = Brushes.DarkRed;
            //redItem.Content = "Olá!";

            blueItem = new ListBoxItem();
            blueItem.HorizontalAlignment = HorizontalAlignment.Right;
            blueItem.Background = Brushes.DarkBlue;
            //blueItem.Content = "Que delícia cara :3";

            ltbTalk.Items.Clear();
            string[] parts;

            for (int i = 0; i < Convert.ToInt32(App.messageBetweenScreens[1]); i++)
            {
                parts = App.messageBetweenScreens[i + 2].Split('|');

                if (parts[0].Equals(App.whoAmI))
                {
                    blueItem.Content = parts[1];
                    ltbTalk.Items.Add(blueItem);
                }
                else
                {
                    redItem.Content = parts[1];    
                    ltbTalk.Items.Add(redItem);
                }
                    
            }

            ltbTalk.Items.Add(blueItem);
            ltbTalk.Items.Add(redItem);
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnListOfFriends_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnLogout_Click(object sender, RoutedEventArgs e)
        {
            App.Current.Shutdown();
        }
    }

    

}

