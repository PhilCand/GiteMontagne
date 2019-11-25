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
    /// Logique d'interaction pour EditReservationWindow.xaml
    /// </summary>
    public partial class EditReservationWindow : Window
    {
        private Customer _selectedCustomer;
        private Reservation _editedReservation;

        public Customer SelectedCustomer { get => _selectedCustomer; set => _selectedCustomer = value; }
        internal Reservation EditedReservation { get => _editedReservation; set => _editedReservation = value; }

        public EditReservationWindow()
        {
            InitializeComponent();
        }

        public EditReservationWindow(int editedReservationID)
        {
            InitializeComponent();
            EditedReservation = DAL.GetUpdatedReservation(editedReservationID);
            SelectedCustomer = EditedReservation.Customer;
            txtbNameCustomer.Text = SelectedCustomer.Name;
            txtbFirstNameCustomer.Text = SelectedCustomer.FirstName;
            txtbEmailCustomer.Text = SelectedCustomer.Email;
            txtbPhoneCustomer.Text = SelectedCustomer.Phone;
            dpArrivalDate.SelectedDate = EditedReservation.ArrivalDate;
            dpDepartureDate.SelectedDate = EditedReservation.DepartureDate;

            foreach (Bed bed in EditedReservation.RevervedBeds)
            {
                listViewSelectedBeds.Items.Add(bed);
                DAL.MakeBedUnavaible(bed);
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            GetAvaibleBeds();
            RAZ();
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
            SelectedCustomer = cw.ShowDialogWithResult();
            if (SelectedCustomer != null)
            {
                txtbNameCustomer.Text = SelectedCustomer.Name;
                txtbFirstNameCustomer.Text = SelectedCustomer.FirstName;
                txtbPhoneCustomer.Text = SelectedCustomer.Phone;
                txtbEmailCustomer.Text = SelectedCustomer.Email;
            }

        }

        private void BtnAddBed_Click(object sender, RoutedEventArgs e)
        {
            if (dpArrivalDate.SelectedDate == null || dpDepartureDate.SelectedDate == null)
            {
                MessageBox.Show("Selectionnez des dates de départs et d'arrivée", "Attention", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }
            listViewSelectedBeds.Items.Add(listViewAvaibleBeds.SelectedItem);
            DAL.MakeBedUnavaible(listViewAvaibleBeds.SelectedItem as Bed);
            GetAvaibleBeds();
        }

        private void BtnRemoveBed_Click(object sender, RoutedEventArgs e)
        {
            DAL.MakeBedAvaible(listViewSelectedBeds.SelectedItem as Bed);
            listViewSelectedBeds.Items.Remove(listViewSelectedBeds.SelectedItem);
            GetAvaibleBeds();
        }

        private void BtnEditResevation_Click(object sender, RoutedEventArgs e)
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
            newReservation.Id = EditedReservation.Id;
            newReservation.Customer = SelectedCustomer;
            newReservation.ArrivalDate = Convert.ToDateTime(dpArrivalDate.Text);
            newReservation.DepartureDate = Convert.ToDateTime(dpDepartureDate.Text);
            newReservation.Comment = txtbComment.Text;
            MessageBox.Show("Modifications effectuées", "Edition réservation", MessageBoxButton.OK, MessageBoxImage.Information);
            this.Close();

            foreach (Bed bed in listViewSelectedBeds.Items)
            {
                newReservation.RevervedBeds.Add(bed);
            }

            DAL.UpdateReservation(newReservation);
            RAZ();
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
            GetAvaibleBeds();
        }

        private void DpArrivalDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
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

        private void DpDepartureDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
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
        }

        private void ListViewSelectedBeds_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            DAL.MakeBedAvaible(listViewSelectedBeds.SelectedItem as Bed);
            listViewSelectedBeds.Items.Remove(listViewSelectedBeds.SelectedItem);
            GetAvaibleBeds();
        }

        private void BtnEditDates_Click(object sender, RoutedEventArgs e)
        {
            dpArrivalDate.IsEnabled = true;
            dpDepartureDate.IsEnabled = true;

            foreach (Bed bed in listViewSelectedBeds.Items)
            {
                DAL.MakeBedAvaible(bed);
            }

            listViewSelectedBeds.Items.Clear();


            DateTime bookingFromDate = Convert.ToDateTime(dpArrivalDate.Text);
            DateTime bookingToDate = Convert.ToDateTime(dpDepartureDate.Text);
            
            listViewAvaibleBeds.ItemsSource = DAL.GetAvaibleBeds(bookingFromDate, bookingToDate, EditedReservation.Id);

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

            listViewAvaibleBeds.ItemsSource = DAL.GetAvaibleBeds(bookingFromDate, bookingToDate, EditedReservation.Id);
        }

       
    }
}
