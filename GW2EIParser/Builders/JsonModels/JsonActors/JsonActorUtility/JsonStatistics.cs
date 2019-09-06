using System.Collections.Generic;
using GW2EIParser.EIData;
using GW2EIParser.Parser;
using static GW2EIParser.Builders.JsonModels.JsonLog;

namespace GW2EIParser.Builders.JsonModels
{
    public class JsonStatistics
    {
        /// <summary>
        /// Defensive stats 
        /// </summary>
        public class JsonDefense
        {
            /// <summary>
            /// Total damage taken
            /// </summary>
            public long TakenDamage { get; set; }
            /// <summary>
            /// Number of hits taken
            /// </summary>
            public long TakenCount { get; set; }
            /// <summary>
            /// Number of blocks
            /// </summary>
            public int BlockedCount { get; set; }
            /// <summary>
            /// Number of evades
            /// </summary>
            public int EvadedCount { get; set; }
            /// <summary>
            /// Number of time an incoming attack was negated by invul
            /// </summary>
            public int InvulnedCount { get; set; }
            /// <summary>
            /// Damage negated by invul
            /// </summary>
            public int InvulDamage { get; set; }
            /// <summary>
            /// Damage done against barrier
            /// </summary>
            public int BarrierDamage { get; set; }
            /// <summary>
            /// Number of hits done against barrier
            /// </summary>
            public int BarrierCount { get; set; }
            /// <summary>
            /// Number of time interrupted
            /// </summary>
            public int InterruptedCount { get; set; }
        }

        /// <summary>
        /// Defensive stats
        /// </summary>
        public class JsonDefenseAll : JsonDefense
        {
            /// <summary>
            /// Number of dodges
            /// </summary>
            public int DodgeCount { get; set; }
            /// <summary>
            /// Number of time downed
            /// </summary>
            public int DownCount { get; set; }
            /// <summary>
            /// Time passed in downstate
            /// </summary>
            public int DownDuration { get; set; }
            /// <summary>
            /// Number of time died
            /// </summary>
            public int DeadCount { get; set; }
            /// <summary>
            /// Time passed in dead state
            /// </summary>
            public int DeadDuration { get; set; }
            /// <summary>
            /// Number of time disconnected
            /// </summary>
            public int DcCount { get; set; }
            /// <summary>
            /// Time passed in disconnected state
            /// </summary>
            public int DcDuration { get; set; }
        }
        /// <summary>
        /// DPS stats
        /// </summary>
        public class JsonDPS
        {
            /// <summary>
            /// Total dps
            /// </summary>
            public int Dps { get; set; }
            /// <summary>
            /// Total damage
            /// </summary>
            public int Damage { get; set; }
            /// <summary>
            /// Total condi dps
            /// </summary>
            public int CondiDps { get; set; }
            /// <summary>
            /// Total condi damage
            /// </summary>
            public int CondiDamage { get; set; }
            /// <summary>
            /// Total power dps
            /// </summary>
            public int PowerDps { get; set; }
            /// <summary>
            /// Total power damage
            /// </summary>
            public int PowerDamage { get; set; }
            /// <summary>
            /// Total actor only dps, missing if same as Dps
            /// </summary>
            public int ActorDps { get; set; }
            /// <summary>
            /// Total actor only damage, missing if same as Damage
            /// </summary>
            public int ActorDamage { get; set; }
            /// <summary>
            /// Total actor only condi dps, missing if same as CondiDps
            /// </summary>
            public int ActorCondiDps { get; set; }
            /// <summary>
            /// Total actor only condi damage, missing if same as CondiDamage
            /// </summary>
            public int ActorCondiDamage { get; set; }
            /// <summary>
            /// Total actor only power dps, missing if same as PowerDps
            /// </summary>
            public int ActorPowerDps { get; set; }
            /// <summary>
            /// Total actor only power damage, missing if same as PowerDamage
            /// </summary>
            public int ActorPowerDamage { get; set; }

        }

