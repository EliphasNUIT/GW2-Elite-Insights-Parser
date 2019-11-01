using System.Collections.Generic;
using System.Linq;
using GW2EIParser.EIData;
using GW2EIParser.Parser;
using static GW2EIParser.Builders.JsonModels.JsonLog;

namespace GW2EIParser.Builders.JsonModels
{
    /// <summary>
    /// Base class single actors
    /// </summary>
    public abstract class JsonSingleActor
    {
        /// <summary>
        /// Buff data
        /// </summary>
        /// <seealso cref="JsonBuffData"/>
        public JsonBuffData BuffData { get; set; }
        /// <summary>
        /// Damage distribution data
        /// </summary>
        /// <seealso cref="JsonDamageDistData"/>
        public JsonDamageDistData DamageDistributionData { get; set; }
        /// <summary>
        /// Rotation data
        /// </summary>
        /// <seealso cref="JsonRotation"/>
        public List<JsonRotation> Rotation { get; set; }
        /// <summary>
        /// Unique ID representing the actor
        /// </summary>
        public string UniqueID { get; set; }
        /// <summary>
        /// Unique ID representing the description of the actor
        /// </summary>
        public string DescriptionID { get; set; }

        /// <summary>
        /// List of time during which the actor was active (not dead and not dc) \n
        /// Length == number of phases
        /// </summary>
        public List<long> ActiveTimes { get; set; }
        /// <summary>
        /// List of minions, regrouped by type
        /// </summary>
        /// <seealso cref="JsonMinions"/>
        public List<JsonMinions> Minions { get; set; }

        protected JsonSingleActor(ParsedLog log, AbstractSingleActor actor, Dictionary<string, Desc> description)
        {
            // Rotation
            Rotation = JsonRotation.BuildRotation(actor.GetCastLogs(log, 0, log.FightData.FightDuration), description);
            // Damage dist
            DamageDistributionData = new JsonDamageDistData(log, actor, description);
            // Buff
            BuffData = new JsonBuffData(log, actor, description);
            //
            UniqueID = actor.AgentItem.UniqueID;
            //
            ActiveTimes = log.FightData.GetPhases(log).Select(x => x.GetActorActiveDuration(actor, log)).ToList();
            // Minions
            Minions = actor.GetMinions(log).Select(x => new JsonMinions(log, x.Value, description)).ToList();
            if (Minions.Count == 0)
            {
                Minions = null;
            }
        }
    }
}

