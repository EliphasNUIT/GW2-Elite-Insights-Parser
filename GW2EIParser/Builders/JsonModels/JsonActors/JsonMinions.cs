using System.Collections.Generic;
using System.Linq;
using GW2EIParser.EIData;
using GW2EIParser.Parser;
using GW2EIParser.Parser.ParsedData.CombatEvents;
using static GW2EIParser.Builders.JsonModels.JsonLog;

namespace GW2EIParser.Builders.JsonModels
{
    /// <summary>
    /// Class corresponding to the regrouping of the same type of minions
    /// </summary>
    public class JsonMinions : JsonActor
    {
        private const bool SimpleMinions = false;

        public List<JsonNPC> MinionList { get; set; }
        public JsonMinions(ParsedLog log, Minions minions, Dictionary<string, Desc> description)
        {
            if (!minions.MinionList.Any())
            {
                return;
            }
            if  (!SimpleMinions)
            {
                MinionList = minions.MinionList.Select(x => new JsonNPC(log, x, description)).ToList();
            }
            else
            {
                UniqueID = minions.AgentItem.UniqueID;
                DescriptionID = "npc" + minions.ID;
                DamageDistributionData = new JsonDamageDistData(log, minions, description);
                Rotation = JsonRotation.BuildRotation(minions.GetCastLogs(log, 0, log.FightData.FightDuration), description);
                if (!description.ContainsKey(DescriptionID))
                {
                    description.Add(DescriptionID, new NPCDesc(minions.MinionList.FirstOrDefault(), log));
                }
            }
        }
    }
}
