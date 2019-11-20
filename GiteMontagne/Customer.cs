using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GiteMontagne
{
    public class Customer
    {
        private int _id;
        private string _name;
        private string _firstName;
        private string _email;
        private string _phone;
        private double _price;

        public int Id { get => _id; set => _id = value; }
        public string Name { get => _name; set => _name = value; }
        public string FirstName { get => _firstName; set => _firstName = value; }
        public string Email { get => _email; set => _email = value; }
        public string Phone { get => _phone; set => _phone = value; }
        public double Price { get => _price; set => _price = value; }
    }
}
