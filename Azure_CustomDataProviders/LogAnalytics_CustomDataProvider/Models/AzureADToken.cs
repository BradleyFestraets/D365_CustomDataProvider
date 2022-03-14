using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace LogAnalytics_CustomDataProvider.Models
{
    [DataContract]
    public class AzureADToken
    {
        [DataMember]
        public string token_type { get; set; }

        [DataMember]
        public string expires_in { get; set; }

        [DataMember]
        public string ext_expires_in { get; set; }

        [DataMember]
        public string expires_on { get; set; }

        [DataMember]
        public string not_before { get; set; }

        public string resource { get; set; }

        [DataMember]
        public string access_token { get; set; }
    }
}
