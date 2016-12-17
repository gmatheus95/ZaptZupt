using System;
using System.Windows;


namespace ClientZaptZupt
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

       
        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            AsynchronousClient.StartClient();        
        }

        private void Window_Closed(object sender, EventArgs e)
        {

        }

        private void btnListOfFriends_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Lista de amiguinhos!");
        }

        private void btnLogout_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Sai fora!");
        }
    }

    

}

