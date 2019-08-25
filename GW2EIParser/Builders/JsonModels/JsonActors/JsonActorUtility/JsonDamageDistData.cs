using System.Collections.Generic;
using GW2EIParser.EIData;
using GW2EIParser.Parser;
using static GW2EIParser.Builders.JsonModels.JsonLog;

namespace GW2EIParser.Builders.JsonModels
{
    /// <summary>
    /// Class corresponding a damage distribution
    /// </summary>
    public class JsonDamageDistData
    {
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
        /// Length == # of targets for <see cref="JsonLog.Friendlies"/> or # of players for <see cref="JsonLog.Enemies"/> and the length of each sub array is equal to # of phases
        /// </summary>
        /// <seealso cref="JsonDamageDist"/>
        public List<List<List<JsonDamageDist>>> TargetDamageDists { get; set; } = new List<List<List<JsonDamageDist>>>();
        /// <summary>
        /// Per Target Damage Taken distribution array \n
        /// Length == # of targets for <see cref="JsonLog.Friendlies"/> or # of players for <see cref="JsonLog.Enemies"/> and the length of each sub array is equal to # of phases
        /// </summary>
        /// <seealso cref="JsonDamageDist"/>
        public List<List<List<JsonDamageDist>>> TargetDamageTakenDists { get; set; } = new List<List<List<JsonDamageDist>>>();

        public JsonDamageDistData(ParsedLog log, AbstractSingleActor actor, Dictionary<string, SkillDesc> skillMap, Dictionary<string, BuffDesc> buffMap, IEnumerable<AbstractSingleActor> targets)
        {
            List<PhaseData> phases = log.FightData.GetPhases(log);
            TotalDamageDists = new List<List<JsonDamageDist>>();
            TotalDamageTakenDists = new List<List<JsonDamageDist>>();
            foreach (PhaseData phase in phases)
            {
                TotalDamageDists.Add(JsonDamageDist.BuildJsonDamageDists(actor.GetDamageLogs(null, log, phase.Start, phase.End), log, skillMap, buffMap));
                TotalDamageTakenDists.Add(JsonDamageDist.BuildJsonDamageDists(actor.GetDamageTakenLogs(null, log, phase.Start, phase.End), log, skillMap, buffMap));
            }
            foreach (AbstractSingleActor target in targets)
            {
                var TargetDamageDist = new List<List<JsonDamageDist>>();
                var TargetDamageTakenDist = new List<List<JsonDamageDist>>();
                foreach (PhaseData phase in phases)
                {
                    TargetDamageDist.Add(JsonDamageDist.BuildJsonDamageDists(actor.GetDamageLogs(target, log, phase.Start, phase.End), log, skillMap, buffMap));
                    TargetDamageTakenDist.Add(JsonDamageDist.BuildJsonDamageDists(actor.GetDamageTakenLogs(target, log, phase.Start, phase.End), log, skillMap, buffMap));
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
