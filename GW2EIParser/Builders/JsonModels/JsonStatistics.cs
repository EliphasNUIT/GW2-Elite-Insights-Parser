using GW2EIParser.Models;

namespace GW2EIParser.Builders.JsonModels
{
    public class JsonStatistics
    {
        /// <summary>
        /// Defensive stats 
        /// </summary>
        public class JsonDefenses
        {
            /// <summary>
            /// Total damage taken
            /// </summary>
            public long DamageTaken { get; set; }
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
            public int DamageInvulned { get; set; }
            /// <summary>
            /// Damage done against barrier
            /// </summary>
            public int DamageBarrier { get; set; }
            /// <summary>
            /// Number of time interrupted
            /// </summary>
            public int InterruptedCount { get; set; }

            public JsonDefenses(Statistics.FinalDefenses defenses)
            {
                DamageTaken = defenses.DamageTaken;
                BlockedCount = defenses.BlockedCount;
                EvadedCount = defenses.EvadedCount;
                InvulnedCount = defenses.InvulnedCount;
                DamageInvulned = defenses.DamageInvulned;
                DamageBarrier = defenses.DamageBarrier;
                InterruptedCount = defenses.InterruptedCount;
            }
        }

        /// <summary>
        /// Defensive stats
        /// </summary>
        public class JsonDefensesAll: JsonDefenses
        {
            /// <summary>
            /// Number of dodges
            /// </summary>
            public int DodgeCount { get; set; }
            /// <summary>
            /// Number of time an incoming attack was negated by invul
            /// </summary>
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

            public JsonDefensesAll(Statistics.FinalDefensesAll defenses) : base(defenses)
            {
                DodgeCount = defenses.DodgeCount;
                DownCount = defenses.DownCount;
                DownDuration = defenses.DownDuration;
                DeadCount = defenses.DeadCount;
                DeadDuration = defenses.DeadDuration;
                DcCount = defenses.DcCount;
                DcDuration = defenses.DcDuration;
            }
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
            /// Total actor only dps
            /// </summary>
            public int ActorDps { get; set; }
            /// <summary>
            /// Total actor only damage
            /// </summary>
            public int ActorDamage { get; set; }
            /// <summary>
            /// Total actor only condi dps
            /// </summary>
            public int ActorCondiDps { get; set; }
            /// <summary>
            /// Total actor only condi damage
            /// </summary>
            public int ActorCondiDamage { get; set; }
            /// <summary>
            /// Total actor only power dps
            /// </summary>
            public int ActorPowerDps { get; set; }
            /// <summary>
            /// Total actor only power damage
            /// </summary>
            public int ActorPowerDamage { get; set; }

            public JsonDPS(Statistics.FinalDPS stats)
            {
                Dps = stats.Dps;
                Damage = stats.Damage;
                CondiDps = stats.CondiDps;
                CondiDamage = stats.CondiDamage;
                PowerDps = stats.PowerDps;
                PowerDamage = stats.PowerDamage;

                ActorDps = stats.ActorDps;
                ActorDamage = stats.ActorDamage;
                ActorCondiDps = stats.ActorCondiDps;
                ActorCondiDamage = stats.ActorCondiDamage;
                ActorPowerDps = stats.ActorPowerDps;
                ActorPowerDamage = stats.ActorPowerDamage;
            }

        }

        /// <summary>
        /// Gameplay stats
        /// </summary>
        public class JsonStats
        {
            /// <summary>
            /// Number of direct damage hit
            /// </summary>
            public int DirectDamageCount;
            /// <summary>
            /// Number of critable hit
            /// </summary>
            public int CritableDirectDamageCount;
            /// <summary>
            /// Number of crit
            /// </summary>
            public int CriticalRate;
            /// <summary>
            /// Total critical damage
            /// </summary>
            public int CriticalDmg;
            /// <summary>
            /// Number of hits while flanking
            /// </summary>
            public int FlankingRate;
            /// <summary>
            /// Number of glanced hits
            /// </summary>
            public int GlanceRate;
            /// <summary>
            /// Number of missed hits
            /// </summary>
            public int Missed;
            /// <summary>
            /// Number of hits that interrupted a skill
            /// </summary>
            public int Interrupts;
            /// <summary>
            /// Number of hits against invulnerable targets
            /// </summary>
            public int Invulned;

            public JsonStats(Statistics.FinalStats stats)
            {
                DirectDamageCount = stats.DirectDamageCount;
                CritableDirectDamageCount = stats.CritableDirectDamageCount;
                CriticalRate = stats.CriticalCount;
                CriticalDmg = stats.CriticalDmg;
                FlankingRate = stats.FlankingCount;
                GlanceRate = stats.GlanceCount;
                Missed = stats.Missed;
                Interrupts = stats.Interrupts;
                Invulned = stats.Invulned;
            }

            public JsonStats(Statistics.FinalStatsAll stats)
            {
                DirectDamageCount = stats.DirectDamageCount;
                CritableDirectDamageCount = stats.CritableDirectDamageCount;
                CriticalRate = stats.CriticalCount;
                CriticalDmg = stats.CriticalDmg;
                FlankingRate = stats.FlankingCount;
                GlanceRate = stats.GlanceCount;
                Missed = stats.Missed;
                Interrupts = stats.Interrupts;
                Invulned = stats.Invulned;
            }
        }

        public class JsonStatsAll : JsonStats
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
            /// Distance to the epicenter of the squad
            /// </summary>
            public double StackDist { get; set; }
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

            public JsonStatsAll(Statistics.FinalStatsAll stats) : base(stats)
            {
                Wasted = stats.Wasted;
                TimeWasted = stats.TimeWasted;
                Saved = stats.Saved;
                TimeSaved = stats.TimeSaved;
                StackDist = stats.StackDist;
                AvgBoons = stats.AvgBoons;
                AvgActiveBoons = stats.AvgActiveBoons;
                AvgConditions = stats.AvgConditions;
                AvgActiveConditions = stats.AvgActiveConditions;
                SwapCount = stats.SwapCount;
            }
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

            public JsonSupport(Statistics.FinalSupport stats)
            {
                CondiCleanse = stats.CondiCleanse;
                CondiCleanseTime = stats.CondiCleanseTime;
                BoonStrips = stats.BoonStrips;
                BoonStripsTime = stats.BoonStripsTime;
            }
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

            public JsonSupportAll(Statistics.FinalSupportAll stats) : base(stats)
            {
                Resurrects = stats.Resurrects;
                ResurrectTime = stats.ResurrectTime;
            }
        }
    }
}
