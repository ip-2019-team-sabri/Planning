using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace IP_Planning_Logger
{
    public class Logger
    {
        private static string _logTitle;

        private static Logger _instance;
        private static readonly object padlock = new object();

        private Logger() { }

        public static Logger Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (padlock)
                    {
                        if (_instance == null)
                        {
                            _logTitle = "Log_" + DateTime.Now.ToString("MM-dd-yy_H-mm-ss") + ".txt";

                            _instance = new Logger();

                            if (!Directory.Exists("Logs"))
                            {
                                Directory.CreateDirectory("Logs");
                            }
                        }
                    }
                }
                return _instance;
            }
        }

        public void Welcome()
        {
            string message = @"
###########################################################################
#  __  __                                  ____            _              #
# |  \/  | ___  ___ ___  __ _  __ _  ___  | __ ) _ __ ___ | | _____ _ __  #
# | |\/| |/ _ \/ __/ __|/ _` |/ _` |/ _ \ |  _ \| '__/ _ \| |/ / _ \ '__| #
# | |  | |  __/\__ \__ \ (_| | (_| |  __/ | |_) | | | (_) |   <  __/ |    #
# |_|  |_|\___||___/___/\__,_|\__, |\___| |____/|_|  \___/|_|\_\___|_|    #
#                             |___/                                       #
#                                                                         #
###########################################################################
";
            Console.WriteLine(message);
        }

        public void Log(string message, string severity)
        {
            using (StreamWriter _writer = File.AppendText("Logs/" + _logTitle))
            {
                DateTime time = DateTime.Now;
                string stime = time.ToString("hh:mm:ss.ff");

                if (severity == "info")
                {
                    string log = "[" + stime + "] INFO: " + message;

                    _writer.WriteLine(log);

                    Console.ForegroundColor = ConsoleColor.Gray;
                    Console.WriteLine(log);
                }
                else if (severity == "warning")
                {
                    string log = "[" + stime + "] WARNING: " + message;

                    _writer.WriteLine(log);

                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine(log);
                }
                else
                {
                    string log = "[" + stime + "] ERROR: " + message;

                    _writer.WriteLine(log);

                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(log);
                }
                Console.ForegroundColor = ConsoleColor.White;
            }
        }
    }
}
