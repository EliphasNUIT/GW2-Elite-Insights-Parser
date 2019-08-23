using Newtonsoft.Json;
using System.Collections.Generic;

namespace GW2EIParser.Controllers.GW2API
{
    public class GW2APISkill
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Icon { get; set; }
        [JsonProperty(PropertyName = "chat_link")]
        public string ChatLink { get; set; }
        public string Type { get; set; }
        [JsonProperty(PropertyName = "weapon_type")]
        public string WeaponType { get; set; }
        public List<string> Professions { get; set; }
        public string Slot { get; set; }
        public List<string> Categories { get; set; }
        public List<string> Flags { get; set; }
        public List<GW2APIFact> Facts { get; set; }
        public long Specialization { get; set; }
        [JsonProperty(PropertyName = "dual_wield")]
        public string DualWield { get; set; }
    }

}