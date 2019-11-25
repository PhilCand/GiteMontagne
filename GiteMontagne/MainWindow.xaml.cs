﻿using System;
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

namespace GiteMontagne
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private Customer _selectedCustomer;

        public Customer SelectedCustomer { get => _selectedCustomer; set => _selectedCustomer = value; }

        public MainWindow()
        {
            InitializeComponent();
            dpArrivalDate.DisplayDateStart = DateTime.Now;
            dpDepartureDate.DisplayDateStart = DateTime.Now.AddDays(1);
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

        private void BtnCreateResevation_Click(object sender, RoutedEventArgs e)
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

            foreach (Bed bed in listViewSelectedBeds.Items)
            {               
                newReservation.RevervedBeds.Add(bed);
            }

            DAL.CreateReservation(newReservation);
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
            GetAvaibleBeds();
        }

        private void DpDepartureDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            GetAvaibleBeds();
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
    }
}
