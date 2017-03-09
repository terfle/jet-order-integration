using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace jet.classes
{
    class OrderUrlList
    {
        [JsonProperty("order_urls")]
        public List<string> OrderUrls { get; set; }
    }
}
