using System.Text.Json.Serialization;

namespace GW2EIParser.Controllers.GW2API
{
    public class GW2APITraitedFact : GW2APIFact
    {
        [JsonPropertyName("requires_trait")]
        public int RequiresTrait { get; set; }
        public int Overrides { get; set; }
    }
}
