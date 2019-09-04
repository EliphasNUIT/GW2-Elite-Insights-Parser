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
        public List<JsonNPC> MinionList { get; set; }
        public JsonMinions(ParsedLog log, Minions minions, Dictionary<string, Desc> description, List<AbstractSingleActor> targets, List<AbstractSingleActor> allies)
        {
            MinionList = minions.MinionList.Select(x => new JsonNPC(log, x, description, targets, allies)).ToList();
            if (MinionList.Count == 0)
            {
                MinionList = null;
            }
        }
    }
}