        /// <summary>
        /// Gameplay stats
        /// </summary>
        public class JsonGameplay
        {
            /// <summary>
            /// Number of damage hit
            /// </summary>
            public int DamageCount { get; set; }
            /// <summary>
            /// Number of direct damage hit
            /// </summary>
            public int DirectDamageCount { get; set; }
            /// <summary>
            /// Number of critable hit
            /// </summary>
            public int CritableDirectDamageCount { get; set; }
            /// <summary>
            /// Number of crit
            /// </summary>
            public int CriticalCount { get; set; }
            /// <summary>
            /// Total critical damage
            /// </summary>
            public int CriticalDamage { get; set; }
            /// <summary>
            /// Number of hits while flanking
            /// </summary>
            public int FlankingCount { get; set; }
            /// <summary>
            /// Number of glanced hits
            /// </summary>
            public int GlanceCount { get; set; }
            /// <summary>
            /// Number of missed hits
            /// </summary>
            public int Missed { get; set; }
            /// <summary>
            /// Number of hits that interrupted a skill
            /// </summary>
            public int Interrupts { get; set; }
            /// <summary>
            /// Number of hits against invulnerable targets
            /// </summary>
            public int Invulned { get; set; }
            /// <summary>
            /// Damage done against barrier
            /// </summary>
            public int BarrierDamage { get; set; }
            /// <summary>
            /// Number of hits against barrier
            /// </summary>
            public int BarrierCount { get; set; }
        }

        public class JsonGameplayAll : JsonGameplay
        {
            /// <summary>
            /// Number of time you interrupted your cast
            /// </summary>
            public int Wasted { get; set; }
            /// <summary>
            /// Time wasted by interrupting your cast
            /// </summary>
            public double TimeWasted { get; set; }
            /// <summary>
            /// Number of time you skipped an aftercast
            /// </summary>
            public int Saved { get; set; }
            /// <summary>
            /// Time saved while skipping aftercast
            /// </summary>
            public double TimeSaved { get; set; }
            /// <summary>
            /// Average amount of boons
            /// </summary>
            public double AvgBoons { get; set; }
            /// <summary>
            /// Average amount of boons over active time
            /// </summary>
            public double AvgActiveBoons { get; set; }
            /// <summary>
            /// Average amount of conditions
            /// </summary>
            public double AvgConditions { get; set; }
            /// <summary>
            /// Average amount of conditions over active time
            /// </summary>
            public double AvgActiveConditions { get; set; }
            /// <summary>
            /// Number of time a weapon swap happened
            /// </summary>
            public int SwapCount { get; set; }
        }

        /// <summary>
        /// Support stats
        /// </summary>
        public class JsonSupport
        {
            /// <summary>
            /// Number of time a condition was removed
            /// </summary>
            public long CondiCleanse { get; set; }
            /// <summary>
            /// Total time of condition removed
            /// </summary>
            public double CondiCleanseTime { get; set; }
            /// <summary>
            /// Number of time a boon was removed
            /// </summary>
            public long BoonStrips { get; set; }
            /// <summary>
            /// Total time of boons removed
            /// </summary>
            public double BoonStripsTime { get; set; }
        }

        /// <summary>
        /// Support stats
        /// </summary>
        public class JsonSupportAll : JsonSupport
        {
            /// <summary>
            /// Number of time ressurected someone
            /// </summary>
            public long Resurrects { get; set; }
            /// <summary>
            /// Time passed on ressurecting
            /// </summary>
            public double ResurrectTime { get; set; }
        }

        /// <summary>
        /// Stats against all  \n
        /// Length == # of phases
        /// </summary>
        /// <seealso cref="JsonGameplayAll"/>
        public List<JsonGameplayAll> GameplayAll { get; set; }


        /// <summary>
        /// Defensive stats \n
        /// Length == # of phases
        /// </summary>
        /// <seealso cref="JsonDefenseAll"/>
        public List<JsonDefenseAll> DefenseAll { get; set; }

        /// <summary>
        /// Support stats \n
        /// Length == # of phases
        /// </summary>
        /// <seealso cref="JsonSupport"/>
        public List<JsonSupportAll> SupportAll { get; set; }

        /// <summary>
        /// Array of Total DPS stats \n
        /// Length == # of phases
        /// </summary>
        /// <seealso cref="JsonDPS"/>
        public List<JsonDPS> DpsAll { get; set; }

        public JsonStatistics(ParsedLog log, AbstractSingleActor actor)
        {
            DpsAll = actor.GetDPS(log);
            GameplayAll = actor.GetStats(log);
            DefenseAll = actor.GetDefenses(log);
            SupportAll = actor.GetSupport(log);
        }

    }
}
