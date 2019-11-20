using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GiteMontagne
{
    class DAL
    {
        private static SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["MyConnectionTestString"].ConnectionString);
        //private static SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["MyConnectionProdString"].ConnectionString);

        static DAL()
        {
            connection.Open();
        }

        public static List<Customer> GetCustomers()
        {
            List<Customer> customers = new List<Customer>();

            string sqlQuery = "SELECT * FROM Customers";
            SqlCommand command = new SqlCommand(sqlQuery, connection);
            SqlDataReader dataReader = command.ExecuteReader();

            if (dataReader.HasRows)
            {
                while (dataReader.Read())
                {
                    Customer newCustomer = new Customer();
                    newCustomer.Name = dataReader["name"].ToString();
                    newCustomer.FirstName = dataReader["firstName"].ToString();
                    newCustomer.Email = dataReader["email"].ToString();
                    newCustomer.Phone = dataReader["phone"].ToString();
                    newCustomer.Id = Convert.ToInt32(dataReader["id"]);
                    customers.Add(newCustomer);
                }
            }
            dataReader.Close();
            command.Dispose();

            return customers;
        }

        public static void CreateCustomer(Customer newCustomer)
        {
            string sqlQuery = ($"INSERT INTO Customers (name, firstName, email, phone) VALUES('{newCustomer.Name}', '{newCustomer.FirstName}', '{newCustomer.Email}', '{newCustomer.Phone}')");

            SqlCommand command = new SqlCommand(sqlQuery, connection);

            command.ExecuteNonQuery();
            command.Dispose();

        }


    }
}
