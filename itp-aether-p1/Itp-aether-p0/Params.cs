using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLine;

namespace Itp_aether_p1
{
    public class Params
    {
        [Option("module-type", Required = false, HelpText = "")]
        public string ModuleType { get; set; }

        #region common params 
        [Option("workspace-name", Required = false, HelpText = "")]
        public string WorkspaceName { get; set; }

        [Option("subscription-id", Required = false, HelpText = "")]
        public string SubscriptionId{ get; set; }

        [Option("resourcegroup-name", Required = false, HelpText = "")]
        public string ResourceGroupName { get; set; }

        [Option("output-file", Required = false, HelpText = "")]
        public string OutputFile { get; set; }
        #endregion

        #region DataStore module
        [Option("datastore-name", Required = false, HelpText = "")]
        public string DatastoreName{ get; set; }

        [Option("path-on-datastore", Required = false, HelpText = "")]
        public string PathOnDataStore{ get; set; }
        #endregion

        #region Cmk8s module
        [Option("enable-ipython", Required = false, HelpText = "")]
        public string EnableIphthon { get; set; }

        [Option("enable-ssh", Required = false, HelpText = "")]
        public string EnableSsh { get; set; }

        [Option("enable-tensorboard", Required = false, HelpText = "")]
        public string EnableTensorboard { get; set; }

        [Option("gpu-count", Required = false, HelpText = "")]
        public string GpuCount{ get; set; }

        [Option("preemption-allowed", Required = false, HelpText = "")]
        public string PreemptionAllowed { get; set; }

        [Option("deepspeed-config", Required = false, HelpText = "")]
        public string DeepspeedConfig{ get; set; }

        [Option("interactive-port", Required = false, HelpText = "")]
        public string InteractivePort { get; set; }

        [Option("ssh-public-key", Required = false, HelpText = "")]
        public string SshPublicKey { get; set; }
        #endregion

        #region Submitter module
        [Option("code-datastore-info", Required = false, HelpText = "")]
        public string CodeDatastoreInfo { get; set; }

        [Option("data-datastore-info", Required = false, HelpText = "")]
        public string DataDatastoreInfo { get; set; }

        [Option("cmk8s-info", Required = false, HelpText = "")]
        public string Cmk8sInfo { get; set; }

        [Option("aml-experiment-name", Required = false, HelpText = "")]
        public string AmlExperimentName{ get; set; }

        [Option("target", Required = false, HelpText = "")]
        public string Target{ get; set; }

        [Option("frame-work", Required = false, HelpText = "")]
        public string FrameWork{ get; set; }

        [Option("script", Required = false, HelpText = "")]
        public string Script { get; set; }

        [Option("arguments", Required = false, HelpText = "")]
        public string Arguments { get; set; }

        [Option("base-dockerimage", Required = false, HelpText = "")]
        public string BaseDockerImage{ get; set; }

        [Option("registry-address", Required = false, HelpText = "")]
        public string RegistryAddress{ get; set; }

        [Option("registry-pwd", Required = false, HelpText = "")]
        public string RegistryPwd{ get; set; }

        [Option("registry-username", Required = false, HelpText = "")]
        public string RegistryUserName{ get; set; }

        [Option("gpu-support", Required = false, HelpText = "")]
        public string GpuSupport{ get; set; }

        [Option("node-count", Required = false, HelpText = "")]
        public string NodeCount{ get; set; }

        [Option("conda-dependencies", Required = false, HelpText = "")]
        public string CondaDependencies{ get; set; }

        [Option("mpi-process-count-per-node", Required = false, HelpText = "")]
        public string MpiProcessCountPerNode { get; set; }

        [Option("communicator", Required = false, HelpText = "")]
        public string Communicator { get; set; }

        [Option("max-run-duration-seconds", Required = false, HelpText = "")]
        public string MaxRunDurationSeconds { get; set; }

        [Option("user-managed-dependencies", Required = false, HelpText = "")]
        public string UserManagedDependencies { get; set; }

        [Option("interpreter-path", Required = false, HelpText = "")]
        public string InterpreterPath { get; set; }

        [Option("tensorflow-workcount", Required = false, HelpText = "")]
        public string TensorflowWorkerCount { get; set; }

        [Option("tensorflow-parameter-server-count", Required = false, HelpText = "")]
        public string TensorflowParameterServerCount { get; set; }
        [Option("token", Required = false, HelpText = "")]
        public string Token { get; set; }
        #endregion
    }
}
