using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using GW2EIParser.EIData;
using GW2EIParser.Parser;
using GW2EIParser.Parser.ParsedData.CombatEvents;
using static GW2EIParser.Builders.JsonModels.JsonLog;

namespace GW2EIParser.Builders.JsonModels
{
    public class JsonNPC : JsonMasterActor
    {
        /// <summary>
        /// % of health burned
        /// </summary>
        public double HpLeft { get; set; }
        /// <summary>
        /// Array of double[2] that represents the health status of the target \n
        /// Value[i][0] will be the time, value[i][1] will be health % \n
        /// If i corresponds to the last element that means the health did not change for the remainder of the fight \n
        /// </summary>
        public List<double[]> HealthPercents { get; set; }

        public string DescriptionID { get; set; }

        public JsonNPC(ParsedLog log, NPC npc, Dictionary<string, Desc> description, IEnumerable<AbstractSingleActor> targets, IEnumerable<AbstractSingleActor> allies) : base(log, npc, description, targets, allies)
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
            HealthPercents = hpUpdates.Select(x => new double[2] { x.Time, x.HPPercent }).ToList();
            DescriptionID = "npc" + npc.ID;
            if (!description.ContainsKey(DescriptionID))
            {
                description.Add(DescriptionID, new NPCDesc(npc, log));
            }
        }
    }
}
