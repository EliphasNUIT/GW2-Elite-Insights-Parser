using System.Collections.Generic;
using System.Linq;
using GW2EIParser.EIData;
using GW2EIParser.Parser;
using static GW2EIParser.Builders.JsonModels.JsonLog;

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

        protected JsonMasterActor(ParsedLog log, AbstractSingleActor actor, Dictionary<string, Desc> description, IEnumerable<AbstractSingleActor> targets, IEnumerable<AbstractSingleActor> allies) : base(log, actor, description, targets, allies)
        {
            // Minions
            Minions = actor.GetMinions(log).Select(x => new JsonMinions(log, x.Value, description, targets, allies)).ToList();
            CombatReplayID = actor.GetCombatReplayID(log);
            if (Minions.Count == 0)
            {
                Minions = null;
            }
        }
    }
}
