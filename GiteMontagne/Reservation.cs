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
        private int _customerId;
        private int _roomId;
        private int _bedId;
        private DateTime _arrivalDate;
        private DateTime _departureDate;
        private string _comment;

        public int Id { get => _id; set => _id = value; }
        public int CustomerId { get => _customerId; set => _customerId = value; }
        public int RoomId { get => _roomId; set => _roomId = value; }
        public int BedId { get => _bedId; set => _bedId = value; }
        public DateTime ArrivalDate { get => _arrivalDate; set => _arrivalDate = value; }
        public DateTime DepartureDate { get => _departureDate; set => _departureDate = value; }
        public string Comment { get => _comment; set => _comment = value; }
    }
}
