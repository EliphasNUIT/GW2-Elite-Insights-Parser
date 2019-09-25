using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace GW2EIParser.Controllers.GW2API
{
    public class GW2APISkill
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Icon { get; set; }
        [JsonPropertyName("chat_link")]
        public string ChatLink { get; set; }
        public string Type { get; set; }
        [JsonPropertyName("weapon_type")]
        public string WeaponType { get; set; }
        public List<string> Professions { get; set; }
        public string Slot { get; set; }
        public List<GW2APIFact> Facts { get; set; }
        [JsonPropertyName("traited_facts")]
        public List<GW2APITraitedFact> TraitedFacts { get; set; }
        public List<string> Categories { get; set; }
        public string Attunement { get; set; }
        public int Cost { get; set; }
        [JsonPropertyName("dual_wield")]
        public string DualWield { get; set; }
        [JsonPropertyName("flip_skill")]
        public int FlipSkill { get; set; }
        public int Initiative { get; set; }
        [JsonPropertyName("next_chain")]
        public int NextChain { get; set; }
        [JsonPropertyName("prev_chain")]
        public int PrevChain { get; set; }
        [JsonPropertyName("transform_skills")]
        public List<int> TransformSkills { get; set; }
        [JsonPropertyName("bundle_skills")]
        public List<int> BundleSkills { get; set; }

        [JsonPropertyName("toolbelt_skill")]
        public int ToolbeltSkill { get; set; }
    }

}
