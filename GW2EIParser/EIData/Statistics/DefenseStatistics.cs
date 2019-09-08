using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIParser.EIData;
using GW2EIParser.Parser;
using GW2EIParser.Parser.ParsedData;
using GW2EIParser.Parser.ParsedData.CombatEvents;
//using static GW2EIParser.Builders.JsonModels.JsonStatistics;

namespace GW2EIParser.Models
{
    /// <summary>
    /// Passes statistical information about dps logs
    /// </summary>
    /*public static class DefenseStatistics
    {
        private static void FillFinalDefenses(JsonDefense finalDefenses, AbstractSingleActor actor, ParsedLog log, long start, long end, AbstractSingleActor target)
        {

            List<AbstractDamageEvent> damageLogs = actor.GetDamageTakenLogs(target, log, start, end);
            foreach (AbstractDamageEvent de in damageLogs)
            {
                finalDefenses.TakenDamage += de.Damage;
                finalDefenses.TakenCount++;
                if (de.ShieldDamage > 0)
                {
                    finalDefenses.BarrierDamage += de.ShieldDamage;
                    finalDefenses.BarrierCount++;
                }
                if (de.IsBlocked)
                {
                    finalDefenses.BlockedCount++;
                }
                if (de.IsEvaded)
                {
                    finalDefenses.EvadedCount++;
                }
                if (de.HasInterrupted)
                {
                    finalDefenses.InterruptedCount++;
                }
                if (de.IsAbsorbed)
                {
                    finalDefenses.InvulnedCount++;
                    finalDefenses.InvulDamage += de.Damage;
                }
            }
        }

        public static List<JsonDefenseAll> GetFinalDefenses(AbstractSingleActor actor, ParsedLog log)
        {
            var dead = new List<(long start, long end)>();
            var down = new List<(long start, long end)>();
            var dc = new List<(long start, long end)>();
            actor.AgentItem.GetAgentStatus(dead, down, dc, log);
            var res = new List<JsonDefenseAll>();
            foreach (PhaseData phase in log.FightData.GetPhases(log))
            {
                var final = new JsonDefenseAll();
                res.Add(final);
                long start = phase.Start;
                long end = phase.End;
                FillFinalDefenses(final, actor, log, start, end, null);
                //	Commons	
                final.DodgeCount = actor.GetCastLogs(log, start, end).Count(x => x.SkillId == SkillItem.DodgeId);
                final.DownCount = log.MechanicData.GetMechanicLogs(log, SkillItem.DownId).Count(x => x.Actor == actor && x.Time >= start && x.Time <= end);
                final.DeadCount = log.MechanicData.GetMechanicLogs(log, SkillItem.DeathId).Count(x => x.Actor == actor && x.Time >= start && x.Time <= end);
                final.DcCount = log.MechanicData.GetMechanicLogs(log, SkillItem.DCId).Count(x => x.Actor == actor && x.Time >= start && x.Time <= end);

                final.DownDuration = final.DownCount > 0 ? (int)down.Where(x => x.end >= start && x.start <= end).Sum(x => Math.Min(end, x.end) - Math.Max(x.start, start)) : 0;
                final.DeadDuration = final.DeadCount > 0 ? (int)dead.Where(x => x.end >= start && x.start <= end).Sum(x => Math.Min(end, x.end) - Math.Max(x.start, start)) : 0;
                final.DcDuration = final.DcCount > 0 ? (int)dc.Where(x => x.end >= start && x.start <= end).Sum(x => Math.Min(end, x.end) - Math.Max(x.start, start)) : 0;
            }
            return res;
        }

        public static List<JsonDefense> GetFinalDefenses(AbstractSingleActor actor, ParsedLog log, AbstractSingleActor target)
        {
            var res = new List<JsonDefense>();
            foreach (PhaseData phase in log.FightData.GetPhases(log))
            {
                var final = new JsonDefense();
                res.Add(final);
                long start = phase.Start;
                long end = phase.End;
                FillFinalDefenses(final, actor, log, start, end, target);
            }
            return res;
        }
    }*/
}
