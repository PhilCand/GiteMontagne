using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GiteMontagne
{
    class Bed
    {
        private int _id;
        private int _roomId;
        private double _price;
        private bool _status;
        private string _roomName;

        public int Id { get => _id; set => _id = value; }
        public int RoomId { get => _roomId; set => _roomId = value; }
        public double Price { get => _price; set => _price = value; }
        public bool Status { get => _status; set => _status = value; }
        public string RoomName { get => _roomName; set => _roomName = value; }
    }
}
