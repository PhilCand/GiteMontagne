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
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void BtnEditer_Click(object sender, RoutedEventArgs e)
        {
            if (listViewReservations.SelectedIndex > 0)
            {
                EditReservationWindow erw = new EditReservationWindow((listViewReservations.SelectedItem as Reservation).Id);
                erw.ShowDialog();
                listViewReservations.ItemsSource = DAL.GetReservations();
            }            
        }

        private void TxtbSearchName_TextChanged(object sender, TextChangedEventArgs e)
        {
            SearchReservations();
        }

        private void DpSearchStart_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            SearchReservations();
        }

        private void DpSearchEnd_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            SearchReservations();
        }

        private void SearchReservations()
        {
            DateTime start = DateTime.Parse("01/01/2000");
            DateTime end = DateTime.Parse("31/12/3000");

            if (dpSearchStart.SelectedDate != null)
            {
                start = Convert.ToDateTime(dpSearchStart.Text);
            }

            if (dpSearchEnd.SelectedDate != null)
            {
                end = Convert.ToDateTime(dpSearchEnd.Text);
            }

            listViewReservations.ItemsSource = DAL.GetSearchedReservations(txtbSearchName.Text, start, end);
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

    }
}
