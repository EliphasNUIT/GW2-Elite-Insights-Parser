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
        /// <summary>
        /// Total Damage distribution array \n
        /// Length == # of phases
        /// </summary>
        /// <seealso cref="JsonDamageDist"/>
        public List<List<JsonDamageDist>> TotalDamageDists { get; set; }
        /// <summary>
        /// Damage taken array
        /// Length == # of phases
        /// </summary>
        /// <seealso cref="JsonDamageDist"/>
        public List<List<JsonDamageDist>> TotalDamageTakenDists { get; set; }
        /// <summary>
        /// Per Target Damage distribution array \n
        /// Length == # of targets for <seealso cref="JsonLog.Friendlies"/> or # of players for <seealso cref="JsonLog.Enemies"/> and the length of each sub array is equal to # of phases
        /// </summary>
        /// <seealso cref="JsonDamageDist"/>
        public List<List<List<JsonDamageDist>>> TargetDamageDists { get; set; }
        /// <summary>
        /// Per Target Damage Taken distribution array \n
        /// Length == # of targets for <seealso cref="JsonLog.Friendlies"/> or # of players for <seealso cref="JsonLog.Enemies"/> and the length of each sub array is equal to # of phases
        /// </summary>
        /// <seealso cref="JsonDamageDist"/>
        public List<List<List<JsonDamageDist>>> TargetDamageTakenDists { get; set; }
        /// <summary>
        /// Rotation data
        /// </summary>
        /// <seealso cref="JsonRotation"/>
        public List<JsonRotation> Rotation { get; set; }
        /// <summary>
        /// Stats against all  \n
        /// Length == # of phases
        /// </summary>
        /// <seealso cref="JsonStatsAll"/>
        public JsonStatsAll[] StatsAll { get; set; }
        /// <summary>
        /// Stats against targets  \n
        /// Length == # of targets for <seealso cref="JsonLog.Friendlies"/> or # of players for <seealso cref="JsonLog.Enemies"/> and the length of each sub array is equal to # of phases
        /// </summary>
        /// <seealso cref="JsonStats"/>
        public JsonStats[][] StatsTargets { get; set; }
        /// <summary>
        /// Defensive stats \n
        /// Length == # of phases
        /// </summary>
        /// <seealso cref="JsonDefenses"/>
        public JsonDefenses[] Defenses { get; set; }
        /// <summary>
        /// Support stats \n
        /// Length == # of phases
        /// </summary>
        /// <seealso cref="JsonSupport"/>
        public JsonSupport[] Support { get; set; }
        /// <summary>
        /// Array of Total DPS stats \n
        /// Length == # of phases
        /// </summary>
        /// <seealso cref="JsonDPS"/>
        public JsonDPS[] DpsAll { get; set; }
        /// <summary>
        /// Array of Total DPS stats \n
        /// Length == # of targets for <seealso cref="JsonLog.Friendlies"/> or # of players <seealso cref="JsonLog.Enemies"/> and the length of each sub array is equal to # of phases
        /// </summary>
        /// <seealso cref="JsonDPS"/>
        public JsonDPS[][] DpsTargets { get; set; }

        protected JsonSingleActor(ParsedLog log, AbstractSingleActor actor, Dictionary<string, SkillDesc> skillMap, Dictionary<string, BuffDesc> buffMap, IEnumerable<AbstractMasterActor> targets) : base(actor)
        {
            List<PhaseData> phases = log.FightData.GetPhases(log);
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
            Rotation = JsonRotation.BuildRotation(actor.GetCastLogs(log, 0, log.FightData.FightDuration), skillMap);
            // Damage dist
            TotalDamageDists = new List<List<JsonDamageDist>>();
            TotalDamageTakenDists = new List<List<JsonDamageDist>>();
            foreach (PhaseData phase in phases)
            {
                TotalDamageDists.Add(JsonDamageDist.ComputeJsonDamageDists(actor.GetDamageLogs(null, log, phase.Start, phase.End), log, skillMap, buffMap));
                TotalDamageTakenDists.Add(JsonDamageDist.ComputeJsonDamageDists(actor.GetDamageTakenLogs(null, log, phase.Start, phase.End), log, skillMap, buffMap));
            }
            TargetDamageDists = new List<List<List<JsonDamageDist>>>();
            TargetDamageTakenDists = new List<List<List<JsonDamageDist>>>();
            foreach (AbstractMasterActor target in targets)
            {
                List<List<JsonDamageDist>> TargetDamageDist = new List<List<JsonDamageDist>>();
                List<List<JsonDamageDist>> TargetDamageTakenDist = new List<List<JsonDamageDist>>();
                foreach (PhaseData phase in phases)
                {
                    TargetDamageDist.Add(JsonDamageDist.ComputeJsonDamageDists(actor.GetDamageLogs(target, log, phase.Start, phase.End), log, skillMap, buffMap));
                    TargetDamageTakenDist.Add(JsonDamageDist.ComputeJsonDamageDists(actor.GetDamageTakenLogs(target, log, phase.Start, phase.End), log, skillMap, buffMap));
                }
                TargetDamageDists.Add(TargetDamageDist);
                TargetDamageTakenDists.Add(TargetDamageTakenDist);
            }
            if (TargetDamageDists.Count == 0)
            {
                TargetDamageDists = null;
            }
            if (TargetDamageTakenDists.Count == 0)
            {
                TargetDamageTakenDists = null;
            }
        }
    }
}
