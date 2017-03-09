using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace jet.classes
{
    class BearerToken
    {
        [JsonProperty("id_token")]
        public string Token { get; set; }

        [JsonProperty("token_type")]
        public string TokenType { get; set; }

        [JsonProperty("expires_on")]
        public DateTime ExpiresOn { get; set; }
    }
}
