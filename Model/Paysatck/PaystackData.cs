using Newtonsoft.Json;

namespace OloEcomm.Model.Paysatck
{
    public class PaystackData
    {
        [JsonProperty("authorization_url")]
        public string AuthorizationUrl { get; set; }

        [JsonProperty("access_code")]
        public string AccessCode { get; set; }

        [JsonProperty("reference")]
        public string Reference { get; set; }

        public DateTime? PaidAt { get; set; }
    }
}
