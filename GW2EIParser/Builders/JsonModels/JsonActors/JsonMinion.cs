using System.Collections.Generic;
using GW2EIParser.EIData;
using GW2EIParser.Parser;
using static GW2EIParser.Builders.JsonModels.JsonLog;

namespace GW2EIParser.Builders.JsonModels
{
    public class JsonMinion : JsonNPC
    {

        public JsonMinion(ParsedLog log, NPC minion, Dictionary<string, Desc> description, IEnumerable<AbstractSingleActor> targets, IEnumerable<AbstractSingleActor> allies) : base(log, minion, description, targets, allies)
        {
            Condition = 0;
            Concentration = 0;
            Toughness = 0;
            Healing = 0;
            HitboxHeight = 0;
            HitboxWidth = 0;
        }
    }
}
