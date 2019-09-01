using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIParser.EIData;
using GW2EIParser.Parser;
using GW2EIParser.Parser.ParsedData.CombatEvents;

namespace GW2EIParser.Models
{
    /// <summary>
    /// Passes statistical information about dps logs
    /// </summary>
    public static class DPSStatistics
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


        public static List<FinalDPS> GetFinalDPS(AbstractSingleActor actor, ParsedLog log, AbstractSingleActor target)
        {
            var res = new List<FinalDPS>();
            foreach (PhaseData phase in log.FightData.GetPhases(log))
            {
                double phaseDuration = (phase.DurationInMS) / 1000.0;
                var final = new FinalDPS();
                //DPS
                List<AbstractDamageEvent> damageLogs = actor.GetDamageLogs(target, log, phase.Start, phase.End);
                foreach(AbstractDamageEvent evt in damageLogs)
                {
                    final.Damage += evt.Damage;
                    final.CondiDamage += evt.IsCondi(log) ? evt.Damage : 0;
                    if (evt.From == actor.AgentItem)
                    {
                        final.ActorDamage += evt.Damage;
                        final.CondiDamage += evt.IsCondi(log) ? evt.Damage : 0;
                    }
                }
                //
                final.Dps = (int)Math.Round(final.Damage / phaseDuration);
                final.CondiDps = (int)Math.Round(final.CondiDamage / phaseDuration);
                final.PowerDamage = final.Damage - final.CondiDamage;
                final.PowerDps = (int)Math.Round(final.PowerDamage / phaseDuration);
                //
                final.ActorDps = (int)Math.Round(final.ActorDamage / phaseDuration);
                final.ActorCondiDps = (int)Math.Round(final.ActorCondiDamage / phaseDuration);
                final.ActorPowerDamage = final.ActorDamage - final.ActorCondiDamage;
                final.ActorPowerDps = (int)Math.Round(final.ActorPowerDamage / phaseDuration);
                res.Add(final);
            }
            return res;
        }

    }
}
