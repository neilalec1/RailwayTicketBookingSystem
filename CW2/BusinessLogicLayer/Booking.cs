using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CW2.BusinessLogicLayer
{
    public class Booking
    {
        public string? TrainId { get; set; }
        public string? DepartureStation { get; set; }
        public string? ArrivalStation { get; set; }
        public int NumberOfTickets { get; set; }
        public ClassType ClassType { get; set; }
        public bool MealReservation { get; set; }
        public string? BookingReference { get; set; }
        public decimal TotalPrice { get; set; }

        public List<int?>? ReservedSeat { get; set; }

        

    }
}
