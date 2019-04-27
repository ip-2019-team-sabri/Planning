using System;
using System.Collections.Generic;
using System.Text;

namespace IP_Planning_GoogleAPI
{
    public class CreateEvent
    {
        public string calendarID { get; set; }

        public string Summary { get; set; }

        public string Location { get; set; }

        public string  Description { get; set; }

        public string TimeZone { get; set; }

        public DateTime StartDatum { get; set; }

        public DateTime EindDatum { get; set; }

        public string SprekerNaam { get; set; }

        public string SprekerEmail { get; set; }
    }
}
