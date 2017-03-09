using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace jet.classes
{
    public class FulfillmentNodes
    {
        [JsonProperty("fulfillment_nodes")]
        public FulfillmentNode[] fulfillmentNode { get; set; }
    }
    public class FulfillmentNode
    {
        [JsonProperty("jet_fulfillment_node_id")]
        public string fulfillmentNodeId { get; set; }

        [JsonProperty("location_name")]
        public string locationName { get; set; }

        [JsonProperty("time_zone")]
        public string timeZone { get; set; }

        [JsonProperty("is_daylight_savings_enabled")]
        public string daylightSavingEnabled { get; set; }

        [JsonProperty("address_1")]
        public string address1 { get; set; }

        [JsonProperty("zip_code")]
        public string zipCode { get; set; }

        [JsonProperty("city")]
        public string city { get; set; }

        [JsonProperty("state")]
        public string state { get; set; }

    }
}
