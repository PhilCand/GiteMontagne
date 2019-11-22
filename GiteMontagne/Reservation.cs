using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GiteMontagne
{
    class Reservation
    {
        private int _id;
        private Customer _customer;
        private string _customerName;
        private DateTime _arrivalDate;
        private DateTime _departureDate;
        private string _comment;
        private List<Bed> _revervedBeds = new List<Bed>();

        public int Id { get => _id; set => _id = value; }
        public Customer Customer { get => _customer; set => _customer = value; }
        public DateTime ArrivalDate { get => _arrivalDate; set => _arrivalDate = value; }
        public DateTime DepartureDate { get => _departureDate; set => _departureDate = value; }
        public string Comment { get => _comment; set => _comment = value; }
        public string CustomerName { get => _customerName; set => _customerName = value; }
        internal List<Bed> RevervedBeds { get => _revervedBeds; set => _revervedBeds = value; }
    }
}
