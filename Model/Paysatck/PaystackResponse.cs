using Newtonsoft.Json;

namespace OloEcomm.Model.Paysatck
{
    public class PaystackResponse
    {
      

        public bool Status { get; set; }

        public string Message { get; set; } = string.Empty;

        [JsonProperty("data")]
        public PaystackData Data { get; set; }
    }
}
