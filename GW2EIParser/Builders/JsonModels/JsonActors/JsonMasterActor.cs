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

        /// <summary>
        /// Combat Replay ID to link with <see cref="JsonCombatReplay"/> \n
        /// <see cref="JsonPlayer"/> will be in <seea cref="JsonCombatReplay.Players"/> and <see cref="JsonNPC"/> in <see cref="JsonCombatReplay.Npcs"/>
        /// </summary>
        public int CombatReplayID { get; set; }

        protected JsonMasterActor(ParsedLog log, AbstractMasterActor actor, Dictionary<string, SkillDesc> skillMap, Dictionary<string, BuffDesc> buffMap, IEnumerable<AbstractMasterActor> targets, IEnumerable<AbstractMasterActor> allies) : base(log, actor, skillMap, buffMap, targets, allies)
        {
            // Minions
            Minions = actor.GetMinions(log).Select(x => new JsonMinions(log, x.Value, skillMap, buffMap, targets, allies)).ToList();
            CombatReplayID = actor.GetCombatReplayID(log);
            if (Minions.Count == 0)
            {
                Minions = null;
            }
        }
    }
}
