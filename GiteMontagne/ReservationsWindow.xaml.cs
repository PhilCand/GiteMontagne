using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace GiteMontagne
{
    /// <summary>
    /// Logique d'interaction pour ReservationsWindow.xaml
    /// </summary>
    public partial class ReservationsWindow : Window
    {
        public ReservationsWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            listViewReservations.ItemsSource = DAL.GetReservations();
        }

        private void BtnSupprimer_Click(object sender, RoutedEventArgs e)
        {
            if (listViewReservations.SelectedIndex >= 0)
            {
                DAL.DeleteReservation((listViewReservations.SelectedItem as Reservation).Id);
                listViewReservations.ItemsSource = DAL.GetReservations();
            }
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
