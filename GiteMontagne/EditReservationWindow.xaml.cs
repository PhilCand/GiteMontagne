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
            GetAvaibleBeds();

            foreach (Bed bed in EditedReservation.RevervedBeds)
            {
                listViewSelectedBeds.Items.Add(bed);
                DAL.MakeBedUnavaible(bed);
            }

            GetNumberOfNights();
            GetTotalPrice();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {            
            RAZ();
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void BtnSelectCustomer_Click(object sender, RoutedEventArgs e)
        {
            CustomerWindow cw = new CustomerWindow();
            Customer SelectCustomer = cw.ShowDialogWithResult();
            if (SelectCustomer != null)
            {
                SelectedCustomer = SelectCustomer;
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

            if (listViewAvaibleBeds.SelectedIndex >= 0)
            {
                listViewSelectedBeds.Items.Add(listViewAvaibleBeds.SelectedItem);
                DAL.MakeBedUnavaible(listViewAvaibleBeds.SelectedItem as Bed);
                GetAvaibleBedsWithEdited();
                GetTotalPrice();
            }

        }

        private void BtnRemoveBed_Click(object sender, RoutedEventArgs e)
        {
            if (listViewSelectedBeds.SelectedIndex >= 0)
            {
                DAL.MakeBedAvaible(listViewSelectedBeds.SelectedItem as Bed);
                listViewSelectedBeds.Items.Remove(listViewSelectedBeds.SelectedItem);
                GetAvaibleBedsWithEdited();
                GetTotalPrice();
            }
        }

        private void BtnEditResevation_Click(object sender, RoutedEventArgs e)
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
                newReservation.Id = EditedReservation.Id;
                newReservation.Customer = SelectedCustomer;
                newReservation.ArrivalDate = Convert.ToDateTime(dpArrivalDate.Text);
                newReservation.DepartureDate = Convert.ToDateTime(dpDepartureDate.Text);
                newReservation.Comment = txtbComment.Text;
                newReservation.Price = double.Parse(txtbTotalPrice.Text);
                newReservation.NumberOfBeds = listViewSelectedBeds.Items.Count;

                MessageBox.Show("Modifications effectuées", "Edition réservation", MessageBoxButton.OK, MessageBoxImage.Information);
                this.Close();

                foreach (Bed bed in listViewSelectedBeds.Items)
                {
                    newReservation.RevervedBeds.Add(bed);
                }

                DAL.UpdateReservation(newReservation);
                RAZ();
        }
            catch (System.Data.SqlClient.SqlException)
            {
                MessageBox.Show("Le client sélectionné n'existe pas dans la base de donnée", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }

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

            listViewSelectedBeds.Items.Clear();
            DAL.MakeAllBedAvaible();
            GetAvaibleBedsWithEdited();
            GetNumberOfNights();
            GetTotalPrice();
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

            listViewSelectedBeds.Items.Clear();
            DAL.MakeAllBedAvaible();
            GetAvaibleBedsWithEdited();
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
            GetAvaibleBedsWithEdited();
            GetTotalPrice();
        }

        private void ListViewSelectedBeds_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            DAL.MakeBedAvaible(listViewSelectedBeds.SelectedItem as Bed);
            listViewSelectedBeds.Items.Remove(listViewSelectedBeds.SelectedItem);
            GetAvaibleBedsWithEdited();
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

            foreach (Bed bed in listViewSelectedBeds.Items)
            {
                DAL.MakeBedUnavaible(bed);
            }
                        
            listViewAvaibleBeds.ItemsSource = DAL.GetAvaibleBeds(bookingFromDate, bookingToDate);
        }

        private void GetAvaibleBedsWithEdited()
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

            foreach (Bed bed in listViewSelectedBeds.Items)
            {
                DAL.MakeBedUnavaible(bed);
            }

            listViewAvaibleBeds.ItemsSource = DAL.GetAvaibleBeds(bookingFromDate, bookingToDate, EditedReservation.Id);
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

        private void DpDepartureDate_CalendarOpened(object sender, RoutedEventArgs e)
        {
            dpDepartureDate.DisplayDateStart = dpArrivalDate.SelectedDate.Value.AddDays(1);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            DAL.MakeAllBedAvaible();
        }

        private void DpDepartureDate_LostFocus(object sender, RoutedEventArgs e)
        {
            if (dpDepartureDate.SelectedDate <= dpArrivalDate.SelectedDate)
            {
                DateTime today = (DateTime)dpArrivalDate.SelectedDate;
                dpDepartureDate.SelectedDate = today.AddDays(1);
            }
        }
    }
}
