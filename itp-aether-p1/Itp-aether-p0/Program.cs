using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using CommandLine;

namespace Itp_aether_p1
{
    class Program
    {
        static int Main(string[] args)
        {
            Parser.Default.ParseArguments<Params>(args)
                   .WithParsed<Params>(p =>
                   {
                       Modules.BaseModule module = Modules.ModuleFactory.CreateModule(p);
                       if (module == null)
                       {
                           Console.WriteLine("Error: Invalid module type!");
                           Environment.Exit(1);
                       }
                       module.Exec(p);
                   });
            return 0;
        }
    }
}
