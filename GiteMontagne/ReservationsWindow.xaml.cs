using System;
using System.Collections.Generic;
using System.ComponentModel;
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

            if (listViewReservations.Items.Count > 0)
            {
                listViewReservations.SelectedIndex = 0;
            }
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void BtnEditer_Click(object sender, RoutedEventArgs e)
        {
            if (listViewReservations.SelectedIndex >= 0)
            {
                EditReservationWindow erw = new EditReservationWindow((listViewReservations.SelectedItem as Reservation).Id);
                erw.ShowDialog();
                listViewReservations.ItemsSource = DAL.GetReservations();
            }            
        }

        private void ListViewReservations_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (listViewReservations.SelectedIndex < 0) return;

            EditReservationWindow erw = new EditReservationWindow((listViewReservations.SelectedItem as Reservation).Id);
            erw.ShowDialog();
            listViewReservations.ItemsSource = DAL.GetReservations();
        }
        
        //Sorting listview

        GridViewColumnHeader _lastHeaderClicked = null;

        ListSortDirection _lastDirection = ListSortDirection.Ascending;

        void GridViewColumnHeaderClickedHandler(object sender, RoutedEventArgs e)
        {
            var headerClicked = e.OriginalSource as GridViewColumnHeader;
            ListSortDirection direction;

            if (headerClicked != null)
            {
                if (headerClicked.Role != GridViewColumnHeaderRole.Padding)
                {
                    if (headerClicked != _lastHeaderClicked)
                    {
                        direction = ListSortDirection.Ascending;
                    }
                    else
                    {
                        if (_lastDirection == ListSortDirection.Ascending)
                        {
                            direction = ListSortDirection.Descending;
                        }
                        else
                        {
                            direction = ListSortDirection.Ascending;
                        }
                    }

                    var columnBinding = headerClicked.Column.DisplayMemberBinding as Binding;
                    var sortBy = columnBinding?.Path.Path ?? headerClicked.Column.Header as string;

                    Sort(sortBy, direction);

                    if (direction == ListSortDirection.Ascending)
                    {
                        headerClicked.Column.HeaderTemplate =
                          Resources["HeaderTemplateArrowUp"] as DataTemplate;
                    }
                    else
                    {
                        headerClicked.Column.HeaderTemplate =
                          Resources["HeaderTemplateArrowDown"] as DataTemplate;
                    }

                    // Remove arrow from previously sorted header
                    if (_lastHeaderClicked != null && _lastHeaderClicked != headerClicked)
                    {
                        _lastHeaderClicked.Column.HeaderTemplate = null;
                    }

                    _lastHeaderClicked = headerClicked;
                    _lastDirection = direction;
                }
            }
        }

        private void Sort(string sortBy, ListSortDirection direction)
        {
            ICollectionView dataView =
              CollectionViewSource.GetDefaultView(listViewReservations.ItemsSource);

            dataView.SortDescriptions.Clear();
            SortDescription sd = new SortDescription(sortBy, direction);
            dataView.SortDescriptions.Add(sd);
            dataView.Refresh();
        }

        //Searching reservations

        private void SearchReservations()
        {
            DateTime startDate = DateTime.Parse("01/01/2000");
            DateTime endDate = DateTime.Parse("31/12/3000");

            if (dpSearchStart.SelectedDate != null)
            {
                startDate = Convert.ToDateTime(dpSearchStart.SelectedDate);
            }

            if (dpSearchEnd.SelectedDate != null)
            {
                endDate = Convert.ToDateTime(dpSearchEnd.SelectedDate);
            }

            dpSearchIn.Text = "";
            listViewReservations.ItemsSource = DAL.GetSearchedReservations(txtbSearchName.Text, startDate, endDate);

        }

        private void SearchReservationsInDate()
        {
            if (dpSearchIn.SelectedDate != null)
            {
                dpSearchStart.Text = "";
                dpSearchEnd.Text = "";
                txtbSearchName.Text = "";
                DateTime inDate = Convert.ToDateTime(dpSearchIn.SelectedDate);
                listViewReservations.ItemsSource = DAL.GetSearchedReservationsInDate(inDate);

            }

        }

        private void TxtbSearchName_KeyUp(object sender, KeyEventArgs e)
        {
            SearchReservations();
        }

        private void DpSearchEnd_LostFocus(object sender, RoutedEventArgs e)
        {
            SearchReservations();
        }

        private void DpSearchStart_LostFocus(object sender, RoutedEventArgs e)
        {
            SearchReservations();
        }

        private void DpSearchIn_LostFocus(object sender, RoutedEventArgs e)
        {
            SearchReservationsInDate();
        }

        private void DpSearchEnd_CalendarOpened(object sender, RoutedEventArgs e)
        {
            dpSearchEnd.DisplayDateStart = dpSearchStart.SelectedDate.Value.AddDays(1);
        }

        
    }
}
