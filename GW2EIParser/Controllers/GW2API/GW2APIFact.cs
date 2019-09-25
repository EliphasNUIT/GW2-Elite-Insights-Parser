using System.Text.Json.Serialization;

namespace GW2EIParser.Controllers.GW2API
{
    public class GW2APIFact
    {
        public string Text { get; set; }
        public string Icon { get; set; }
        public string Type { get; set; }
        //
        public string Target { get; set; }

        public object Value { get; set; }
        // 
        public string Status { get; set; }
        public string Description { get; set; }
        [JsonPropertyName("apply_count")]
        public int ApplyCount { get; set; }
        public float Duration { get; set; }
        //
        [JsonPropertyName("field_type")]
        public string FieldType { get; set; }
        //
        [JsonPropertyName("finisher_type")]
        public string FinisherType { get; set; }
        public float Percent { get; set; }
        [JsonPropertyName("hit_count")]
        //
        public int HitCount { get; set; }
        [JsonPropertyName("dmg_multiplier")]
        public float DmgMultiplier { get; set; }
        //
        public int Distance { get; set; }
        public GW2APIFact Prefix { get; set; }
    }
}
