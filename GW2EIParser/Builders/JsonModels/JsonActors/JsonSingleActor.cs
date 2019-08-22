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
    /// Base class single actors
    /// </summary>
    public abstract class JsonSingleActor : JsonActor
    {
        /// <summary>
        /// Array of int[2] that represents the number of conditions status \n
        /// Value[i][0] will be the time, value[i][1] will be the number of conditions present from value[i][0] to value[i+1][0] \n
        /// If i corresponds to the last element that means the status did not change for the remainder of the fight \n
        /// </summary>
        public List<int[]> ConditionsStates { get; set; }
        /// <summary>
        /// Array of int[2] that represents the number of boons status \n
        /// Value[i][0] will be the time, value[i][1] will be the number of boons present from value[i][0] to value[i+1][0] \n
        /// If i corresponds to the last element that means the status did not change for the remainder of the fight
        /// </summary>
        public List<int[]> BoonsStates { get; set; }

        protected JsonSingleActor(ParsedLog log, AbstractSingleActor actor, Dictionary<string, SkillDesc> skillMap, Dictionary<string, BuffDesc> buffMap, IEnumerable<AbstractMasterActor> targets) : base(log, actor, skillMap, buffMap, targets)
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
        }
    }
}
