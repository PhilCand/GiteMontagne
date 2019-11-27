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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace GiteMontagne
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private Customer _selectedCustomer;
        private DispatcherTimer dispatcherTimer;

        public Customer SelectedCustomer { get => _selectedCustomer; set => _selectedCustomer = value; }

        public MainWindow()
        {
            InitializeComponent();
            //Create a timer with interval of 2 secs
            dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 2);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            RAZ();
            dpArrivalDate.DisplayDateStart = DateTime.Now;
            dpDepartureDate.DisplayDateStart = DateTime.Now.AddDays(1);

        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void BtnResetAll_Click(object sender, RoutedEventArgs e)
        {
            RAZ();
        }

        private void BtnSelectCustomer_Click(object sender, RoutedEventArgs e)
        {
            CustomerWindow cw = new CustomerWindow();
            Customer SelectCustomer = cw.ShowDialogWithResult();
            if (SelectCustomer != null)
            {
                SelectedCustomer = SelectCustomer;
                txtbNameCustomer.Text = SelectedCustomer.Name;
                txtbNameCustomer.Visibility = Visibility.Visible;
                txtbFirstNameCustomer.Text = SelectedCustomer.FirstName;
                txtbFirstNameCustomer.Visibility = Visibility.Visible;
                txtbPhoneCustomer.Text = SelectedCustomer.Phone;
                txtbPhoneCustomer.Visibility = Visibility.Visible;
                txtbEmailCustomer.Text = SelectedCustomer.Email;
                txtbEmailCustomer.Visibility = Visibility.Visible;
            }

        }

        private void BtnAddBed_Click(object sender, RoutedEventArgs e)
        {
            if (dpArrivalDate.SelectedDate == null || dpDepartureDate.SelectedDate == null)
            {
                MessageBox.Show("Selectionnez des dates de départs et d'arrivée", "Attention", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }

            if (listViewAvaibleBeds.SelectedIndex >= 0)
            {
                listViewSelectedBeds.Items.Add(listViewAvaibleBeds.SelectedItem);
                DAL.MakeBedUnavaible(listViewAvaibleBeds.SelectedItem as Bed);
                GetAvaibleBeds();
                GetTotalPrice();
            }

        }

        private void BtnRemoveBed_Click(object sender, RoutedEventArgs e)
        {
            if (listViewSelectedBeds.SelectedIndex >= 0)
            {
                DAL.MakeBedAvaible(listViewSelectedBeds.SelectedItem as Bed);
                listViewSelectedBeds.Items.Remove(listViewSelectedBeds.SelectedItem);
                GetAvaibleBeds();
                GetTotalPrice();
            }

        }

        private void BtnCreateResevation_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (SelectedCustomer == null)
                {
                    MessageBox.Show("Selectionnez un client", "Attention", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return;
                }

                if (dpArrivalDate.Text == null || dpDepartureDate == null)
                {
                    MessageBox.Show("Selectionnez des dates", "Attention", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return;
                }

                if (listViewSelectedBeds.Items.Count == 0)
                {
                    MessageBox.Show("Selectionnez au moins un lit", "Attention", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return;
                }

                Reservation newReservation = new Reservation();
                newReservation.Customer = SelectedCustomer;
                newReservation.ArrivalDate = Convert.ToDateTime(dpArrivalDate.Text);
                newReservation.DepartureDate = Convert.ToDateTime(dpDepartureDate.Text);
                newReservation.Comment = txtbComment.Text;
                newReservation.Price = double.Parse(txtbTotalPrice.Text);
                newReservation.NumberOfBeds = listViewSelectedBeds.Items.Count;

                foreach (Bed bed in listViewSelectedBeds.Items)
                {
                    newReservation.RevervedBeds.Add(bed);
                }

                DAL.CreateReservation(newReservation);

                labelConfirm.Visibility = System.Windows.Visibility.Visible;
                dispatcherTimer.Start();

                RAZ();
            }

            catch (System.Data.SqlClient.SqlException ex)
            {
                MessageBox.Show("Le client sélectionné n'existe pas dans la base de donnée", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnShowResevations_Click(object sender, RoutedEventArgs e)
        {
            ReservationsWindow rw = new ReservationsWindow();
            rw.ShowDialog();

        }

        private void RAZ()
        {
            SelectedCustomer = null;
            txtbNameCustomer.Clear();
            txtbFirstNameCustomer.Clear();
            txtbPhoneCustomer.Clear();
            txtbEmailCustomer.Clear();
            dpArrivalDate.SelectedDate = null;
            dpDepartureDate.SelectedDate = null;
            txtbComment.Clear();
            listViewSelectedBeds.Items.Clear();
            DAL.MakeAllBedAvaible();
            listViewAvaibleBeds.ItemsSource = DAL.GetBeds();
            txtbTotalPrice.Text = "0";
            txtbNameCustomer.Visibility = Visibility.Hidden;
            txtbFirstNameCustomer.Visibility = Visibility.Hidden;
            txtbPhoneCustomer.Visibility = Visibility.Hidden;
            txtbEmailCustomer.Visibility = Visibility.Hidden;
        }

        private void DpArrivalDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {

            if (dpArrivalDate.SelectedDate != null)
            {
                DateTime start = (DateTime)dpArrivalDate.SelectedDate;
                dpDepartureDate.DisplayDateStart = start.AddDays(1);
            }
            else dpDepartureDate.DisplayDateStart = DateTime.Now.AddDays(1);

            if (dpArrivalDate.SelectedDate > dpDepartureDate.SelectedDate)
            {
                DateTime start = (DateTime)dpArrivalDate.SelectedDate;
                dpDepartureDate.SelectedDate = start.AddDays(1);
            }
            listViewSelectedBeds.Items.Clear();
            DAL.MakeAllBedAvaible();
            GetAvaibleBeds();
            GetNumberOfNights();
            GetTotalPrice();
        }

        private void DpDepartureDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            listViewSelectedBeds.Items.Clear();
            DAL.MakeAllBedAvaible();
            GetAvaibleBeds();
            GetNumberOfNights();
            GetTotalPrice();
        }

        private void ListViewAvaibleBeds_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (dpArrivalDate.SelectedDate == null || dpDepartureDate.SelectedDate == null)
            {
                MessageBox.Show("Selectionnez des dates de départs et d'arrivée", "Attention", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }
            listViewSelectedBeds.Items.Add(listViewAvaibleBeds.SelectedItem);
            DAL.MakeBedUnavaible(listViewAvaibleBeds.SelectedItem as Bed);
            GetAvaibleBeds();
            GetTotalPrice();
        }

        private void ListViewSelectedBeds_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            DAL.MakeBedAvaible(listViewSelectedBeds.SelectedItem as Bed);
            listViewSelectedBeds.Items.Remove(listViewSelectedBeds.SelectedItem);
            GetAvaibleBeds();
            GetTotalPrice();
        }

        private void GetAvaibleBeds()
        {
            DateTime bookingFromDate = DateTime.Now;
            DateTime bookingToDate = DateTime.Now;

            if (dpArrivalDate.SelectedDate != null)
            {
                bookingFromDate = Convert.ToDateTime(dpArrivalDate.Text);
            }

            if (dpDepartureDate.SelectedDate != null)
            {
                bookingToDate = Convert.ToDateTime(dpDepartureDate.Text);
            }

            listViewAvaibleBeds.ItemsSource = DAL.GetAvaibleBeds(bookingFromDate, bookingToDate);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Voulez vous vraiment quitter ?", "Quitter", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.No) e.Cancel = true;
        }

        private void GetNumberOfNights()
        {

            if (!dpArrivalDate.SelectedDate.HasValue || !dpDepartureDate.SelectedDate.HasValue)
            {
                txtbNbNight.Text = "0";
                return;
            }

            DateTime arrival = dpArrivalDate.SelectedDate.Value.Date;
            DateTime departure = dpDepartureDate.SelectedDate.Value.Date;
            TimeSpan diff = departure.Subtract(arrival);

            txtbNbNight.Text = diff.TotalDays.ToString();

        }

        private void GetTotalPrice()
        {
            double total = 0;

            foreach (Bed bed in listViewSelectedBeds.Items)
            {
                total += bed.Price;
            }
            total *= int.Parse(txtbNbNight.Text);

            txtbTotalPrice.Text = total.ToString();
        }

        private void Window_Activated(object sender, EventArgs e)
        {
            GetAvaibleBeds();
        }

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            //Things which happen after 1 timer interval
            //MessageBox.Show("Show some data");
            labelConfirm.Visibility = System.Windows.Visibility.Collapsed;

            //Disable the timer
            dispatcherTimer.IsEnabled = false;
        }
    }
}
