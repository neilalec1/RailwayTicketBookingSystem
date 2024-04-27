using CW2.DataAccessLayer;
using CW2.BusinessLogicLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CW2.BusinessLogicLayer
{
    public class BookingSystem
    {
        private CsvDataAccess csvDataAccess;
        private List<Train> trains;
        private List<Booking> bookings;

        public BookingSystem(string trainsFilePath, string bookingsFilePath)
        {
            this.csvDataAccess = new CsvDataAccess(trainsFilePath, bookingsFilePath);
            this.trains = csvDataAccess.ReadTrainsData();
            this.bookings = csvDataAccess.ReadBookingsData();
        }

        public void AddNewTrain(Train train)
        {
            // add new train to the list
            trains.Add(train);

            // save updated list of trains to CSV file
            csvDataAccess.WriteTrainsData(trains);
        }

        public void CreateNewBooking(Booking booking)
        {
            // check if booking object is null
            if (booking == null)
            {
                Console.WriteLine("Error: Booking object is null.");
                return;
            }

            if (booking.TrainId == null)
            {
                // handle case when TrainId is null
                Console.WriteLine("Error: Train ID is not specified.");
                return;
            }

            // check if train with specified ID exists
            Train? train = trains.FirstOrDefault(t => t.ID == booking.TrainId);
            if (train == null)
            {
                // error handling
                Console.WriteLine("Error: Train with ID '" + booking.TrainId + "' does not exist.");
                return;
            }


            // check if departure and arrival stations are served by train
            if (!StopsAtStation(train, booking.DepartureStation) || !StopsAtStation(train, booking.ArrivalStation))
            {
                // error handling
                Console.WriteLine("Error: The specified train does not serve either the departure station or arrival station.");
                return;
            }

            // check that stations are not null before calling CalculatePrice
            string departureStation = booking.DepartureStation != null ? booking.DepartureStation.ToString() : string.Empty;
            string arrivalStation = booking.ArrivalStation != null ? booking.ArrivalStation.ToString() : string.Empty;

            // null checks for stations
            if (booking.DepartureStation == null)
            {
                Console.WriteLine("Error: Departure station is not specified.");
                return;
            }

            if (booking.ArrivalStation == null)
            {
                Console.WriteLine("Error: Arrival station is not specified.");
                return;
            }

            // check that reservedSeatList is not null before passing to CalculatePrice
            List<int?> reservedSeatList = booking.ReservedSeat ?? new List<int?>();

            // calculate total price for the booking based on relevant properties
            decimal totalPrice = CalculatePrice(booking.ClassType, departureStation, arrivalStation, reservedSeatList);


            // creation of unique booking reference
            string bookingReference = GenerateBookingReference();

            // booking reference and total price assigned to booking object
            booking.BookingReference = bookingReference;
            booking.TotalPrice = totalPrice;

            
            

            // add new booking to list of bookings
            bookings.Add(booking);

            // save updated list to CSV file
            csvDataAccess.WriteBookingsData(bookings);

            
        }

        private bool StopsAtStation(Train train, string? station)
        {
            // null-conditional to handle null station
            if (station == null)
            {
                // handle case when station is null
                return false;
            }

            // check if station is departure or terminal station
            if (station == train.DepartureStation || station == train.TerminalStation)
            {
                return true;
            }

            // check if station is an intermediate station
            return train.IntermediateStations != null && train.IntermediateStations.Contains(station);
        }

        public string GenerateBookingReference()
        {
            // unique booking refrence logic (random string)
            return Guid.NewGuid().ToString("N").Substring(0, 10).ToUpper();
        }

        public decimal CalculatePrice(ClassType classType, string? departureStation, string? arrivalStation, List<int?> reservedSeatList)
        {
            // standard seat costs
            Dictionary<string, decimal> standardSeatCosts = new Dictionary<string, decimal>
    {
        { "London|Edinburgh", 40m },
        { "London|York", 20m },
        { "London|Newcastle", 20m },
        { "Edinburgh|York", 20m },
        { "Edinburgh|Newcastle", 20m },
        { "York|Edinburgh", 20m },
        { "York|London", 20m },
        { "Newcastle|Edinburgh", 20m },
        { "Newcastle|London", 20m },
        { "Edinburgh|London", 40m }

    };

            // first class supplement and seat reservation cost
            decimal firstClassSupplement = 10m;
            decimal seatReservationCost = 5m;

            // base price calculation based on departure and arrival stations
            string key = $"{departureStation}|{arrivalStation}";
            decimal basePrice = standardSeatCosts.ContainsKey(key) ? standardSeatCosts[key] : 0m;

            // first class supplement if applicable
            if (classType == ClassType.FirstClass)
            {
                basePrice += firstClassSupplement;
            }

            // seat reservation cost if applicable
            if (reservedSeatList.Any())
            {
                basePrice += seatReservationCost;
            }

            return basePrice;
        }

        public string GenerateManifest(string trainId)
        {
            // find train with specified ID
            Train? train = trains.FirstOrDefault(t => t.ID == trainId);
            if (train == null)
            {
                // error handling
                return "Error: Train with ID '" + trainId + "' does not exist.";
            }

            // manifest header with train details
            string manifest = $"Train ID: {train.ID}\n" +
                              $"Departure Station: {train.DepartureStation}\n" +
                              $"Terminal Station: {train.TerminalStation}\n" +
                              $"Intermediate Stations: {string.Join(", ", train.IntermediateStations ?? Enumerable.Empty<string>())}\n" + // Use null-coalescing operator with Enumerable.Empty<string>()
                              $"Date of Service: {train.DateOfService:d}\n" +
                              $"Departure Time: {train.DepartureTime}\n\n";

            // find all bookings for specified train
            List<Booking> trainBookings = bookings.Where(b => b.TrainId == trainId).ToList();

            if (trainBookings.Count == 0)
            {
                // message for no bookings found
                manifest += "No bookings for this train.";
            }
            else
            {
                // table headers for bookings
                manifest += "Booking ID | No. of Tickets | Ticket Type | Stations | Seat Reservation | Meal Reservation\n";

                // booking details details added to manifest
                foreach (Booking booking in trainBookings)
                {
                    string ticketType = booking.ClassType == ClassType.StandardClass ? "Standard" : "First Class";
                    string stations = $"{booking.DepartureStation} -> {booking.ArrivalStation}";

                    // format the seat reservation (if available) and meal reservation
                    string seatReservation = booking.ReservedSeat != null && booking.ReservedSeat.Any()
                        ? string.Join(", ", booking.ReservedSeat.Select(seat => seat?.ToString() ?? "N/A"))
                        : "N/A"; 
                    string mealReservation = booking.MealReservation ? "Yes" : "No";

                    // add booking details to the manifest
                    manifest += $"{booking.BookingReference} | {booking.NumberOfTickets} | {ticketType} | {stations} | {seatReservation} | {mealReservation}\n";
                }
            }

            // return manifest as a string
            return manifest;
        }

    }

}
    