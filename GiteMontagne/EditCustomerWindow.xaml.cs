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
    /// Logique d'interaction pour EditCustomerWindow.xaml
    /// </summary>
    public partial class EditCustomerWindow : Window
    {

        private Customer _editedCustomer;

        public Customer EditedCustomer { get => _editedCustomer; set => _editedCustomer = value; }


        public EditCustomerWindow()
        {
            InitializeComponent();
        }

        public EditCustomerWindow(Customer editedCustomer)
        {
            InitializeComponent();
            EditedCustomer = editedCustomer;
            txtbEditNameCustomer.Text = EditedCustomer.Name;
            txtbEditFirstNameCustomer.Text = EditedCustomer.FirstName;
            txtbEditEmailCustomer.Text = EditedCustomer.Email;
            txtbEditPhoneCustomer.Text = EditedCustomer.Phone;
        }

        private void BtnEditCustomer_Click(object sender, RoutedEventArgs e)
        {
            EditedCustomer.Name = txtbEditNameCustomer.Text;
            EditedCustomer.FirstName = txtbEditFirstNameCustomer.Text;
            EditedCustomer.Email = txtbEditEmailCustomer.Text;
            EditedCustomer.Phone = txtbEditPhoneCustomer.Text;

            DAL.UpdateCustomer(EditedCustomer);
            this.Close();
        }

        private void BtnCloseEdit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
