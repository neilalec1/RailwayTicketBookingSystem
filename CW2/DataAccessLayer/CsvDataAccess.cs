using CsvHelper;
using CsvHelper.Configuration;
using CW2.BusinessLogicLayer;
using System;
using System.Collections.Generic;
using System.Formats.Asn1;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CW2.DataAccessLayer
{
    public class CsvDataAccess
    {
        private string trainsFilePath; // file path for trains CSV file
        private string bookingsFilePath; // file path for bookings CSV file

        public CsvDataAccess(string trainsFilePath, string bookingsFilePath)
        {
            this.trainsFilePath = trainsFilePath;
            this.bookingsFilePath = bookingsFilePath;
        }

        public void WriteBookingsData(List<Booking> bookings)
        {
            try
            {
                using (var writer = new StreamWriter(bookingsFilePath))
                using (var csv = new CsvWriter(writer, new CsvConfiguration(CultureInfo.InvariantCulture)))
                {
                    // write header row for CSV fields
                    csv.WriteHeader<Booking>();
                    csv.NextRecord();

                    foreach (var booking in bookings)
                    {
                        // write record to CSV file 
                        csv.WriteRecord(booking);
                        csv.NextRecord();
                    }
                }
            }
            catch (IOException ex)
            {
                // handle io exceptions
                Console.WriteLine("Error: Unable to write bookings data: " + ex.Message);
            }
            catch (Exception ex)
            {
                // handle any other exceptions
                Console.WriteLine("Error writing bookings data: " + ex.Message);
            }
        }

        public void WriteTrainsData(List<Train> trains)
        {
            try
            {
                using (var writer = new StreamWriter(trainsFilePath))
                using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                {
                    // write header row 
                    csv.WriteField("ID");
                    csv.WriteField("DepartureStation");
                    csv.WriteField("TerminalStation");
                    
                    csv.NextRecord();

                    foreach (var train in trains)
                    {
                        // write record to CSV file
                        csv.WriteRecord(train);
                        csv.NextRecord();
                    }
                }
            }
            catch (Exception ex)
            {
                // handle exceptions
                Console.WriteLine("Error writing trains data: " + ex.Message);
            }
        }

        public List<Booking> ReadBookingsData()
        {
            List<Booking> bookings = new List<Booking>();

            try
            {
                if (File.Exists(bookingsFilePath))
                {
                    using (var reader = new StreamReader(bookingsFilePath))
                    {
                        // skip header row if already present
                        reader.ReadLine();

                        while (!reader.EndOfStream)
                        {
                            var line = reader.ReadLine();
                            var values = line?.Split(',');

                            if (values != null && values.Length >= 7)
                            {
                                // create new booking object
                                Booking booking = new Booking
                                {
                                    TrainId = values[0],
                                    DepartureStation = values[1],
                                    ArrivalStation = values[2],
                                    NumberOfTickets = int.Parse(values[3]),
                                    ClassType = (ClassType)Enum.Parse(typeof(ClassType), values[4]),
                                    ReservedSeat = values[5].Split(',').Select(s => int.TryParse(s, out int seat) ? (int?)seat : null).ToList<int?>(),
                                    MealReservation = bool.Parse(values[6])
                                };

                                bookings.Add(booking);
                            }
                            else
                            {
                                // handle case when CSV doesn't contain enough values
                                Console.WriteLine("Error: Invalid CSV line format for booking data.");
                            }
                        }
                    }
                }
                else
                {
                    // handle case when bookings file does not exist
                    Console.WriteLine("Error: Bookings file not found.");
                }
            }
            catch (FileNotFoundException ex)
            {
                // handle case when bookings file is not found
                Console.WriteLine("Error: Bookings file not found: " + ex.Message);
            }
            catch (IOException ex)
            {
                // handle io exceptions 
                Console.WriteLine("Error: Unable to read bookings data: " + ex.Message);
            }
            catch (Exception ex)
            {
                // handle any other exceptions
                Console.WriteLine("Error reading bookings data: " + ex.Message);
            }

            return bookings;
        }

        public List<Train> ReadTrainsData()
        {
            List<Train> trains = new List<Train>();

            try
            {
                using (var reader = new StreamReader(trainsFilePath))
                {
                    // skip header row if already present 
                    reader.ReadLine();

                    while (!reader.EndOfStream)
                    {
                        var line = reader.ReadLine();
                        var values = line?.Split(',');

                        if (values != null && values.Length >= 7)
                        {
                            // create new train object
                            Train train = new Train
                            {
                                ID = values[0],
                                DepartureStation = values[1],
                                TerminalStation = values[2],
                                DateOfService = DateTime.ParseExact(values[3], "dd/MM/yyyy", CultureInfo.InvariantCulture),
                                DepartureTime = TimeSpan.Parse(values[4]),
                                TotalStandardSeats = int.Parse(values[5]),
                                TotalFirstClassSeats = int.Parse(values[6])
                                
                            };

                            trains.Add(train);
                        }
                        else
                        {
                            // handle case when CSV doesn't contain enough values
                            Console.WriteLine("Error: Invalid CSV line format for train data.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // handle exceptinos
                Console.WriteLine("Error reading trains data: " + ex.Message);
            }

            // populate trains list
            return trains;
        }

    }

}
