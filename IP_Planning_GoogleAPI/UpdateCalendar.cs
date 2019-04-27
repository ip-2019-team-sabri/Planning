using System;
using System.Collections.Generic;
using System.Text;

namespace IP_Planning_GoogleAPI
{
    public class UpdateCalendar
    {
        public string calendarID { get; set; }

        public string Summary { get; set; }

        public string Description { get; set; }

        public string TimeZone { get; set; }
    }
}
