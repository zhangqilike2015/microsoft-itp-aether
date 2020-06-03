using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IdentityModel.Tokens.Jwt;
//using Microsoft.Aether.Library;
//using Microsoft.IdentityModel.Clients.ActiveDirectory;

namespace Itp_aether_p1.Utils
{
    class Authorization
    {
       public static string GetUserNameFromCmd()
        {
            try
            {
                string[] cmd = Environment.CommandLine.Split(new char[] { ' ' });
                foreach (string str in cmd)
                {
                    if (str.StartsWith("Owner"))
                    {
                        string[] tmp = str.Split(new char[] { '=' });
                        return tmp[1];
                    }
                }
                return "";
            }
            catch (Exception e)
            {
                Logger.Error($"get user name from cmd: {e.ToString()}");
                return "";
            }
        }

        public static string GetUserNameFromToken(string token)
        {
            try
            {
                var handler = new JwtSecurityTokenHandler();
                var tokenS = handler.ReadToken(token) as JwtSecurityToken;
                var upn = tokenS.Claims.First(claim => claim.Type == "upn").Value;
                string username = upn.Split(new char[] { '@' })[0];
                return username;
            }
            catch (Exception e)
            {
                Logger.Error($"get user name from token: {e.ToString()}");
                return "";
            }
        }
        
        public static bool IsAurhorised(string token)
        {
            var userNameFromCmd = GetUserNameFromCmd();
            Logger.Info(userNameFromCmd);
            var userNameFromToken = GetUserNameFromToken(token);
            Logger.Info(userNameFromToken);
            return userNameFromCmd != "" && userNameFromCmd.Contains(userNameFromToken);
         } 
        //public static string GetUserName()
        //{
        //    var aetherS2STokenProvider = new AadS2STokenProvider(
        //        appId: "9190a3e7-aa29-43e0-a2a3-f1bc0d06c245",
        //        appSecret: "~N5iv4_Al1x-1wv~ICz~6_S3Z0KHjYJat1",
        //        tokenCache: new TokenCache());
        //    var environment = new AetherEnvironment(
        //        endpointAddress: AetherEnvironment.Aether1PEndpointAddress,
        //        clientName: "MyTestApp",
        //        azureAppServicePrincipal: new AzureAppServicePrincipal(
        //            getTokenCallbackAsync: aetherS2STokenProvider.GetAccessTokenAsync,
        //            onBehalfOfUserName: "qizhang4",
        //            onBehalfOfUserDomainName: "FAREAST"));
        //    string[] cmd = Environment.CommandLine.Split(new char[] { ' ' });
        //    string[] tmp = new string[] { };
        //    foreach (string str in cmd)
        //    {
        //        if (str.StartsWith("ExperimentID"))
        //        {
        //            Console.WriteLine(str);
        //            tmp = str.Split(new char[] { '=' });
        //        }
        //    }
        //    if (tmp.Length < 2)
        //    {
        //        return "";
        //    }
        //    IAetherExperiment experiment = environment.GetExperiment(tmp[1]);
        //    return experiment.Owner;
        //}
    }
}
