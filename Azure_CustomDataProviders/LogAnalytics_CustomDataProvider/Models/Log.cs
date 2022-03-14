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
        public string computer { get; set; }
        public string managementgroupname { get; set; }
        public string mg { get; set; }
        public string rawdata { get; set; }
        public string resourceid { get; set; }
        public string sourcesystem { get; set; }
        public string tenantid { get; set; }
        public string type { get; set; }
        public DateTime timegenerated { get; set; }


        public Entity ToEntity(ITracingService tracingService)
        {
            Entity entity = new Entity("bf_Log");

            // Map data to entity
            entity["bf_CustomAuditLogId"] = uniqueIdentifier;
            entity["bf_name"] = name;
            entity["bf_computer"] = computer;
            entity["bf_managementgroupname"] = managementgroupname;
            entity["bf_mg"] = mg;
            entity["bf_rawdata"] = rawdata;
            entity["bf_resourceid"] = resourceid;
            entity["bf_sourcesystem"] = sourcesystem;
            entity["bf_tenantid"] = tenantid;
            entity["bf_type"] = type;
            entity["bf_timegenerated"] = timegenerated;

            return entity;
        }
    }
}
