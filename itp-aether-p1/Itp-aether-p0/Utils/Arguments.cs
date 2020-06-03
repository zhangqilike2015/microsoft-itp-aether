using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Itp_aether_p1.Utils
{
    class Arguments
    {
        public static string Parse(string arguments)
        {
            try
            {
                string[] args = arguments.Split(new char[] { ',' });
                string ret = "";
                foreach (string arg in args)
                {
                    string[] tmp = arg.Split(new char[] { ':' });
                    ret += $"--{tmp[0]} {tmp[1]} ";
                }
                return ret;
            }
            catch (Exception e)
            {
                Logger.Error($"parse arguments error: {e.ToString()}");
                return "";
            }
        }
    }
}
