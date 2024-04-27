using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CW2.BusinessLogicLayer
{
    public class Train
    {
        public string? ID { get; set; }
        public string? DepartureStation { get; set; }
        public string? TerminalStation { get; set; }
        public List<string>? IntermediateStations { get; set; }
        public DateTime DateOfService { get; set; }
        public TimeSpan DepartureTime { get; set; }
        public int TotalStandardSeats { get; set; }
        public int TotalFirstClassSeats { get; set; }

        
        public override string ToString()
        {
            
            return $"Train {ID}: {DepartureStation} to {TerminalStation} on {DateOfService:dd/MM/yyyy} at {DepartureTime:hh\\:mm}";
        }

    }
}
