using System;
using System.Collections.Generic;
using System.Text;

namespace IP_Planning_GoogleAPI
{
    public class CreateCalendar
    {
        public CreateCalendar(string Summary, string Description)
        {
            this.Summary = Summary;
            this.Description = Description;
            this.TimeZone = "Europe/Brussels";
        }
        public string Summary { get; set; }

        public string Description { get; set; }

        public string TimeZone { get; set; }
    }
}
