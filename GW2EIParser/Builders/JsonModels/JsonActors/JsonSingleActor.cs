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
    public abstract class JsonSingleActor : JsonActor
    {
        /// <summary>
        /// Array of int that represents the number of conditions status \n
        /// Value[2*i] will be the time, value[2*i+1] will be the number of conditions present from value[2*i] to value[2*(i+1)] \n
        /// If i corresponds to the last element that means the status did not change for the remainder of the fight \n
        /// </summary>
        public List<int> ConditionsStates { get; }
        /// <summary>
        /// Array of int that represents the number of boons status \n
        /// Value[2*i] will be the time, value[2*i+1] will be the number of boons present from value[2*i] to value[2*(i+1)] \n
        /// If i corresponds to the last element that means the status did not change for the remainder of the fight
        /// </summary>
        public List<int> BoonsStates { get; }
        /// <summary>
        /// Statistics data
        /// </summary>
        /// <seealso cref="JsonStatistics"/>
        public JsonStatistics Statistics { get; set; }
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

        protected JsonSingleActor(ParsedLog log, AbstractSingleActor actor, Dictionary<string, Desc> description)
        {
            // # of Boons and Conditions States
            if (actor.GetBuffGraphs(log).TryGetValue(ProfHelper.NumberOfBoonsID, out BuffsGraphModel bgmBoon))
            {
                BoonsStates = bgmBoon.GetStatesList();
            }
            if (actor.GetBuffGraphs(log).TryGetValue(ProfHelper.NumberOfConditionsID, out BuffsGraphModel bgmCondition))
            {
                ConditionsStates = bgmCondition.GetStatesList();
            }
            // Rotation
            Rotation = JsonRotation.BuildRotation(actor.GetCastLogs(log, 0, log.FightData.FightDuration), description);
            // Damage dist
            DamageDistributionData = new JsonDamageDistData(log, actor, description);
            // Stats
            Statistics = new JsonStatistics(log, actor);
            // Buff
            BuffData = new JsonBuffData(log, actor, description);
            //
            UniqueID = actor.AgentItem.UniqueID;
            //
            ActiveTimes = log.FightData.GetPhases(log).Select(x => x.GetActorActiveDuration(actor, log)).ToList();
        }
    }
}
