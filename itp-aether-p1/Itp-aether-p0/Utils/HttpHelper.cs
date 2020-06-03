using System;
using System.IO;
using System.Net;
using Newtonsoft.Json;

namespace Itp_aether_p1.Utils
{
    class HttpHelper
    {
        public static void Submit(string SubscriptionId, string ResourceGroupName, string WorkspaceName, string ExperimentName, string token)
        {
            try
            {
                string entry = Properties.Resources.ResourceManager.GetString("String1");
                if (!Directory.Exists("payload"))
                {
                    Directory.CreateDirectory("payload");
                }
                if (!Directory.Exists("payload/tmp"))
                {
                    Directory.CreateDirectory("payload/tmp");
                }
                File.WriteAllText("payload/tmp/entry.py", entry);
                Utils.ZipHelper.CreateZip("payload/tmp", "payload/project.zip");
                string region = GetRegion(SubscriptionId, ResourceGroupName, WorkspaceName, token);
                string str = $"curl -X POST https://{region}.api.azureml.ms/execution/v1.0/subscriptions/{SubscriptionId}/resourceGroups/{ResourceGroupName}/providers/Microsoft.MachineLearningServices/workspaces/{WorkspaceName}/experiments/{ExperimentName}/run?api-version=2019-08-01 -H \"Authorization: Bearer {token}\" -F files=@payload/definition.json -F files=@payload/project.zip";
                System.Diagnostics.Process p = new System.Diagnostics.Process();
                p.StartInfo.FileName = "cmd.exe";
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.RedirectStandardInput = true;
                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.RedirectStandardError = true;
                p.StartInfo.CreateNoWindow = true;
                p.Start();
                p.StandardInput.WriteLine(str + "&exit");
                p.StandardInput.AutoFlush = true;
                string output = p.StandardOutput.ReadToEnd();
                Console.WriteLine(output);
                p.WaitForExit();
                p.Close();
            }
            catch (Exception e)
            {
                Logger.Error($"post body to aml api : {e.ToString()}");
            }
        }

        public static string GetRegion(string SubscriptionId, string ResourceGroupName, string WorkspaceName, string token)
        {
            string url = $"https://management.azure.com/subscriptions/{SubscriptionId}/resourceGroups/{ResourceGroupName}/providers/Microsoft.MachineLearningServices/workspaces/{WorkspaceName}?api-version=2019-05-01";
            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(url);
            req.Method = "GET";
            req.Headers.Set("Authorization", $"Bearer {token}");
            using (WebResponse response = req.GetResponse())
            {
                var responseStream = response.GetResponseStream();
                if (responseStream != null)
                {
                    var reader = new StreamReader(responseStream);
                    string receiveContent = reader.ReadToEnd();
                    dynamic body = JsonConvert.DeserializeObject(receiveContent);
                    reader.Close();
                    return body["location"];
                }
            }
            return "";
        }
    }
}
