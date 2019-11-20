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
using System.Windows.Shapes;

namespace GiteMontagne
{
    /// <summary>
    /// Logique d'interaction pour CustomerWindow.xaml
    /// </summary>
    public partial class CustomerWindow : Window
    {
        private Customer _selectedCustomer;

        internal Customer SelectedCustomer { get => _selectedCustomer; set => _selectedCustomer = value; }

        public CustomerWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            listViewCustomers.ItemsSource = DAL.GetCustomers();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Customer newCustomer = new Customer();
            newCustomer.Name = txtbNameCustomer.Text;
            newCustomer.FirstName = txtbFirstNameCustomer.Text;
            newCustomer.Phone = txtbPhoneCustomer.Text;
            newCustomer.Email = txtbEmailCustomer.Text;

            DAL.CreateCustomer(newCustomer);
            listViewCustomers.ItemsSource = DAL.GetCustomers();

            txtbNameCustomer.Clear();
            txtbFirstNameCustomer.Clear();
            txtbPhoneCustomer.Clear();
            txtbEmailCustomer.Clear();

        }

        private void BtnSelectCustomer_Click(object sender, RoutedEventArgs e)
        {
            SelectedCustomer = (listViewCustomers.SelectedItem as Customer);
            this.Close();

        }

        public Customer ShowDialogWithResult()
        {
        ShowDialog();
        return SelectedCustomer;
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}