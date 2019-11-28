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

        public static List<Customer> SearchCustomers(string search)
        {
            List<Customer> customers = new List<Customer>();

            string sqlQuery = $"SELECT * FROM Customers " +
                              $"WHERE name LIKE '%{search}%' " +
                              $"OR (firstName LIKE '%{search}%') " +
                              $"OR (email LIKE '%{search}%') " +
                              $"OR (phone LIKE '%{search}%');";

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

        public static List<Bed> GetAvaibleBeds(DateTime bookingFromDate, DateTime bookingToDate)
        {
            List<Bed> beds = new List<Bed>();

            string bookingFromDateStr = bookingFromDate.ToString("MM/dd/yyyy");
            string bookingToDateStr = bookingToDate.ToString("MM/dd/yyyy");

            string sqlQuery = "SELECT ro.name as roomName,ro.id as roomID, b.id as numBed, b.price, b.status " +
                              "from Beds b JOIN Rooms ro on ro.id = b.room_id " +
                              "WHERE b.id NOT IN((SELECT rb.bed_id from reservations r join Reservation_Beds rb on r.id = rb.reservation_id " +
                              $"WHERE(arrivalDate <= '{bookingFromDateStr}' AND departureDate > '{bookingFromDateStr}') " +
                              $"OR(arrivalDate < '{bookingToDateStr}' AND departureDate >= '{bookingToDateStr}')" +
                              $"OR('{bookingFromDateStr}' < arrivalDate AND '{bookingToDateStr}' > arrivalDate)))" +
                              $"AND b.status = 1;";

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

        public static List<Bed> GetAvaibleBeds(DateTime bookingFromDate, DateTime bookingToDate, int excludedReservationID)
        {
            List<Bed> beds = new List<Bed>();

            string bookingFromDateStr = bookingFromDate.ToString("MM/dd/yyyy");
            string bookingToDateStr = bookingToDate.ToString("MM/dd/yyyy");

            string sqlQuery = "SELECT ro.name as roomName,ro.id as roomID, b.id as numBed, b.price, b.status " +
                              "from Beds b JOIN Rooms ro on ro.id = b.room_id " +
                              "WHERE b.status = 1" +
                              "AND b.id NOT IN(SELECT rb.bed_id from reservations r join Reservation_Beds rb on r.id = rb.reservation_id " +
                              $"WHERE((arrivalDate <= '{bookingFromDateStr}' AND departureDate > '{bookingFromDateStr}') " +
                              $"OR(arrivalDate < '{bookingToDateStr}' AND departureDate >= '{bookingToDateStr}') " +
                              $"OR('{bookingFromDateStr}' < arrivalDate AND '{bookingToDateStr}' > arrivalDate)) " +
                              $"AND(r.id <> {excludedReservationID}));";

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
            string sqlQuery = String.Format("select r.id as resaID, r.arrivalDate, r.departureDate,r.comment, c.name, r.price, r.numberOFBeds " +
                                            "from Reservations r join Customers c on c.id = r.customer_id;");
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
                    newReservation.Price = Convert.ToDouble(dataReader["price"]);
                    newReservation.NumberOfBeds = Convert.ToInt32(dataReader["numberOfBeds"]);
                    reservations.Add(newReservation);
                }
            }
            dataReader.Close();
            command.Dispose();

            return reservations;
        }

        public static List<Reservation> GetSearchedReservations(string name, DateTime startDate, DateTime endDate)
        {
            List<Reservation> reservations = new List<Reservation>();

            string startDateStr = startDate.ToString("MM/dd/yyyy");
            string endDateStr = endDate.ToString("MM/dd/yyyy");

            string sqlQuery = $"select r.id as resaID, r.arrivalDate, r.departureDate,r.comment, c.name, r.price, r.numberOFBeds " +
                              $"from Reservations r join Customers c on c.id = r.customer_id " +
                              $"where name like '%{name}%' " +
                              $"and arrivalDate >= '{startDateStr}' " +
                              $"and departureDate <= '{endDateStr}';";

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
                    newReservation.Price = Convert.ToDouble(dataReader["price"]);
                    newReservation.NumberOfBeds = Convert.ToInt32(dataReader["numberOfBeds"]);
                    reservations.Add(newReservation);
                }
            }
            dataReader.Close();
            command.Dispose();

            return reservations;
        }

        public static List<Reservation> GetSearchedReservationsInDate(DateTime inDate)
        {
            List<Reservation> reservations = new List<Reservation>();

            string inDateStr = inDate.ToString("MM/dd/yyyy");

            string sqlQuery = $"select r.id as resaID, r.arrivalDate, r.departureDate,r.comment, c.name, r.price, r.numberOFBeds " +
                              $"from Reservations r join Customers c on c.id = r.customer_id " +
                              $"where '{inDateStr}' BETWEEN arrivalDate AND departureDate;";

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
                    newReservation.Price = Convert.ToDouble(dataReader["price"]);
                    newReservation.NumberOfBeds = Convert.ToInt32(dataReader["numberOfBeds"]);
                    reservations.Add(newReservation);
                }
            }
            dataReader.Close();
            command.Dispose();

            return reservations;
        }

        public static Reservation GetUpdatedReservation(int editedReservationID)
        {

            string sqlQuery = ($"SELECT r.id as reservation_id, r.customer_id, r.arrivalDate, r.departureDate, r.comment, " +
                               $"rb.bed_id, c.name, b.room_id, b.price, ro.name as roomName, c.email, c.firstName, c.phone " +
                               $"FROM Reservations r " +
                               $"JOIN Reservation_Beds rb on r.id = rb.reservation_id " +
                               $"JOIN Customers c on c.id = r.customer_id " +
                               $"JOIN Beds b on b.id = rb.bed_id " +
                               $"JOIN Rooms ro on ro.id = b.room_id " +
                               $"WHERE r.id = {editedReservationID};");

            SqlCommand command = new SqlCommand(sqlQuery, connection);
            SqlDataReader dataReader = command.ExecuteReader();

            Reservation editedReservation = new Reservation();
            Customer newCustomer = new Customer();

            if (dataReader.HasRows)
            {
                while (dataReader.Read())
                {
                    if (editedReservation.Customer == null)
                    {
                        editedReservation.Id = Convert.ToInt32(dataReader["reservation_id"]);
                        editedReservation.CustomerName = dataReader["name"].ToString();
                        editedReservation.CustomerID = Convert.ToInt32(dataReader["customer_id"]);
                        editedReservation.ArrivalDate = Convert.ToDateTime(dataReader["arrivalDate"]);
                        editedReservation.DepartureDate = Convert.ToDateTime(dataReader["departureDate"]);
                        editedReservation.Comment = dataReader["comment"].ToString();
                        newCustomer.Id = Convert.ToInt32(dataReader["customer_id"]);
                        newCustomer.Name = dataReader["name"].ToString();
                        newCustomer.Email = dataReader["email"].ToString();
                        newCustomer.Phone = dataReader["phone"].ToString();
                        newCustomer.FirstName = dataReader["firstName"].ToString();
                        editedReservation.Customer = newCustomer;
                    }
                    Bed newBed = new Bed();
                    newBed.Id = Convert.ToInt32(dataReader["bed_id"]);
                    newBed.RoomId = Convert.ToInt32(dataReader["room_id"]);
                    newBed.RoomName = dataReader["roomName"].ToString();
                    newBed.Status = false;
                    newBed.Price = Convert.ToDouble(dataReader["price"]);

                    editedReservation.RevervedBeds.Add(newBed);              

                }
            }
            dataReader.Close();
            command.Dispose();

            return editedReservation;
        }

        public static void CreateReservation(Reservation newReservation)
        {
            string sqlQuery = $"INSERT INTO Reservations (customer_id, arrivalDate, departureDate, comment, price, numberOfBeds)" +
                              $"VALUES('{newReservation.Customer.Id}'," +
                              $"'{newReservation.ArrivalDate.ToString("MM/dd/yyyy")}'," +
                              $"'{newReservation.DepartureDate.ToString("MM/dd/yyyy")}'," +
                              $"'{newReservation.Comment}'," +
                              $"'{newReservation.Price}'," +
                              $"'{newReservation.NumberOfBeds}');";


            foreach (Bed bed in newReservation.RevervedBeds)
            {
                sqlQuery += ($"INSERT INTO Reservation_beds (reservation_id, bed_id) VALUES((SELECT MAX(id) from Reservations), '{bed.Id}');");

            }
            SqlCommand command = new SqlCommand(sqlQuery, connection);
            command.ExecuteNonQuery();
            command.Dispose();
        }

        public static void UpdateReservation(Reservation EditedReservation)
        {
            string sqlQuery = $"UPDATE Reservations SET customer_id='{EditedReservation.Customer.Id}', " +
                $"arrivalDate='{EditedReservation.ArrivalDate.ToString("MM/dd/yyyy")}', " +
                $"departureDate='{EditedReservation.DepartureDate.ToString("MM/dd/yyyy")}', " +
                $"comment='{EditedReservation.Comment}', price={EditedReservation.Price}, numberOfBeds={EditedReservation.NumberOfBeds} " +
                $"WHERE id = '{EditedReservation.Id}' ;";

            sqlQuery += $"DELETE from Reservation_beds where reservation_id = '{EditedReservation.Id}';";

            foreach (Bed bed in EditedReservation.RevervedBeds)
            {
                sqlQuery += $"INSERT INTO Reservation_beds (reservation_id, bed_id) VALUES('{EditedReservation.Id}', '{bed.Id}');";
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
