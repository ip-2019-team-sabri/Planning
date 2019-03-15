using System;
using System.Collections.Generic;
using System.Text;

namespace IP_Planning_Broker
{
    public class Logger
    {

        public void Log(string message, string severity)
        {
            if(severity == "info")
            {
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.WriteLine("INFO: " + message);
            }
            else if(severity == "warning")
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("WARNING: " + message);
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("ERROR: " + message);
            }
            Console.ForegroundColor = ConsoleColor.White;
        }
    }
}
