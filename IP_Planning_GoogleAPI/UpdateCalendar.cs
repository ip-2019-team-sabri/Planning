using System;
using System.Collections.Generic;
using System.Text;

namespace IP_Planning_GoogleAPI
{
    public class UpdateCalendar
    {
        public UpdateCalendar(string CalendarID, string Summary, string Description)
        {
            this.CalendarID = CalendarID;
            this.Summary = Summary;
            this.Description = Description;
            this.TimeZone = "Europe/Brussels";
        }
        public string CalendarID { get; set; }

        public string Summary { get; set; }

        public string Description { get; set; }

        public string TimeZone { get; set; }
    }
}
