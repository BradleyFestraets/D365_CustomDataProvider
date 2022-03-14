using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogAnalytics_CustomDataProvider.Models
{
    /// <summary>
    /// Represents Logs from Azure Log Analytics
    /// </summary>
    class Log
    {
        public Guid uniqueIdentifier { get; set; }
        public string name { get; set; }       

        public Entity ToEntity(ITracingService tracingService)
        {
            Entity entity = new Entity("bf_CustomAuditLog");

            // Map data to entity
            entity["bf_CustomAuditLogId"] = uniqueIdentifier;
            entity["bf_name"] = name;

            return entity;
        }
    }
}
