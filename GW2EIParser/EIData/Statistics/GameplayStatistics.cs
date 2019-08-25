using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIParser.EIData;
using GW2EIParser.Parser;
using GW2EIParser.Parser.ParsedData;
using GW2EIParser.Parser.ParsedData.CombatEvents;

namespace GW2EIParser.Models
{
    /// <summary>
    /// Passes statistical information about dps logs
    /// </summary>
    public static class GameplayStatistics
    {
        public class FinalGameplay
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

        public class FinalGameplayAll : FinalGameplay
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



        private static void FillFinalGameplay(List<AbstractDamageEvent> dls, FinalGameplay final)
        {
            var nonCritable = new HashSet<long>
                    {
                        9292,
                        5492,
                        13014,
                        30770,
                        52370
                    };
            // (x - 1) / x
            foreach (AbstractDamageEvent dl in dls)
            {
                if (!(dl is NonDirectDamageEvent))
                {
                    if (dl.HasCrit)
                    {
                        final.CriticalCount++;
                        final.CriticalDmg += dl.Damage;
                    }

                    if (dl.IsFlanking)
                    {
                        final.FlankingCount++;
                    }

                    if (dl.HasGlanced)
                    {
                        final.GlanceCount++;
                    }

                    if (dl.IsBlind)
                    {
                        final.Missed++;
                    }
                    if (dl.HasInterrupted)
                    {
                        final.Interrupts++;
                    }

                    if (dl.IsAbsorbed)
                    {
                        final.Invulned++;
                    }
                    final.DamageAgainstBarrier += dl.ShieldDamage;
                    final.DirectDamageCount++;
                    if (!nonCritable.Contains(dl.SkillId))
                    {
                        final.CritableDirectDamageCount++;
                    }
                }
            }
        }

        public static List<FinalGameplayAll> GetFinalGameplay(AbstractSingleActor actor, ParsedLog log)
        {
            var res = new List<FinalGameplayAll>();
            List<PhaseData> phases = log.FightData.GetPhases(log);
            for (int i = 0; i < phases.Count; i++)
            {
                PhaseData phase = phases[i];

                var final = new FinalGameplayAll();
                res.Add(final);
                FillFinalGameplay(actor.GetJustActorDamageLogs(null, log, phase.Start, phase.End), final);
                // If conjured sword, stop
                if (actor.IsFakeActor)
                {
                    continue;
                }
                foreach (AbstractCastEvent cl in actor.GetCastLogs(log, phase.Start, phase.End))
                {
                    if (cl.Interrupted)
                    {
                        final.Wasted++;
                        final.TimeWasted += cl.ActualDuration;
                    }
                    if (cl.ReducedAnimation)
                    {
                        if (cl.ActualDuration < cl.ExpectedDuration)
                        {
                            final.Saved++;
                            final.TimeSaved += cl.ExpectedDuration - cl.ActualDuration;
                        }
                    }
                    if (cl.SkillId == SkillItem.WeaponSwapId)
                    {
                        final.SwapCount++;
                    }
                }
                final.TimeSaved = Math.Round(final.TimeSaved / 1000.0, GeneralHelper.TimeDigit);
                final.TimeWasted = Math.Round(final.TimeWasted / 1000.0, GeneralHelper.TimeDigit);

                double avgBoons = 0;
                foreach (long duration in actor.GetBuffPresence(log, i).Where(x => log.Buffs.BuffsByIds[x.Key].Nature == Buff.BuffNature.Boon).Select(x => x.Value))
                {
                    avgBoons += duration;
                }
                final.AvgBoons = Math.Round(avgBoons / phase.DurationInMS, GeneralHelper.BoonDigit);
                long activeDuration = phase.GetActorActiveDuration(actor, log);
                final.AvgActiveBoons = activeDuration > 0 ? Math.Round(avgBoons / activeDuration, GeneralHelper.BoonDigit) : 0.0;

                double avgCondis = 0;
                foreach (long duration in actor.GetBuffPresence(log, i).Where(x => log.Buffs.BuffsByIds[x.Key].Nature == Buff.BuffNature.Condition).Select(x => x.Value))
                {
                    avgCondis += duration;
                }
                final.AvgConditions = Math.Round(avgCondis / phase.DurationInMS, GeneralHelper.BoonDigit);
                final.AvgActiveConditions = activeDuration > 0 ? Math.Round(avgCondis / activeDuration, GeneralHelper.BoonDigit) : 0.0;
            }
            return res;
        }
        public static List<FinalGameplay> GetFinalGameplay(AbstractSingleActor actor, ParsedLog log, AbstractSingleActor target)
        {
            var res = new List<FinalGameplay>();
            List<PhaseData> phases = log.FightData.GetPhases(log);
            for (int i = 0; i < phases.Count; i++)
            {
                PhaseData phase = phases[i];

                var final = new FinalGameplay();
                res.Add(final);
                FillFinalGameplay(actor.GetJustActorDamageLogs(target, log, phase.Start, phase.End), final);
            }
            return res;
        }
    }
}
