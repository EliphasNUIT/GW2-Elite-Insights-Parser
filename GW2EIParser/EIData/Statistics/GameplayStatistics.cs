using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIParser.EIData;
using GW2EIParser.Parser;
using GW2EIParser.Parser.ParsedData;
using GW2EIParser.Parser.ParsedData.CombatEvents;
using static GW2EIParser.Builders.JsonModels.JsonStatistics;

namespace GW2EIParser.Models
{
    /// <summary>
    /// Passes statistical information about dps logs
    /// </summary>
    public static class GameplayStatistics
    {

        private static void FillFinalGameplay(List<AbstractDamageEvent> dls, JsonGameplay final)
        {

            // (x - 1) / x
            foreach (AbstractDamageEvent dl in dls)
            {
                final.DamageCount++;
                if (!(dl is NonDirectDamageEvent))
                {
                    if (dl.HasCrit)
                    {
                        final.CriticalCount++;
                        final.CriticalDamage += dl.Damage;
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
                    if (dl.ShieldDamage > 0)
                    {
                        final.BarrierDamage += dl.ShieldDamage;
                        final.BarrierCount++;
                    }
                    final.DirectDamageCount++;
                    if (dl.Skill.CanCrit)
                    {
                        final.CritableDirectDamageCount++;
                    }
                }
            }
        }

        public static List<JsonGameplayAll> GetFinalGameplay(AbstractSingleActor actor, ParsedLog log)
        {
            var res = new List<JsonGameplayAll>();
            List<PhaseData> phases = log.FightData.GetPhases(log);
            for (int i = 0; i < phases.Count; i++)
            {
                PhaseData phase = phases[i];

                var final = new JsonGameplayAll();
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
        public static List<JsonGameplay> GetFinalGameplay(AbstractSingleActor actor, ParsedLog log, AbstractSingleActor target)
        {
            var res = new List<JsonGameplay>();
            List<PhaseData> phases = log.FightData.GetPhases(log);
            for (int i = 0; i < phases.Count; i++)
            {
                PhaseData phase = phases[i];

                var final = new JsonGameplay();
                res.Add(final);
                FillFinalGameplay(actor.GetJustActorDamageLogs(target, log, phase.Start, phase.End), final);
            }
            return res;
        }
    }
}
