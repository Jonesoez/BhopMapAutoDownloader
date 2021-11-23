using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BhopMapAutoDownloader.Services
{
    public class LoggerService
    {
        public enum LogType
        {
            INFO = 0,
            ERROR = 1,
            DONE = 2,
            EXCEPTION = 3
        }

        //will remove this or use something else since i made this just for my own
        public static void Log(string message, LogType logtype = 0)
        {
            switch((int)logtype)
            {
                case 0:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.Write("[INFO] \t");
                    Console.WriteLine(message);
                    Console.ResetColor();
                    break;
                case 1:
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write("[Error] \t");
                    Console.ResetColor();
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine(message);
                    Console.ResetColor();
                    break;
                case 2:
                    Console.ForegroundColor = ConsoleColor.DarkGreen;
                    Console.Write("[DONE] \t");
                    Console.WriteLine(message);
                    Console.ResetColor();
                    break;
                case 3:
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write("[EXCEPTION] \t");
                    Console.ResetColor();
                    Console.WriteLine(message);
                    break;
            }
        }
    }
}
