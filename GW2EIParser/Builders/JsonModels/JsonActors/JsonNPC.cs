using System.Collections.Generic;
using System.Linq;
using GW2EIParser.EIData;
using GW2EIParser.Parser;
using GW2EIParser.Parser.ParsedData.CombatEvents;
using static GW2EIParser.Builders.JsonModels.JsonLog;

namespace GW2EIParser.Builders.JsonModels
{
    public class JsonNPC : JsonSingleActor
    {
        /// <summary>
        /// % of health burned
        /// </summary>
        public double HpLeft { get; set; }
        /// <summary>
        /// Array of double that represents the health status of the target \n
        /// Value[2 * i] will be the time, value[2 * i + 1] will be health % \n
        /// If i corresponds to the last element that means the health did not change for the remainder of the fight \n
        /// </summary>
        public List<double> HealthPercents { get; set; }
        /// <summary>
        /// Indicates a main target of the fight
        /// </summary>
        public bool MainTarget { get; set; }

        public JsonNPC(ParsedLog log, NPC npc, Dictionary<string, Desc> description) : base(log, npc, description)
        {
            HpLeft = 0.0;
            List<HealthUpdateEvent> hpUpdates = log.CombatData.GetHealthUpdateEvents(npc.AgentItem);
            if (log.FightData.Success && log.FightData.GetMainTargets(log).Contains(npc))
            {
                HpLeft = 0;
            }
            else
            {
                if (hpUpdates.Count > 0)
                {
                    HpLeft = hpUpdates.Last().HPPercent;
                }
            }
            HealthPercents = new List<double>();
            foreach (HealthUpdateEvent item in hpUpdates)
            {
                HealthPercents.Add(item.Time);
                HealthPercents.Add(item.HPPercent);
            }
            if (!HealthPercents.Any())
            {
                HealthPercents = null;
            }
            DescriptionID = "npc" + npc.ID;
            if (!description.ContainsKey(DescriptionID))
            {
                description.Add(DescriptionID, new NPCDesc(npc, log));
            }
            MainTarget = log.FightData.GetMainTargets(log).Contains(npc);
        }
    }
}
