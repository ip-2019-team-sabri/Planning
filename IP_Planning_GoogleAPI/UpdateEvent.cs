using System;
using System.Collections.Generic;
using System.Text;

namespace IP_Planning_GoogleAPI
{
    public class UpdateEvent
    {
        public UpdateEvent(string CalendarID, string EventID, string Summary, string Location, string Description, string TimeZone, 
            DateTime StartDatum, DateTime EindDatum, string SprekerNaam, string SprekerEmail)
        {
            this.CalendarID = CalendarID;
            this.EventID = EventID;
            this.Summary = Summary;
            this.Location = Location;
            this.Description = Description;
            this.TimeZone = TimeZone;
            this.StartDatum = StartDatum;
            this.EindDatum = EindDatum;
            this.SprekerNaam = SprekerNaam;
            this.SprekerEmail = SprekerEmail;
        }
        public string CalendarID { get; set; }

        public string  EventID { get; set; }

        public string Summary { get; set; }

        public string Location { get; set; }

        public string Description { get; set; }

        public string TimeZone { get; set; }

        public DateTime StartDatum { get; set; }

        public DateTime EindDatum { get; set; }

        public string SprekerNaam { get; set; }

        public string SprekerEmail { get; set; }
    }
}
