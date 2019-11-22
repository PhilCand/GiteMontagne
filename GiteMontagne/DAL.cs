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

        #region CUSTOMER

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

        public static void DeleteCustomer(int CustomerID)
        {
            string sqlQuery = String.Format($"delete from Customers where id = {CustomerID}");

            SqlCommand command = new SqlCommand(sqlQuery, connection);
            command.ExecuteNonQuery();

            command.Dispose();
        }

        public static void UpdateCustomer(Customer editedCustomer)
        {
            string updateQuery = $"Update Customers SET name = '{editedCustomer.Name}', firstName = '{editedCustomer.FirstName}', phone = '{editedCustomer.Phone}', email = '{editedCustomer.Email}' Where id = '{editedCustomer.Id}';";

            SqlCommand command = new SqlCommand(updateQuery, connection);
            command.ExecuteNonQuery();

            command.Dispose();
        }

        #endregion

        #region BED

        public static List<Bed> GetBeds()
        {
            List<Bed> beds = new List<Bed>();

            string sqlQuery = "SELECT r.name as roomName,r.id as roomID, b.id as numBed, b.price, b.status FROM Beds b JOIN Rooms r on b.room_id = r.id WHERE b.status = 1;";

            SqlCommand command = new SqlCommand(sqlQuery, connection);
            SqlDataReader dataReader = command.ExecuteReader();

            if (dataReader.HasRows)
            {
                while (dataReader.Read())
                {
                    Bed newBed = new Bed();
                    newBed.Id = Convert.ToInt32(dataReader["numBed"]);
                    newBed.RoomId = Convert.ToInt32(dataReader["roomID"]);
                    newBed.RoomName = dataReader["roomName"].ToString();
                    newBed.Price = Convert.ToDouble(dataReader["price"]);
                    newBed.Status = (bool)(dataReader["status"]);
                    beds.Add(newBed);
                }
            }
            dataReader.Close();
            command.Dispose();

            return beds;
        }

        public static List<Bed> GetAVaibleBeds(DateTime bookingFromDate, DateTime bookingToDate)
        {
            List<Bed> beds = new List<Bed>();

            string bookingFromDateStr = bookingFromDate.ToString("MM/dd/yyyy");
            string bookingToDateStr = bookingToDate.ToString("MM/dd/yyyy");

            string sqlQuery = "SELECT ro.name as roomName,ro.id as roomID, b.id as numBed, b.price, b.status " +
                              "from Beds b JOIN Rooms ro on ro.id = b.room_id " +
                              "WHERE b.id NOT IN(SELECT rb.bed_id from reservations r join Reservation_Beds rb on r.id = rb.reservation_id " +
                              $"WHERE(arrivalDate < '{bookingFromDateStr}' AND departureDate > '{bookingFromDateStr}') " +
                              $"OR(arrivalDate < '{bookingToDateStr}' AND departureDate >= '{bookingToDateStr}')" +
                              $"OR('{bookingFromDateStr}' < arrivalDate AND '{bookingToDateStr}' > arrivalDate));";

            SqlCommand command = new SqlCommand(sqlQuery, connection);
            SqlDataReader dataReader = command.ExecuteReader();

            if (dataReader.HasRows)
            {
                while (dataReader.Read())
                {
                    Bed newBed = new Bed();
                    newBed.Id = Convert.ToInt32(dataReader["numBed"]);
                    newBed.RoomId = Convert.ToInt32(dataReader["roomID"]);
                    newBed.RoomName = dataReader["roomName"].ToString();
                    newBed.Price = Convert.ToDouble(dataReader["price"]);
                    newBed.Status = (bool)(dataReader["status"]);
                    beds.Add(newBed);
                }
            }
            dataReader.Close();
            command.Dispose();

            return beds;
        }



        public static void MakeBedUnavaible(Bed selectedBed)
        {
            string updateQuery = $"Update Beds SET status = '0' Where id = '{selectedBed.Id}';";
            SqlCommand command = new SqlCommand(updateQuery, connection);
            command.ExecuteNonQuery();
            command.Dispose();
        }

        public static void MakeBedAvaible(Bed selectedBed)
        {
            string updateQuery = $"Update Beds SET status = '1' Where id = '{selectedBed.Id}';";
            SqlCommand command = new SqlCommand(updateQuery, connection);
            command.ExecuteNonQuery();
            command.Dispose();
        }

        public static void MakeAllBedAvaible()
        {
            string updateQuery = "update Beds SET status = '1';";
            SqlCommand command = new SqlCommand(updateQuery, connection);
            command.ExecuteNonQuery();
            command.Dispose();
        }

        #endregion

        #region RESERVATION

        public static List<Reservation> GetReservations()
        {
            List<Reservation> reservations = new List<Reservation>();
            string sqlQuery = String.Format("select r.id as resaID, r.arrivalDate, r.departureDate,r.comment, c.name from Reservations r join Customers c on c.id = r.customer_id;");
            SqlCommand command = new SqlCommand(sqlQuery, connection);
            SqlDataReader dataReader = command.ExecuteReader();

            if (dataReader.HasRows)
            {
                while (dataReader.Read())
                {
                    Reservation newReservation = new Reservation();
                    newReservation.Id = Convert.ToInt32(dataReader["resaID"]);
                    newReservation.CustomerName = dataReader["name"].ToString();
                    newReservation.ArrivalDate = Convert.ToDateTime(dataReader["arrivalDate"]);
                    newReservation.DepartureDate = Convert.ToDateTime(dataReader["departureDate"]);
                    newReservation.Comment = dataReader["comment"].ToString();
                    reservations.Add(newReservation);
                }
            }
            dataReader.Close();
            command.Dispose();

            return reservations;
        }

        public static void CreateReservation(Reservation newReservation)
        {
            string sqlQuery = ($"INSERT INTO Reservations (customer_id, arrivalDate, departureDate, comment) VALUES('{newReservation.Customer.Id}', '{newReservation.ArrivalDate.ToString("MM/dd/yyyy")}', '{newReservation.DepartureDate.ToString("MM/dd/yyyy")}', '{newReservation.Comment}');");


            foreach (Bed bed in newReservation.RevervedBeds)
            {
                sqlQuery += ($"INSERT INTO Reservation_beds (reservation_id, bed_id) VALUES((SELECT MAX(id) from Reservations), '{bed.Id}');");

            }
            SqlCommand command = new SqlCommand(sqlQuery, connection);
            command.ExecuteNonQuery();
            command.Dispose();
        }

        public static void DeleteReservation(int ReservationID)
        {
            string sqlQuery = String.Format($"delete from Reservations where id = {ReservationID}");
            SqlCommand command = new SqlCommand(sqlQuery, connection);
            command.ExecuteNonQuery();
            command.Dispose();
        }


        #endregion

    }
}
