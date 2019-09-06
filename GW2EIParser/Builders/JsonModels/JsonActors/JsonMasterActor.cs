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
        /// List of minions, regrouped by type
        /// </summary>
        /// <seealso cref="JsonMinions"/>
        public List<JsonMinions> Minions { get; set; }

        protected JsonMasterActor(ParsedLog log, AbstractSingleActor actor, Dictionary<string, Desc> description) : base(log, actor, description)
        {
            // Minions
            Minions = actor.GetMinions(log).Select(x => new JsonMinions(log, x.Value, description)).ToList();
            if (Minions.Count == 0)
            {
                Minions = null;
            }
        }
    }
}
