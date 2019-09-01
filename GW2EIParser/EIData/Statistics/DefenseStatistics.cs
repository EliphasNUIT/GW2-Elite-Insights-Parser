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
    public static class DefenseStatistics
    {

        public class FinalDefense
        {
            public long DamageTaken { get; set; }
            public int BlockedCount { get; set; }
            public int EvadedCount { get; set; }
            public int InvulnedCount { get; set; }
            public int DamageInvulned { get; set; }
            public int DamageBarrier { get; set; }
            public int InterruptedCount { get; set; }
        }

        public class FinalDefenseAll : FinalDefense
        {
            public int DodgeCount { get; set; }
            public int DownCount { get; set; }
            public int DownDuration { get; set; }
            public int DeadCount { get; set; }
            public int DeadDuration { get; set; }
            public int DcCount { get; set; }
            public int DcDuration { get; set; }
        }



        private static void FillFinalDefenses(FinalDefense finalDefenses, AbstractSingleActor actor, ParsedLog log, long start, long end, AbstractSingleActor target)
        {

            List<AbstractDamageEvent> damageLogs = actor.GetDamageTakenLogs(target, log, start, end);
            foreach (AbstractDamageEvent de in damageLogs)
            {
                finalDefenses.DamageTaken += de.Damage;
                finalDefenses.DamageBarrier += de.ShieldDamage;
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
                    finalDefenses.DamageInvulned += de.Damage;
                }
            }
        }

        public static List<FinalDefenseAll> GetFinalDefenses(AbstractSingleActor actor, ParsedLog log)
        {
            var dead = new List<(long start, long end)>();
            var down = new List<(long start, long end)>();
            var dc = new List<(long start, long end)>();
            actor.AgentItem.GetAgentStatus(dead, down, dc, log);
            var res = new List<FinalDefenseAll>();
            foreach (PhaseData phase in log.FightData.GetPhases(log))
            {
                var final = new FinalDefenseAll();
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

        public static List<FinalDefense> GetFinalDefenses(AbstractSingleActor actor, ParsedLog log, AbstractSingleActor target)
        {
            var res = new List<FinalDefense>();
            foreach (PhaseData phase in log.FightData.GetPhases(log))
            {
                var final = new FinalDefenseAll();
                res.Add(final);
                long start = phase.Start;
                long end = phase.End;
                FillFinalDefenses(final, actor, log, start, end, target);
            }
            return res;
        }
    }
}
