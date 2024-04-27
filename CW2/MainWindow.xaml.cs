using CW2.BusinessLogicLayer;
using System;
using System.Collections.Generic;
using System.Globalization;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CW2
{
   
    public partial class MainWindow : Window
    {

        private BookingSystem bookingSystem;

        public MainWindow()
        {
            InitializeComponent();

            string trainsFilePath = "C:\\Users\\neila\\Desktop\\OOSD Coursework 2\\Trains.csv";
            string bookingsFilePath = "C:\\Users\\neila\\Desktop\\OOSD Coursework 2\\Bookings.csv";
            this.bookingSystem = new BookingSystem(trainsFilePath, bookingsFilePath);
        }

        private void AddNewTrainButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // retrieve train details from UI
                string trainId = TrainIdTextBox.Text;
                string departureStation = DepartureStationTextBox.Text;
                string terminalStation = TerminalStationTextBox.Text;
                List<string> intermediateStations = IntermediateStationsTextBox.Text.Split(',').ToList();
                DateTime dateOfService = DateTime.ParseExact(DateOfServiceTextBox.Text, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                TimeSpan departureTime = TimeSpan.Parse(DepartureTimeTextBox.Text);
                int totalStandardSeats = int.Parse(TotalStandardSeatsTextBox.Text);
                int totalFirstClassSeats = int.Parse(TotalFirstClassSeatsTextBox.Text);

                // create new train object with retrieved details
                Train newTrain = new Train
                {
                    ID = trainId,
                    DepartureStation = departureStation,
                    TerminalStation = terminalStation,
                    IntermediateStations = intermediateStations,
                    DateOfService = dateOfService,
                    DepartureTime = departureTime,
                    TotalStandardSeats = totalStandardSeats,
                    TotalFirstClassSeats = totalFirstClassSeats
                    
                };

                // add new train to BookingSystem and save to CSV
                bookingSystem.AddNewTrain(newTrain);
            }
            catch (FormatException ex)
            {
                MessageBox.Show("Invalid input. Please check your input and try again.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                
                Console.WriteLine("FormatException occurred: " + ex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show("An unexpected error occurred: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                
                Console.WriteLine("Unexpected exception occurred: " + ex.Message);
            }
        }

        private void CreateBookingButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // get input data for the new booking
                string trainId = BookingTrainIdTextBox.Text;
                string departureStation = DepartureStationBookingTextBox.Text;
                string arrivalStation = ArrivalStationTextBox.Text;
                int numberOfTickets = int.Parse(NumberOfTicketsTextBox.Text);
                bool isNumberOfTicketsValid = int.TryParse(NumberOfTicketsTextBox.Text, out numberOfTickets);
                ClassType classType = FirstClassRadioButton.IsChecked == true ? ClassType.FirstClass : ClassType.StandardClass;

                if (!isNumberOfTicketsValid || numberOfTickets <= 0)
                {
                        MessageBox.Show("Invalid number of tickets. Please enter a positive integer.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                }

                List<int?> reservedSeatList = new List<int?>();
                if (!string.IsNullOrWhiteSpace(ReservedSeatTextBox.Text))
                    {
                        foreach (var seatText in ReservedSeatTextBox.Text.Split(','))
                        {
                            if (int.TryParse(seatText, out int seat))
                            {
                                if (seat < 1 || seat > 400)
                                {
                                    MessageBox.Show("Invalid seat number. Seat numbers must be between 1 and 400.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                                    return;
                                }
                                reservedSeatList.Add(seat);
                            }
                            else
                            {
                                reservedSeatList.Add(null);
                            }
                        }
                    }

                // total price calculation for the booking based on relevant properties
                decimal totalPrice = bookingSystem.CalculatePrice(classType, departureStation, arrivalStation, reservedSeatList);



                bool mealReservation = MealReservationCheckBox.IsChecked == true;



            

                // unique booking reference creation
                string bookingReference = bookingSystem.GenerateBookingReference();

                // new booking object creation
                Booking newBooking = new Booking
                {
                    TrainId = trainId,
                    DepartureStation = departureStation,
                    ArrivalStation = arrivalStation,
                    NumberOfTickets = numberOfTickets,
                    ClassType = classType,
                    ReservedSeat = reservedSeatList,
                    MealReservation = mealReservation,
                    BookingReference = bookingReference,
                    TotalPrice = totalPrice
                };



                // add new booking to the BookingSystem and save to CSV
                bookingSystem.CreateNewBooking(newBooking);

                // display the booking details in the UI
                BookingResultTextBlock.Text = $"Booking Reference: {newBooking.BookingReference}\n" +
                                              $"Total Price: £{newBooking.TotalPrice}\n" +
                                              $"Class Type: {newBooking.ClassType}\n" +
                                              $"Departure Station: {newBooking.DepartureStation}\n" +
                                              $"Arrival Station: {newBooking.ArrivalStation}\n" +
                                              $"Number of Tickets: {newBooking.NumberOfTickets}\n" +
                                              $"Reserved Seat: {(newBooking.ReservedSeat.Any() ? string.Join(", ", newBooking.ReservedSeat) : "N/A")}\n" +
                                              $"Meal Reservation: {(newBooking.MealReservation ? "Yes" : "No")}";

            }
            catch (FormatException ex)
            {
                MessageBox.Show("Invalid input. Please check your input and try again.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("An unexpected error occurred: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void GenerateManifestButton_Click(object sender, RoutedEventArgs e)
        {
            // retrieve train ID from UI
            string trainId = ManifestTrainIdTextBox.Text;

            // generate the manifest using the BookingSystem
            string manifest = bookingSystem.GenerateManifest(trainId);

            // update the TextBlock with the generated manifest
            ManifestTextBlock.Text = manifest;
        }

        private void TotalStandardSeatsTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }

}
