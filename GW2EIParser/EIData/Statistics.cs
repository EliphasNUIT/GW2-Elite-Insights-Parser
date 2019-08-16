﻿using System.Collections.Generic;
using System.Linq;
using GW2EIParser.EIData;
using GW2EIParser.Parser;
using GW2EIParser.Parser.ParsedData;
using GW2EIParser.Parser.ParsedData.CombatEvents;
using static GW2EIParser.EIData.Buff;

namespace GW2EIParser.Models
{
    /// <summary>
    /// Passes statistical information about dps logs
    /// </summary>
    public class Statistics
    {
        public Statistics(CombatData combatData, List<Player> players, BuffsContainer boons)
        {
            HashSet<long> skillIDs = combatData.GetSkills();
            // Main boons
            foreach (Buff boon in boons.BoonsByNature[BoonNature.Boon])
            {
                if (skillIDs.Contains(boon.ID))
                {
                    PresentBoons.Add(boon);
                }
            }
            // Main Conditions
            foreach (Buff boon in boons.BoonsByNature[BoonNature.Condition])
            {
                if (skillIDs.Contains(boon.ID))
                {
                    PresentConditions.Add(boon);
                }
            }

            // Important class specific boons
            foreach (Buff boon in boons.BoonsByNature[BoonNature.OffensiveBuffTable])
            {
                if (skillIDs.Contains(boon.ID))
                {
                    PresentOffbuffs.Add(boon);
                }
            }

            foreach (Buff boon in boons.BoonsByNature[BoonNature.DefensiveBuffTable])
            {
                if (skillIDs.Contains(boon.ID))
                {
                    PresentDefbuffs.Add(boon);
                }

            }

            // All class specific boons
            Dictionary<long, Buff> remainingBuffsByIds = boons.BoonsByNature[BoonNature.GraphOnlyBuff].GroupBy(x => x.ID).ToDictionary(x => x.Key, x => x.ToList().FirstOrDefault());
            foreach (Player player in players)
            {
                PresentPersonalBuffs[player.InstID] = new HashSet<Buff>();
                foreach (AbstractBuffEvent item in combatData.GetBuffDataByDst(player.AgentItem))
                {
                    if (item is BuffApplyEvent && item.To == player.AgentItem && remainingBuffsByIds.TryGetValue(item.BuffID, out Buff boon))
                    {
                        PresentPersonalBuffs[player.InstID].Add(boon);
                    }
                }
            }
        }

        public class FinalDPS
        {
            // Total
            public int Dps;
            public int Damage;
            public int CondiDps;
            public int CondiDamage;
            public int PowerDps;
            public int PowerDamage;
            // Actor only
            public int ActorDps;
            public int ActorDamage;
            public int ActorCondiDps;
            public int ActorCondiDamage;
            public int ActorPowerDps;
            public int ActorPowerDamage;
        }

        public class FinalStats
        {
            public int DirectDamageCount;
            public int CritableDirectDamageCount;
            public int CriticalCount;
            public int CriticalDmg;
            public int FlankingCount;
            public int GlanceCount;
            public int Missed;
            public int Interrupts;
            public int Invulned;
        }

        public class FinalStatsAll : FinalStats
        {
            // Rates
            public int Wasted;
            public double TimeWasted;
            public int Saved;
            public double TimeSaved;
            public double StackDist;

            // boons
            public double AvgBoons;
            public double AvgActiveBoons;
            public double AvgConditions;
            public double AvgActiveConditions;

            // Counts
            public int SwapCount;
        }

        public class FinalDefenses
        {
            public long DamageTaken;
            public int BlockedCount;
            public int EvadedCount;
            public int InvulnedCount;
            public int DamageInvulned;
            public int DamageBarrier;
            public int InterruptedCount;
        }

        public class FinalDefensesAll : FinalDefenses
        {
            public int DodgeCount;
            public int DownCount;
            public int DownDuration;
            public int DeadCount;
            public int DeadDuration;
            public int DcCount;
            public int DcDuration;
        }

        public class FinalSupport
        {
            public long CondiCleanse;
            public double CondiCleanseTime;
            public long BoonStrips;
            public double BoonStripsTime;
        }

        public class FinalSupportAll: FinalSupport
        {
            public long Resurrects;
            public double ResurrectTime;
        }

        public class FinalBuffs
        {
            public double Uptime;
            public double Generation;
            public double Overstack;
            public double Wasted;
            public double UnknownExtended;
            public double ByExtension;
            public double Extended;
            public double Presence;
        }

        public enum BuffEnum { Self, Group, OffGroup, Squad};

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

            public double Uptime;
            public double Presence;
            public readonly Dictionary<Player, double> Generated;
            public readonly Dictionary<Player, double> Overstacked;
            public readonly Dictionary<Player, double> Wasted;
            public readonly Dictionary<Player, double> UnknownExtension;
            public readonly Dictionary<Player, double> Extension;
            public readonly Dictionary<Player, double> Extended;
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

            public Consumable(Buff item, long time, int duration)
            {
                Buff = item;
                Time = time;
                Duration = duration;
                Stack = 1;
            }
        }

        public class DeathRecap
        {
            public class DeathRecapDamageItem
            {
                public long ID;
                public bool IndirectDamage;
                public string Src;
                public int Damage;
                public int Time;
            }

            public int DeathTime;
            public List<DeathRecapDamageItem> ToDown;
            public List<DeathRecapDamageItem> ToKill;
        }

        // present buff
        public readonly List<Buff> PresentBoons = new List<Buff>();//Used only for Boon tables
        public readonly List<Buff> PresentConditions = new List<Buff>();//Used only for Condition tables
        public readonly List<Buff> PresentOffbuffs = new List<Buff>();//Used only for Off Buff tables
        public readonly List<Buff> PresentDefbuffs = new List<Buff>();//Used only for Def Buff tables
        public readonly Dictionary<ushort, HashSet<Buff>> PresentPersonalBuffs = new Dictionary<ushort, HashSet<Buff>>();

        //Positions for group
        private List<Point3D> _stackCenterPositions = null;

        public List<Point3D> GetStackCenterPositions(ParsedLog log)
        {
            if (_stackCenterPositions == null)
            {
                SetStackCenterPositions(log);
            }
            return _stackCenterPositions;
        }

        private void SetStackCenterPositions(ParsedLog log)
        {
            _stackCenterPositions = new List<Point3D>();
            if (log.CombatData.HasMovementData)
            {
                List<List<Point3D>> GroupsPosList = new List<List<Point3D>>();
                foreach (Player player in log.PlayerList)
                {
                    if (player.IsFakeActor)
                    {
                        continue;
                    }
                    GroupsPosList.Add(player.GetCombatReplayActivePositions(log));
                }
                for (int time = 0; time < GroupsPosList[0].Count; time++)
                {
                    float x = 0;
                    float y = 0;
                    float z = 0;
                    int activePlayers = GroupsPosList.Count;
                    foreach (List<Point3D> points in GroupsPosList)
                    {
                        Point3D point = points[time];
                        if (point != null)
                        {
                            x += point.X;
                            y += point.Y;
                            z += point.Z;
                        }
                        else
                        {
                            activePlayers--;
                        }

                    }
                    x /= activePlayers;
                    y /= activePlayers;
                    z /= activePlayers;
                    _stackCenterPositions.Add(new Point3D(x, y, z, GeneralHelper.PollingRate * time));
                }
            }
        }
    }
}
