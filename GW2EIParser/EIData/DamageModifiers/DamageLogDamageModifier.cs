﻿using System.Collections.Generic;
using System.Linq;
using GW2EIParser.Parser;
using GW2EIParser.Parser.ParsedData.CombatEvents;
using static GW2EIParser.EIData.Player;

namespace GW2EIParser.EIData
{
    public class DamageLogDamageModifier : DamageModifier
    {

        public DamageLogDamageModifier(string name, string tooltip, DamageSource damageSource, double gainPerStack, DamageType srctype, DamageType compareType, ModifierSource src, string icon, DamageLogChecker checker, GainComputer gainComputer, ulong minBuild, ulong maxBuild) : base(name, tooltip, damageSource, gainPerStack, srctype, compareType, src, icon, gainComputer, checker, minBuild, maxBuild)
        {
        }

        public DamageLogDamageModifier(string name, string tooltip, DamageSource damageSource, double gainPerStack, DamageType srctype, DamageType compareType, ModifierSource src, string icon, DamageLogChecker checker, GainComputer gainComputer) : base(name, tooltip, damageSource, gainPerStack, srctype, compareType, src, icon, gainComputer, checker, ulong.MinValue, ulong.MaxValue)
        {
        }

        public override void ComputeDamageModifier(Dictionary<string, List<DamageModifierData>> data, Dictionary<NPC, Dictionary<string, List<DamageModifierData>>> dataTarget, Player p, ParsedLog log)
        {
            List<PhaseData> phases = log.FightData.GetPhases(log);
            double gain = GainComputer.ComputeGain(GainPerStack, 1);
            if (!p.GetDamageLogs(null, log, phases[0].Start, phases[0].End).Exists(x => DLChecker(x)))
            {
                return;
            }
            foreach (NPC target in log.FightData.Logic.NPCs)
            {
                if (!dataTarget.TryGetValue(target, out var extra))
                {
                    dataTarget[target] = new Dictionary<string, List<DamageModifierData>>();
                }
                Dictionary<string, List<DamageModifierData>> dict = dataTarget[target];
                if (!dict.TryGetValue(Name, out var list))
                {
                    var extraDataList = new List<DamageModifierData>();
                    for (int i = 0; i < phases.Count; i++)
                    {
                        int totalDamage = GetTotalDamage(p, log, target, i);
                        List<AbstractDamageEvent> typeHits = GetDamageLogs(p, log, target, phases[i]);
                        var effect = typeHits.Where(x => DLChecker(x)).ToList();
                        extraDataList.Add(new DamageModifierData(effect.Count, typeHits.Count, gain * effect.Sum(x => x.Damage), totalDamage));
                    }
                    dict[Name] = extraDataList;
                }
            }
            data[Name] = new List<DamageModifierData>();
            for (int i = 0; i < phases.Count; i++)
            {
                int totalDamage = GetTotalDamage(p, log, null, i);
                List<AbstractDamageEvent> typeHits = GetDamageLogs(p, log, null, phases[i]);
                var effect = typeHits.Where(x => DLChecker(x)).ToList();
                data[Name].Add(new DamageModifierData(effect.Count, typeHits.Count, gain * effect.Sum(x => x.Damage), totalDamage));
            }
        }
    }
}
