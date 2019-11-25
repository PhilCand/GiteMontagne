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
            //listViewCustomers.ItemsSource = DAL.GetCustomers();
            listViewCustomers.ItemsSource = DAL.SearchCustomers(txtSearchCustomer.Text);
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

        private void BtnDeleteCustomer_Click(object sender, RoutedEventArgs e)
        {
            if (listViewCustomers.SelectedIndex >= 0)
            {            
            int delCustomerID = (listViewCustomers.SelectedItem as Customer).Id;
            DAL.DeleteCustomer(delCustomerID);
                //listViewCustomers.ItemsSource = DAL.GetCustomers();
                listViewCustomers.ItemsSource = DAL.SearchCustomers(txtSearchCustomer.Text);
            }
        }

        private void BtnEditCustomer_Click(object sender, RoutedEventArgs e)
        {
            if (listViewCustomers.SelectedIndex >= 0)
            {
                EditCustomerWindow ecw = new EditCustomerWindow(listViewCustomers.SelectedItem as Customer);
                ecw.ShowDialog();
                //listViewCustomers.ItemsSource = DAL.GetCustomers();
                listViewCustomers.ItemsSource = DAL.SearchCustomers(txtSearchCustomer.Text);
            }
        }

        private void BtnCreateCustomer_Click(object sender, RoutedEventArgs e)
        {
            Customer newCustomer = new Customer();
            newCustomer.Name = txtbNameCustomer.Text;
            newCustomer.FirstName = txtbFirstNameCustomer.Text;
            newCustomer.Phone = txtbPhoneCustomer.Text;
            newCustomer.Email = txtbEmailCustomer.Text;

            DAL.CreateCustomer(newCustomer);
            //listViewCustomers.ItemsSource = DAL.GetCustomers();
            listViewCustomers.ItemsSource = DAL.SearchCustomers(txtSearchCustomer.Text);

            txtbNameCustomer.Clear();
            txtbFirstNameCustomer.Clear();
            txtbPhoneCustomer.Clear();
            txtbEmailCustomer.Clear();

        }

        private void TxtSearchCustomer_TextChanged(object sender, TextChangedEventArgs e)
        {
            listViewCustomers.ItemsSource = DAL.SearchCustomers(txtSearchCustomer.Text);
        }

        private void ListViewCustomers_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            SelectedCustomer = (listViewCustomers.SelectedItem as Customer);
            this.Close();
        }
    }
}
