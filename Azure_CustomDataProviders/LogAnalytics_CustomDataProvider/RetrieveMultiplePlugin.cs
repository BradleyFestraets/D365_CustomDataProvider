﻿using LogAnalytics_CustomDataProvider.Models;
using Microsoft.Xrm.Sdk;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace LogAnalytics_CustomDataProvider
{
    /// <summary>
    /// Base class for all plug-in classes.
    /// Plugin development guide: https://docs.microsoft.com/powerapps/developer/common-data-service/plug-ins
    /// Best practices and guidance: https://docs.microsoft.com/powerapps/developer/common-data-service/best-practices/business-logic/
    /// </summary>    
    public abstract class RetrieveMultiplePlugin : IPlugin
    {
        /// <summary>
        /// Gets or sets the name of the plugin class.
        /// </summary>
        /// <value>The name of the child class.</value>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "PluginBase")]
        protected string PluginClassName { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="PluginBase"/> class.
        /// </summary>
        /// <param name="pluginClassName">The <see cref=" cred="Type"/> of the derived class.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "PluginBase")]
        internal RetrieveMultiplePlugin(Type pluginClassName)
        {
            PluginClassName = pluginClassName.ToString();
        }

        /// <summary>
        /// Main entry point for he business logic that the plug-in is to execute.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        /// <remarks>
        /// For improved performance, Microsoft Dynamics 365 caches plug-in instances. 
        /// The plug-in's Execute method should be written to be stateless as the constructor 
        /// is not called for every invocation of the plug-in. Also, multiple system threads 
        /// could execute the plug-in at the same time. All per invocation state information 
        /// is stored in the context. This means that you should not use global variables in plug-ins.
        /// </remarks>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "Execute")]
        public void Execute(IServiceProvider serviceProvider)
        {
            if (serviceProvider == null)
            {
                throw new InvalidPluginExecutionException("serviceProvider");
            }

            // Construct the local plug-in context.
            var localPluginContext = new LocalPluginContext(serviceProvider);

            localPluginContext.Trace($"Entered {PluginClassName}.Execute() " +
                 $"Correlation Id: {localPluginContext.PluginExecutionContext.CorrelationId}, " +
                 $"Initiating User: {localPluginContext.PluginExecutionContext.InitiatingUserId}");

            try
            {
                // Invoke the custom implementation 
                ExecuteCdsPlugin(localPluginContext);

                // now exit - if the derived plug-in has incorrectly registered overlapping event registrations,
                // guard against multiple executions.
                return;
            }
            catch (FaultException<OrganizationServiceFault> orgServiceFault)
            {
                localPluginContext.Trace($"Exception: {orgServiceFault.ToString()}");

                // Handle the exception.
                throw new InvalidPluginExecutionException($"OrganizationServiceFault: {orgServiceFault.Message}", orgServiceFault);
            }
            finally
            {
                localPluginContext.Trace($"Exiting {PluginClassName}.Execute()");
            }
        }

        /// <summary>
        /// Retrieves Logs from Azure Log Analytics
        /// </summary>
        /// <param name="localPluginContext">Context for the current plug-in.</param>
        protected virtual void ExecuteCdsPlugin(ILocalPluginContext localPluginContext)
        {
            EntityCollection ec = new EntityCollection();

            localPluginContext.TracingService.Trace("Starting to retrieve Audit Logs");

            try
            {
                // Get data from Audit Logs
                var webRequest = WebRequest.Create("https://api.spacexdata.com/v3/launches?filter=rocket/rocket_name,flight_number,mission_name,launch_year,launch_date_utc,links,details") as HttpWebRequest;

                if (webRequest == null)
                {
                    return;
                }

                webRequest.ContentType = "application/json";

                using (var s = webRequest.GetResponse().GetResponseStream())
                {
                    using (var sr = new StreamReader(s))
                    {
                        var logsAsJson = sr.ReadToEnd();
                        var logs = JsonConvert.DeserializeObject<List<Log>>(logsAsJson);
                        localPluginContext.TracingService.Trace("Total number of logs: {0}", logs.Count);
                        ec.Entities.AddRange(logs.Select(l => l.ToEntity(localPluginContext.TracingService)));
                    }
                }
            }
            catch (Exception e)
            {
                localPluginContext.TracingService.Trace("Exception with message: {0}", e.Message);
            }

            // Set output parameter
            localPluginContext.PluginExecutionContext.OutputParameters["BusinessEntityCollection"] = ec;
        }
    }
}
