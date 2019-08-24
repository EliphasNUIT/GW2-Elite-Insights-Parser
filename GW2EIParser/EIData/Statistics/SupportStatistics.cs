﻿using System;
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
    public static class SupportStatistics
    {

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


        private static long[] GetCleanses(AbstractSingleActor actor, ParsedLog log, PhaseData phase, AbstractSingleActor target, Dictionary<long, List<AbstractBuffEvent>> buffsPerId)
        {
            if (target == null)
            {
                return GetCleanses(actor, log, phase, buffsPerId);
            }
            long[] cleanse = { 0, 0 };
            foreach (long id in log.Buffs.BuffsByNature[Buff.BuffNature.Condition].Select(x => x.ID))
            {
                if (buffsPerId.TryGetValue(id, out var list))
                {
                    foreach (AbstractBuffEvent bf in list)
                    {
                        if (bf.To == target.AgentItem && bf.Time >= phase.Start && bf.Time <= phase.End && bf is BuffRemoveAllEvent bra)
                        {
                            cleanse[0]++;
                            cleanse[1] += Math.Max(bra.RemovedDuration, log.FightData.FightDuration);
                        }
                    }
                }
            }
            return cleanse;
        }
        private static long[] GetCleanses(AbstractSingleActor actor, ParsedLog log, PhaseData phase, Dictionary<long, List<AbstractBuffEvent>> buffsPerId)
        {
            long[] cleanse = { 0, 0 };
            foreach (long id in log.Buffs.BuffsByNature[Buff.BuffNature.Condition].Select(x => x.ID))
            {
                if (buffsPerId.TryGetValue(id, out var list))
                {
                    foreach (AbstractBuffEvent bf in list)
                    {
                        if (bf.To != GeneralHelper.UnknownAgent && bf.Time >= phase.Start && bf.Time <= phase.End && bf is BuffRemoveAllEvent bra)
                        {
                            cleanse[0]++;
                            cleanse[1] += Math.Max(bra.RemovedDuration, log.FightData.FightDuration);
                        }
                    }
                }
            }
            return cleanse;
        }

        private static long[] GetBoonStrips(AbstractSingleActor actor, ParsedLog log, PhaseData phase, Dictionary<long, List<AbstractBuffEvent>> buffsPerId)
        {
            long[] strips = { 0, 0 };
            foreach (long id in log.Buffs.BuffsByNature[Buff.BuffNature.Boon].Select(x => x.ID))
            {
                if (buffsPerId.TryGetValue(id, out var list))
                {
                    foreach (AbstractBuffEvent bf in list)
                    {
                        if (bf.To != GeneralHelper.UnknownAgent && bf.Time >= phase.Start && bf.Time <= phase.End && bf is BuffRemoveAllEvent bra)
                        {
                            strips[0]++;
                            strips[1] += Math.Max(bra.RemovedDuration, log.FightData.FightDuration);
                        }
                    }
                }
            }
            return strips;
        }
        private static long[] GetBoonStrips(AbstractSingleActor actor, ParsedLog log, PhaseData phase, AbstractSingleActor target, Dictionary<long, List<AbstractBuffEvent>> buffsPerId)
        {
            if (target == null)
            {
                return GetBoonStrips(actor, log, phase, buffsPerId);
            }
            long[] strips = { 0, 0 };
            foreach (long id in log.Buffs.BuffsByNature[Buff.BuffNature.Boon].Select(x => x.ID))
            {
                if (buffsPerId.TryGetValue(id, out var list))
                {
                    foreach (AbstractBuffEvent bf in list)
                    {
                        if (bf.To == target.AgentItem && bf.Time >= phase.Start && bf.Time <= phase.End && bf is BuffRemoveAllEvent bra)
                        {
                            strips[0]++;
                            strips[1] += Math.Max(bra.RemovedDuration, log.FightData.FightDuration);
                        }
                    }
                }
            }
            return strips;
        }

        private static long[] GetReses(AbstractSingleActor actor, ParsedLog log, long start, long end)
        {
            List<AbstractCastEvent> cls = actor.GetCastLogs(log, start, end);
            long[] reses = { 0, 0 };
            foreach (AbstractCastEvent cl in cls)
            {
                if (cl.SkillId == SkillItem.ResurrectId)
                {
                    reses[0]++;
                    reses[1] += cl.ActualDuration;
                }
            }
            return reses;
        }

        private static void FillFinalSupport(AbstractSingleActor actor, FinalSupport finalSupport, ParsedLog log, PhaseData phase, AbstractSingleActor target, Dictionary<long, List<AbstractBuffEvent>> buffsPerId)
        {

            long[] cleanseArray = GetCleanses(actor, log, phase, target, buffsPerId);
            long[] boonStrips = GetBoonStrips(actor, log, phase, target, buffsPerId);
            finalSupport.CondiCleanse = cleanseArray[0];
            finalSupport.CondiCleanseTime = cleanseArray[1] / 1000.0;
            finalSupport.BoonStrips = boonStrips[0];
            finalSupport.BoonStripsTime = boonStrips[1] / 1000.0;
        }

        public static List<FinalSupportAll> GetFinalSupport(AbstractSingleActor actor, ParsedLog log, Dictionary<long, List<AbstractBuffEvent>> buffsPerId)
        {
            List<FinalSupportAll> res = new List<FinalSupportAll>();
            List<PhaseData> phases = log.FightData.GetPhases(log);
            for (int phaseIndex = 0; phaseIndex < phases.Count; phaseIndex++)
            {
                FinalSupportAll final = new FinalSupportAll();
                res.Add(final);
                PhaseData phase = phases[phaseIndex];

                long[] resArray = GetReses(actor, log, phase.Start, phase.End);
                final.Resurrects = resArray[0];
                final.ResurrectTime = resArray[1] / 1000.0;
                FillFinalSupport(actor, final, log, phase, null, buffsPerId);
            }
            return res;
        }
        public static List<FinalSupport> GetFinalSupport(AbstractSingleActor actor, ParsedLog log, AbstractSingleActor target, Dictionary<long, List<AbstractBuffEvent>> buffsPerId)
        {
            List<FinalSupport> res = new List<FinalSupport>();
            List<PhaseData> phases = log.FightData.GetPhases(log);
            for (int phaseIndex = 0; phaseIndex < phases.Count; phaseIndex++)
            {
                FinalSupport final = new FinalSupport();
                res.Add(final);
                PhaseData phase = phases[phaseIndex];
                FillFinalSupport(actor, final, log, phase, target, buffsPerId);
            }
            return res;
        }
    }
}
