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
    /// Base class for Players and Targets
    /// </summary>
    /// <seealso cref="JsonPlayer"/> 
    /// <seealso cref="JsonNPC"/>
    public abstract class JsonActor
    {

        /// <summary>
        /// Name of the actor
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Condition damage score
        /// </summary>
        public uint Condition { get; set; }
        /// <summary>
        /// Concentration score
        /// </summary>
        public uint Concentration { get; set; }
        /// <summary>
        /// Healing Power score
        /// </summary>
        public uint Healing { get; set; }
        /// <summary>
        /// Toughness score
        /// </summary>
        public uint Toughness { get; set; }
        /// <summary>
        /// Height of the hitbox
        /// </summary>
        public uint HitboxHeight { get; set; }
        /// <summary>
        /// Width of the hitbox
        /// </summary>
        public uint HitboxWidth { get; set; }
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

        protected JsonActor(ParsedLog log, AbstractActor actor, Dictionary<string, SkillDesc> skillMap, Dictionary<string, BuffDesc> buffMap, IEnumerable<AbstractMasterActor> targets)
        {
            List<PhaseData> phases = log.FightData.GetPhases(log);
            // Meta data
            Name = actor.Character;
            Condition = actor.Condition;
            Concentration = actor.Concentration;
            Healing = actor.Healing;
            Toughness = actor.Toughness;
            HitboxHeight = actor.HitboxHeight;
            HitboxWidth = actor.HitboxWidth;
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
