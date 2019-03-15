using System;
using System.Collections.Generic;
using System.Text;

namespace IP_Planning_Broker
{
    public class Logger
    {

        public void Log(string message, string severity)
        {
            DateTime time = DateTime.Now;
            string stime = time.ToString("hh:mm:ss.ff");

            if(severity == "info")
            {
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.WriteLine("[" + stime + "] INFO: " + message);
            }
            else if(severity == "warning")
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("[" + stime + "] WARNING: " + message);
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("[" + stime + "] ERROR: " + message);
            }
            Console.ForegroundColor = ConsoleColor.White;
        }
    }
}
