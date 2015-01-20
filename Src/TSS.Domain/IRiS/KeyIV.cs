using Newtonsoft.Json;

namespace TSS.Domain
{
    public class KeyIV
    {
        [JsonProperty(PropertyName = "key")]
        public string Key{ get; set; }

        [JsonProperty(PropertyName = "iv")]
        public string IV { get; set; }
    }

}
