using System.Collections.Generic;
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
        /// Array of int[2] that represents the number of conditions status \n
        /// Value[i][0] will be the time, value[i][1] will be the number of conditions present from value[i][0] to value[i+1][0] \n
        /// If i corresponds to the last element that means the status did not change for the remainder of the fight \n
        /// </summary>
        public List<int[]> ConditionsStates { get; }
        /// <summary>
        /// Array of int[2] that represents the number of boons status \n
        /// Value[i][0] will be the time, value[i][1] will be the number of boons present from value[i][0] to value[i+1][0] \n
        /// If i corresponds to the last element that means the status did not change for the remainder of the fight
        /// </summary>
        public List<int[]> BoonsStates { get; }
        /// <summary>
        /// Rotation data
        /// </summary>
        /// <seealso cref="JsonRotation"/>
        public List<JsonRotation> Rotation { get; }
        /// <summary>
        /// Damage distribution data
        /// </summary>
        /// <seealso cref="JsonDamageDistData"/>
        public JsonDamageDistData DamageDistributionData { get; set; }
        /// <summary>
        /// Statistics data
        /// </summary>
        /// <seealso cref="JsonStatistics"/>
        public JsonStatistics Statistics { get; set; }
        /// <summary>
        /// Unique ID representing the actor
        /// </summary>
        public string UniqueID { get; set; }
        /// <summary>
        /// Unique ID representing the description of the actor
        /// </summary>
        public string DescriptionID { get; set; }

        protected JsonSingleActor(ParsedLog log, AbstractSingleActor actor, Dictionary<string, Desc> description, IEnumerable<AbstractSingleActor> targets, IEnumerable<AbstractSingleActor> allies)
        {
            // # of Boons and Conditions States
            if (actor.GetBuffGraphs(log).TryGetValue(ProfHelper.NumberOfBoonsID, out BuffsGraphModel bgmBoon))
            {
                BoonsStates = bgmBoon.ToList();
            }
            if (actor.GetBuffGraphs(log).TryGetValue(ProfHelper.NumberOfConditionsID, out BuffsGraphModel bgmCondition))
            {
                ConditionsStates = bgmCondition.ToList();
            }
            // Rotation
            Rotation = JsonRotation.BuildRotation(actor.GetCastLogs(log, 0, log.FightData.FightDuration), description);
            // Damage dist
            DamageDistributionData = new JsonDamageDistData(log, actor, description, targets);
            // Stats
            Statistics = new JsonStatistics(log, actor, targets, allies);
            //
            UniqueID = actor.AgentItem.UniqueID;
        }
    }
}
