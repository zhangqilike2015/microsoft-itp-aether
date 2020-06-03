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
    class Experiment : BaseModule
    {
        [JsonProperty("Configuration", NullValueHandling = NullValueHandling.Ignore)]
        public Configuration Configuration { get; set; }

        public Experiment(Params p)
        {
            p.AmlExperimentName += "fromaether";
            this.Configuration = new Configuration();
            this.Configuration.Framework = p.FrameWork;
            this.Configuration.UseAbsolutePath = false;
            this.Configuration.Communicator = p.Communicator;
            this.Configuration.Framework = "Python";
            this.Configuration.MaxRunDurationSeconds = Convert.ToInt32(p.MaxRunDurationSeconds == "" ? "0" : p.MaxRunDurationSeconds);
            this.Configuration.NodeCount = Convert.ToInt32(p.NodeCount == "" ? "0" : p.NodeCount);
            this.Configuration.Script = "entry.py";
            this.Configuration.Target = p.Target;
            this.Configuration.Docker = JsonConvert.DeserializeObject("{\"useDocker\":true,\"sharedVolumes\":true,\"shmSize\":\"2g\",\"arguments\":[]}");
            this.Configuration.History = JsonConvert.DeserializeObject("{\"outputCollection\":true,\"directoriesToWatch\":[\"logs\"],\"snapshotProject\":true}");
            #region arguments
            this.Configuration.Arguments = new List<string>();
            string script = $"%20{p.Script}";
            if (p.CodeDatastoreInfo != "")
            {
                this.Configuration.Arguments.Add("--env_CODE");
                this.Configuration.Arguments.Add("$AZUREML_DATAREFERENCE_code");
                script = $"%20%24CODE/{p.Script}";
            }
            string data = "";
            if (p.DataDatastoreInfo != "")
            {
                this.Configuration.Arguments.Add("--env_DATA");
                this.Configuration.Arguments.Add("$AZUREML_DATAREFERENCE_data");
                data = "%20--data-folder%20%24DATA";
            }
            string arguments = "";
            if (p.Arguments != "")
            {
                arguments = Utils.Arguments.Parse(p.Arguments);
                if (arguments == "")
                {
                    Console.WriteLine("Error: Invalid arguments pattern");
                    System.Environment.Exit(1);
                }
                this.Configuration.Arguments.Add("--env_EXTRA_PARAMS");
                this.Configuration.Arguments.Add(arguments);
            }
            this.Configuration.Arguments.Add("--command");
            this.Configuration.Arguments.Add($"python{script}{data}%20--model-folder%20outputs/model%20%24EXTRA_ARGS");
            //this.Configuration.Arguments.Add($"python{script}{data}%20%24EXTRA_ARGS");
            #endregion

            #region DataReference
            this.Configuration.DataReferences = new DataReferences();
            if (p.CodeDatastoreInfo != "")
            {
                string codeInfo = File.ReadAllText(p.CodeDatastoreInfo);
                this.Configuration.DataReferences.Code = JsonConvert.DeserializeObject(codeInfo);
            }
            if (p.DataDatastoreInfo != "")
            {
                string dataInfo = File.ReadAllText(p.DataDatastoreInfo);
                this.Configuration.DataReferences.Data = JsonConvert.DeserializeObject(dataInfo);
            }
            #endregion

            #region cmk8s
            if (p.Cmk8sInfo != "")
            {
                string cmk8sInfo = File.ReadAllText(p.Cmk8sInfo);
                this.Configuration.Cmk8sCompute = new Cmk8sCompute();
                this.Configuration.Cmk8sCompute.Configuration = JsonConvert.DeserializeObject(cmk8sInfo);
            }
            #endregion

            #region mpi
            this.Configuration.Mpi = new Mpi();
            this.Configuration.Mpi.ProcessCountPerNode = Convert.ToInt32(p.MpiProcessCountPerNode == "" ? "0" : p.MpiProcessCountPerNode);
            #endregion

            #region tensorflow
            this.Configuration.Tensorflow = new Tensorflow();
            this.Configuration.Tensorflow.ParameterServerCount = Convert.ToInt32(p.TensorflowParameterServerCount == "" ? "0" : p.TensorflowParameterServerCount);
            this.Configuration.Tensorflow.WorkerCount = Convert.ToInt32(p.TensorflowWorkerCount == "" ? "0" : p.TensorflowWorkerCount);
            #endregion

            #region environment
            this.Configuration.Environment = new Environment();
            this.Configuration.Environment.Docker = new Docker();
            this.Configuration.Environment.Docker.BaseImage = p.BaseDockerImage;
            this.Configuration.Environment.Docker.Enabled = true;
            this.Configuration.Environment.Docker.BaseImageRegistry = new BaseImageRegistry();
            if (p.RegistryAddress == "")
            {
                this.Configuration.Environment.Docker.BaseImageRegistry.Address = null;
                this.Configuration.Environment.Docker.BaseImageRegistry.Password = null;
                this.Configuration.Environment.Docker.BaseImageRegistry.Username = null;
            }
            else
            {
                this.Configuration.Environment.Docker.BaseImageRegistry.Address = p.RegistryAddress;
                this.Configuration.Environment.Docker.BaseImageRegistry.Password = p.RegistryPwd;
                this.Configuration.Environment.Docker.BaseImageRegistry.Username = p.RegistryUserName;
            }
            this.Configuration.Environment.Python = new Python();
            this.Configuration.Environment.Python.InterpreterPath = p.InterpreterPath;
            this.Configuration.Environment.Python.UserManagedDependencies = p.UserManagedDependencies == "true" ? true : false;
            this.Configuration.Environment.Python.CondaDependencies = new CondaDependencies();
            if (p.UserManagedDependencies == "false" && p.CondaDependencies != "")
            {
                p.CondaDependencies = p.CondaDependencies + ",azureml-defaults";
                this.Configuration.Environment.Python.CondaDependencies.Dependencies.Add(new Pip(p.CondaDependencies.Split(new char[] { ',' })));
            }
            else
            {
                this.Configuration.Environment.Python.CondaDependencies.Dependencies.Add(new Pip(new string[] { "azureml-defaults" }));
            }
            #endregion
        }
        public void Exec(Params p)
        {
            string definition = JsonConvert.SerializeObject(this);
            Utils.Logger.Info($"definition spec is : {definition}");
            if (!Directory.Exists("payload"))
            {
                Directory.CreateDirectory("payload");
            }
            File.WriteAllText("payload/definition.json", definition);
            if (!Utils.Authorization.IsAurhorised(p.Token))
            {
                Utils.Logger.Error($"unauthorised user");
            }
            Utils.Logger.Info("start submitting");
            Utils.HttpHelper.Submit(SubscriptionId: p.SubscriptionId,
                ResourceGroupName: p.ResourceGroupName, 
                ExperimentName: p.AmlExperimentName, 
                WorkspaceName: p.WorkspaceName,
                token: p.Token);
            Utils.Logger.Info("finished submitting");
        }
    }

    internal class DataReferences
    {

        [JsonProperty("code", NullValueHandling = NullValueHandling.Ignore)]
        public object Code { get; set; }

        [JsonProperty("data", NullValueHandling = NullValueHandling.Ignore)]
        public object Data { get; set; }

        [JsonProperty("output", NullValueHandling = NullValueHandling.Ignore)]
        public object Output { get; set; }
    }

    internal class Cmk8sCompute
    {

        [JsonProperty("configuration", NullValueHandling = NullValueHandling.Ignore)]
        public object Configuration { get; set; }
    }

    internal class BaseImageRegistry
    {

        [JsonProperty("address", NullValueHandling = NullValueHandling.Ignore)]
        public string Address { get; set; }

        [JsonProperty("password", NullValueHandling = NullValueHandling.Ignore)]
        public string Password { get; set; }

        [JsonProperty("username", NullValueHandling = NullValueHandling.Ignore)]
        public string Username { get; set; }
    }

    internal class Docker
    {

        [JsonProperty("baseImage", NullValueHandling = NullValueHandling.Ignore)]
        public string BaseImage { get; set; }

        [JsonProperty("baseImageRegistry", NullValueHandling = NullValueHandling.Ignore)]
        public BaseImageRegistry BaseImageRegistry { get; set; }

        [JsonProperty("enabled", NullValueHandling = NullValueHandling.Ignore)]
        public bool Enabled { get; set; }
    }

    internal class Python
    {

        [JsonProperty("baseCondaEnvironment", NullValueHandling = NullValueHandling.Ignore)]
        public object BaseCondaEnvironment { get; set; }

        [JsonProperty("condaDependencies", NullValueHandling = NullValueHandling.Ignore)]
        public CondaDependencies CondaDependencies { get; set; }

        [JsonProperty("interpreterPath", NullValueHandling = NullValueHandling.Ignore)]
        public string InterpreterPath { get; set; }

        [JsonProperty("userManagedDependencies", NullValueHandling = NullValueHandling.Ignore)]
        public bool UserManagedDependencies { get; set; }

        public Python()
        {
            this.BaseCondaEnvironment = null;
        }
    }

    internal class CondaDependencies
    {

        [JsonProperty("channels")]
        public IList<string> Channels { get; set; }

        [JsonProperty("dependencies")]
        public IList<object> Dependencies { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        public CondaDependencies()
        {
            this.Channels = new List<string>();
            this.Channels.Add("anaconda");
            this.Channels.Add("conda-forge");
            this.Dependencies = new List<object>();
            this.Dependencies.Add("python=3.6.2");
        }
    }

    internal class Pip
    {
        [JsonProperty("pip")]
        string[] pip;
        
        public Pip(string[] pips)
        {
            this.pip = pips;
        }
    }

    internal class Environment
    {

        [JsonProperty("docker", NullValueHandling = NullValueHandling.Ignore)]
        public Docker Docker { get; set; }

        [JsonProperty("python", NullValueHandling = NullValueHandling.Ignore)]
        public Python Python { get; set; }
    }

    internal class Mpi
    {

        [JsonProperty("processCountPerNode", NullValueHandling = NullValueHandling.Ignore)]
        public int ProcessCountPerNode { get; set; }
    }

    internal class Tensorflow
    {

        [JsonProperty("parameterServerCount", NullValueHandling = NullValueHandling.Ignore)]
        public int ParameterServerCount { get; set; }

        [JsonProperty("workerCount", NullValueHandling = NullValueHandling.Ignore)]
        public int WorkerCount { get; set; }
    }

    internal class Configuration
    {
        [JsonProperty("useAbsolutePath", NullValueHandling = NullValueHandling.Ignore)]
        public bool UseAbsolutePath { get; set; }

        [JsonProperty("arguments", NullValueHandling = NullValueHandling.Ignore)]
        public IList<string> Arguments { get; set; }

        [JsonProperty("communicator", NullValueHandling = NullValueHandling.Ignore)]
        public string Communicator { get; set; }

        [JsonProperty("dataReferences", NullValueHandling = NullValueHandling.Ignore)]
        public DataReferences DataReferences { get; set; }

        [JsonProperty("cmk8sCompute", NullValueHandling = NullValueHandling.Ignore)]
        public Cmk8sCompute Cmk8sCompute { get; set; }

        [JsonProperty("environment", NullValueHandling = NullValueHandling.Ignore)]
        public Environment Environment { get; set; }

        [JsonProperty("framework", NullValueHandling = NullValueHandling.Ignore)]
        public string Framework { get; set; }

        [JsonProperty("maxRunDurationSeconds", NullValueHandling = NullValueHandling.Ignore)]
        public int? MaxRunDurationSeconds { get; set; }

        [JsonProperty("mpi", NullValueHandling = NullValueHandling.Ignore)]
        public Mpi Mpi { get; set; }

        [JsonProperty("nodeCount", NullValueHandling = NullValueHandling.Ignore)]
        public int? NodeCount { get; set; }

        [JsonProperty("script", NullValueHandling = NullValueHandling.Ignore)]
        public string Script { get; set; }

        [JsonProperty("sourceDirectoryDataStore", NullValueHandling = NullValueHandling.Ignore)]
        public string SourceDirectoryDataStore { get; set; }

        [JsonProperty("target", NullValueHandling = NullValueHandling.Ignore)]
        public string Target { get; set; }

        [JsonProperty("tensorflow", NullValueHandling = NullValueHandling.Ignore)]
        public Tensorflow Tensorflow { get; set; }

        [JsonProperty("docker", NullValueHandling = NullValueHandling.Ignore)]
        public object Docker { get; set; }

        [JsonProperty("history", NullValueHandling = NullValueHandling.Ignore)]
        public object History { get; set; }
    }
}
