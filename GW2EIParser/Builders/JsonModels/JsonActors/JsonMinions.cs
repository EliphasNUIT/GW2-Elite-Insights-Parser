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
        /// <summary>
        /// Game ID of the minion
        /// </summary>
        public ushort ID { get; set; }
        /// <summary>
        /// Total health of the minion
        /// </summary>
        public int TotalHealth { get; set; }
        public List<JsonMinion> MinionList { get; set; }
        public JsonMinions(ParsedLog log, Minions minions, Dictionary<string, SkillDesc> skillMap, Dictionary<string, BuffDesc> buffMap, IEnumerable<AbstractSingleActor> targets, IEnumerable<AbstractSingleActor> allies) : base(minions)
        {
            MaxHealthUpdateEvent maxHP = log.CombatData.GetMaxHealthUpdateEvents(minions.AgentItem).LastOrDefault();
            TotalHealth = maxHP != null ? maxHP.MaxHealth : 0;
            ID = minions.ID;
            MinionList = minions.MinionList.Select(x => new JsonMinion(log, x, skillMap, buffMap, targets, allies)).ToList();
            if (MinionList.Count == 0)
            {
                MinionList = null;
            }
        }
    }
}
