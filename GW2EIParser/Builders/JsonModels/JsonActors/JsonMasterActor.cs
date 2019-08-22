using GW2EIParser.EIData;
using GW2EIParser.Parser;
using GW2EIParser.Parser.ParsedData.CombatEvents;
using System.Collections.Generic;
using System.Linq;
using static GW2EIParser.Builders.JsonModels.JsonLog;
using static GW2EIParser.Builders.JsonModels.JsonStatistics;

namespace GW2EIParser.Builders.JsonModels
{
    /// <summary>
    /// Base class for Players and NPCs, actor who can have minions
    /// </summary>
    /// <seealso cref="JsonPlayer"/> 
    /// <seealso cref="JsonNPC"/>
    public abstract class JsonMasterActor : JsonSingleActor
    {
        /// <summary>
        /// List of minions
        /// </summary>
        /// <seealso cref="JsonMinions"/>
        public List<JsonMinions> Minions { get; set; }

        protected JsonMasterActor(ParsedLog log, AbstractMasterActor actor, Dictionary<string, SkillDesc> skillMap, Dictionary<string, BuffDesc> buffMap, IEnumerable<AbstractMasterActor> targets) : base(log, actor, skillMap, buffMap, targets)
        {
            // Minions
            Minions = actor.GetMinions(log).Select(x => new JsonMinions(log, x.Value, skillMap, buffMap, targets)).ToList();
        }
    }
}
