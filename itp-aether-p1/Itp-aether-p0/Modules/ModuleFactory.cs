using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Itp_aether_p1.Modules
{
    class ModuleFactory
    {
        public static BaseModule CreateModule(Params p)
        {
            BaseModule module;
            if (p.ModuleType == "datastore")
            {
                module = new Datastore(p);
            }
            else if (p.ModuleType == "cmk8s")
            {
                module = new Cmk8s(p);
            }
            else if(p.ModuleType == "experiment")
            {
                module = new Experiment(p);
            } else
            {
                module = null;
            }
            return module;
        }
    }
}
