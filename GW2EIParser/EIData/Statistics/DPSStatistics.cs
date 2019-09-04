using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIParser.EIData;
using GW2EIParser.Parser;
using GW2EIParser.Parser.ParsedData.CombatEvents;
using static GW2EIParser.Builders.JsonModels.JsonStatistics;

namespace GW2EIParser.Models
{
    /// <summary>
    /// Passes statistical information about dps logs
    /// </summary>
    public static class DPSStatistics
    {
        public static List<JsonDPS> GetFinalDPS(AbstractSingleActor actor, ParsedLog log, AbstractSingleActor target)
        {
            var res = new List<JsonDPS>();
            foreach (PhaseData phase in log.FightData.GetPhases(log))
            {
                double phaseDuration = (phase.DurationInMS) / 1000.0;
                var final = new JsonDPS();
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
