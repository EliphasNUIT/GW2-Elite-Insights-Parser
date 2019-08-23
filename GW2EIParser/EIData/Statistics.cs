using System.Collections.Generic;
using GW2EIParser.EIData;

namespace GW2EIParser.Models
{
    /// <summary>
    /// Passes statistical information about dps logs
    /// </summary>
    public static class Statistics
    {
        public class FinalDPS
        {
            // Total
            public int Dps { get; set; }
            public int Damage { get; set; }
            public int CondiDps { get; set; }
            public int CondiDamage { get; set; }
            public int PowerDps { get; set; }
            public int PowerDamage { get; set; }
            // Actor only
            public int ActorDps { get; set; }
            public int ActorDamage { get; set; }
            public int ActorCondiDps { get; set; }
            public int ActorCondiDamage { get; set; }
            public int ActorPowerDps { get; set; }
            public int ActorPowerDamage { get; set; }
        }

        public class FinalStats
        {
            public int DirectDamageCount { get; set; }
            public int DamageAgainstBarrier { get; set; }
            public int CritableDirectDamageCount { get; set; }
            public int CriticalCount { get; set; }
            public int CriticalDmg { get; set; }
            public int FlankingCount { get; set; }
            public int GlanceCount { get; set; }
            public int Missed { get; set; }
            public int Interrupts { get; set; }
            public int Invulned { get; set; }
        }

        public class FinalStatsAll : FinalStats
        {
            // Rates
            public int Wasted { get; set; }
            public double TimeWasted { get; set; }
            public int Saved { get; set; }
            public double TimeSaved { get; set; }

            // boons
            public double AvgBoons { get; set; }
            public double AvgActiveBoons { get; set; }
            public double AvgConditions { get; set; }
            public double AvgActiveConditions { get; set; }

            // Counts
            public int SwapCount { get; set; }
        }

        public class FinalDefenses
        {
            public long DamageTaken { get; set; }
            public int BlockedCount { get; set; }
            public int EvadedCount { get; set; }
            public int InvulnedCount { get; set; }
            public int DamageInvulned { get; set; }
            public int DamageBarrier { get; set; }
            public int InterruptedCount { get; set; }
        }

        public class FinalDefensesAll : FinalDefenses
        {
            public int DodgeCount { get; set; }
            public int DownCount { get; set; }
            public int DownDuration { get; set; }
            public int DeadCount { get; set; }
            public int DeadDuration { get; set; }
            public int DcCount { get; set; }
            public int DcDuration { get; set; }
        }

        public class FinalSupport
        {
            public long CondiCleanse { get; set; }
            public double CondiCleanseTime { get; set; }
            public long BoonStrips { get; set; }
            public double BoonStripsTime { get; set; }
        }

        public class FinalSupportAll : FinalSupport
        {
            public long Resurrects { get; set; }
            public double ResurrectTime { get; set; }
        }

        public class FinalBuffs
        {
            public double Uptime { get; set; }
            public double Generation { get; set; }
            public double Overstack { get; set; }
            public double Wasted { get; set; }
            public double UnknownExtended { get; set; }
            public double ByExtension { get; set; }
            public double Extended { get; set; }
            public double Presence { get; set; }
        }

        public enum BuffEnum { Self, Group, OffGroup, Squad };

        public class FinalTargetBuffs
        {
            public FinalTargetBuffs(List<Player> plist)
            {
                Uptime = 0;
                Presence = 0;
                Generated = new Dictionary<Player, double>();
                Overstacked = new Dictionary<Player, double>();
                Wasted = new Dictionary<Player, double>();
                UnknownExtension = new Dictionary<Player, double>();
                Extension = new Dictionary<Player, double>();
                Extended = new Dictionary<Player, double>();
                foreach (Player p in plist)
                {
                    Generated.Add(p, 0);
                    Overstacked.Add(p, 0);
                    Wasted.Add(p, 0);
                    UnknownExtension.Add(p, 0);
                    Extension.Add(p, 0);
                    Extended.Add(p, 0);
                }
            }

            public double Uptime { get; set; }
            public double Presence { get; set; }
            public Dictionary<Player, double> Generated { get; }
            public Dictionary<Player, double> Overstacked { get; }
            public Dictionary<Player, double> Wasted { get; }
            public Dictionary<Player, double> UnknownExtension { get; }
            public Dictionary<Player, double> Extension { get; }
            public Dictionary<Player, double> Extended { get; }
        }

        public class DamageModifierData
        {
            public int HitCount { get; }
            public int TotalHitCount { get; }
            public double DamageGain { get; }
            public int TotalDamage { get; }

            public DamageModifierData(int hitCount, int totalHitCount, double damageGain, int totalDamage)
            {
                HitCount = hitCount;
                TotalHitCount = totalHitCount;
                DamageGain = damageGain;
                TotalDamage = totalDamage;
            }
        }


        public class Consumable
        {
            public Buff Buff { get; }
            public long Time { get; }
            public int Duration { get; }
            public int Stack { get; set; }
            public bool Initial { get; set; }

            public Consumable(Buff item, long time, int duration, bool initial)
            {
                Buff = item;
                Time = time;
                Duration = duration;
                Stack = 1;
                Initial = initial;
            }
        }

        public class DeathRecap
        {
            public class DeathRecapDamageItem
            {
                public long ID { get; set; }
                public bool IndirectDamage { get; set; }
                public string Src { get; set; }
                public int Damage { get; set; }
                public int Time { get; set; }
            }

            public int DeathTime { get; set; }
            public List<DeathRecapDamageItem> ToDown { get; set; }
            public List<DeathRecapDamageItem> ToKill { get; set; }
        }

    }
}
