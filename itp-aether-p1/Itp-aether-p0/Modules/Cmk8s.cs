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
    class Cmk8s : BaseModule
    {

        [JsonProperty("enable_ipython", NullValueHandling = NullValueHandling.Ignore)]
        public bool EnableIpython { get; set; }

        [JsonProperty("enable_ssh", NullValueHandling = NullValueHandling.Ignore)]
        public bool EnableSsh { get; set; }

        [JsonProperty("enable_tensorboard", NullValueHandling = NullValueHandling.Ignore)]
        public bool EnableTensorboard { get; set; }

        [JsonProperty("gpu_count", NullValueHandling = NullValueHandling.Ignore)]
        public int? GpuCount { get; set; }

        [JsonProperty("preemption_allowed", NullValueHandling = NullValueHandling.Ignore)]
        public bool PreemptionAllowed { get; set; }

        [JsonProperty("deepspeedConfig", NullValueHandling = NullValueHandling.Ignore)]
        public string DeepspeedConfig { get; set; }

        [JsonProperty("interactive_port", NullValueHandling = NullValueHandling.Ignore)]
        public int? InteractivePort { get; set; }

        [JsonProperty("ssh_public_key", NullValueHandling = NullValueHandling.Ignore)]
        public string SshPublicKey { get; set; }

        public Cmk8s(Params p)
        {
            this.EnableIpython = p.EnableIphthon == "true" ? true : false;
            this.EnableSsh = p.EnableSsh == "true" ? true : false;
            this.EnableTensorboard = p.EnableTensorboard == "true" ? true : false;
            this.PreemptionAllowed = p.PreemptionAllowed == "true" ? true : false;
            if (p.GpuCount != "" && p.GpuCount != "0")
            {
                this.GpuCount = Convert.ToInt32(p.GpuCount);
            }
            if (p.DeepspeedConfig != "")
            {
                this.DeepspeedConfig = p.DeepspeedConfig;
            }
            if (p.SshPublicKey != "")
            {
                this.SshPublicKey = p.SshPublicKey;
            }
            if (p.InteractivePort != "" && Convert.ToInt32(p.InteractivePort) >= 40000)
            {
                this.InteractivePort = Convert.ToInt32(p.InteractivePort);
            }
        }

        public void Exec(Params p)
        {
            Utils.Logger.Info("start running cmk8s module");
            string OutputStr = JsonConvert.SerializeObject(this);
            Utils.Logger.Info($"cmk8s output is : {OutputStr}");
            File.WriteAllText(p.OutputFile, OutputStr);
            Utils.Logger.Info("cmk8s module finished");
        }
    }
}
