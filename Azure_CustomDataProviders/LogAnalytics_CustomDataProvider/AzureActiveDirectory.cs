using LogAnalytics_CustomDataProvider.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace LogAnalytics_CustomDataProvider
{
    class AzureActiveDirectory
    {
        /// <summary>
        /// Retrieves Log from Azure Log Analytics
        /// </summary>
        /// <param name="localPluginContext">Context for the current plug-in.</param>
        public static AzureADToken ClientCredentialsFlow(ILocalPluginContext localPluginContext)
        {
            localPluginContext.TracingService.Trace("Starting to retrieve Client Credentials Flow");

            AzureADToken aadToken = null;
            byte[] bodyAsArray = null;

            try
            {
                var tenantId = "ee159f5f-6698-415a-9936-c765263a7fab";
                var clientId = "4508db82-fb3f-462a-9426-7ca930062fb8";
                var secret = "Km57Q~PTQkKYkXb33kqunkETOS8dSijTLipnb";
                var resourceUrl = "https://api.loganalytics.io";
                var requestUrl = $"/nhttps://login.microsoftonline.com/{tenantId}/oauth2/token";

                localPluginContext.TracingService.Trace("tenantId: {0}", tenantId);
                localPluginContext.TracingService.Trace("clientId: {0}", clientId);
                localPluginContext.TracingService.Trace("secret length: {0}", secret.Length);
                localPluginContext.TracingService.Trace("resourceUrl: {0}", resourceUrl);
                localPluginContext.TracingService.Trace("requestUrl: {0}", requestUrl);

                var dict = new Dictionary<string, string>
                {
                    { "grant_type", "client_credentials" },
                    { "client_id", clientId },
                    { "client_secret", secret },
                    { "resource", resourceUrl }
                };

                BinaryFormatter bf = new BinaryFormatter();
                using (MemoryStream ms = new MemoryStream())
                {
                    bf.Serialize(ms, dict);
                    bodyAsArray = ms.ToArray();

                    // Retrieve a client token
                    var webRequest = WebRequest.Create(requestUrl) as HttpWebRequest;

                    if (webRequest != null)
                    {
                        webRequest.ContentType = "application/x-www-form-urlencoded";
                        webRequest.Method = "POST";
                        webRequest.ContentLength = bodyAsArray.Length;

                        using (Stream dataStream = webRequest.GetRequestStream())
                        {
                            dataStream.Write(bodyAsArray, 0, bodyAsArray.Length);

                            using (var s = webRequest.GetResponse().GetResponseStream())
                            {
                                using (var sr = new StreamReader(s))
                                {
                                    var responseAsJson = sr.ReadToEnd();
                                    localPluginContext.TracingService.Trace("Response: {0}", responseAsJson);

                                    aadToken = JsonConvert.DeserializeObject<AzureADToken>(responseAsJson);
                                    if (aadToken == null)
                                    {
                                        localPluginContext.TracingService.Trace("Exception with message: {0}", "aadToken is null");
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                localPluginContext.TracingService.Trace("Exception with message: {0}", e.Message);
            }

            // Set output parameter
            return aadToken;
        }
    }
}
