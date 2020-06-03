using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Itp_aether_p1.Modules
{
    class Datastore : BaseModule
    {
        [JsonProperty("dataStoreName", NullValueHandling = NullValueHandling.Ignore)]
        public string DataStoreName { get; set; }

        [JsonProperty("mode", NullValueHandling = NullValueHandling.Ignore)]
        public string Mode { get; set; }

        [JsonProperty("overwrite", NullValueHandling = NullValueHandling.Ignore)]
        public bool Overwrite { get; set; }

        [JsonProperty("pathOnCompute", NullValueHandling = NullValueHandling.Ignore)]
        public object PathOnCompute { get; set; }

        [JsonProperty("pathOnDataStore", NullValueHandling = NullValueHandling.Ignore)]
        public string PathOnDataStore { get; set; }

        public string OutputFile { get; set; }

        public Datastore(Params p)
        {
            //hardcode
            this.Mode = "mount";
            this.Overwrite = false;
            this.PathOnCompute = null;
            this.DataStoreName = p.DatastoreName;
            this.PathOnDataStore = p.PathOnDataStore;
            this.OutputFile = p.OutputFile;
        }
        public void Exec(Params p)
        {
            Utils.Logger.Info("start running dsmodule");
            string OutputStr = JsonConvert.SerializeObject(this);
            Utils.Logger.Info($"ds output is : {OutputStr}");
            File.WriteAllText(p.OutputFile, OutputStr);
            Utils.Logger.Info("ds module finished");
        }
    }
}
