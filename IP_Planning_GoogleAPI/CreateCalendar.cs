using System;
using System.Collections.Generic;
using System.Text;

namespace IP_Planning_GoogleAPI
{
    public class CreateCalendar
    {
        public CreateCalendar(string Summary, string Description, string TimeZone)
        {
            this.Summary = Summary;
            this.Description = Description;
            this.TimeZone = TimeZone;
        }
        public string Summary { get; set; }

        public string Description { get; set; }

        public string TimeZone { get; set; }
    }
}
