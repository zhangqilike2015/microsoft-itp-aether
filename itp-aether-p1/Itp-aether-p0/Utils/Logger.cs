using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Itp_aether_p1.Utils
{
    class Logger
    {
        public static void Error(string info)
        {
            Console.WriteLine($"Error: {info}");
            Environment.Exit(1);
        }

        public static void Warning(string info)
        {
            Console.WriteLine($"Warning: {info}");
        }

        public static void Info(string info)
        {
            Console.WriteLine($"Info: {info}");
        }
    }
}
