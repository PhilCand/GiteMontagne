using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GiteMontagne
{
    class Room
    {
        private int _id;
        private string _name;
        private int _capacity;
        private bool _status;

        public int Id { get => _id; set => _id = value; }
        public string Name { get => _name; set => _name = value; }
        public int Capacity { get => _capacity; set => _capacity = value; }
        public bool Status { get => _status; set => _status = value; }
    }
}
