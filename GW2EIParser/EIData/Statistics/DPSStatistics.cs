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
            List<FinalDPS> res = new List<FinalDPS>();
            foreach (PhaseData phase in log.FightData.GetPhases(log))
            {
                double phaseDuration = (phase.DurationInMS) / 1000.0;
                int damage;
                double dps = 0.0;
                FinalDPS final = new FinalDPS();
                List<AbstractDamageEvent> damageLogs = actor.GetDamageLogs(target, log, phase.Start, phase.End);
                List<AbstractDamageEvent> damageLogsActor = actor.GetJustActorDamageLogs(target, log, phase.Start, phase.End);
                //DPS
                damage = damageLogs.Sum(x => x.Damage);

                if (phaseDuration > 0)
                {
                    dps = damage / phaseDuration;
                }
                final.Dps = (int)Math.Round(dps);
                final.Damage = damage;
                //Condi DPS
                damage = damageLogs.Sum(x => x.IsCondi(log) ? x.Damage : 0);

                if (phaseDuration > 0)
                {
                    dps = damage / phaseDuration;
                }
                final.CondiDps = (int)Math.Round(dps);
                final.CondiDamage = damage;
                //Power DPS
                damage = final.Damage - final.CondiDamage;
                if (phaseDuration > 0)
                {
                    dps = damage / phaseDuration;
                }
                final.PowerDps = (int)Math.Round(dps);
                final.PowerDamage = damage;
                // Actor DPS
                damage = damageLogsActor.Sum(x => x.Damage);

                if (phaseDuration > 0)
                {
                    dps = damage / phaseDuration;
                }
                final.ActorDps = (int)Math.Round(dps);
                final.ActorDamage = damage;
                //Actor Condi DPS
                damage = damageLogsActor.Sum(x => x.IsCondi(log) ? x.Damage : 0);

                if (phaseDuration > 0)
                {
                    dps = damage / phaseDuration;
                }
                final.ActorCondiDps = (int)Math.Round(dps);
                final.ActorCondiDamage = damage;
                //Actor Power DPS
                damage = final.ActorDamage - final.ActorCondiDamage;
                if (phaseDuration > 0)
                {
                    dps = damage / phaseDuration;
                }
                final.ActorPowerDps = (int)Math.Round(dps);
                final.ActorPowerDamage = damage;
                res.Add(final);
            }
            return res;
        }

    }
}
