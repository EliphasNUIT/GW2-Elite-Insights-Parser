using System.Collections.Generic;
using System.Linq;
using GW2EIParser.EIData;
using GW2EIParser.Parser;
using GW2EIParser.Parser.ParsedData;
using GW2EIParser.Parser.ParsedData.CombatEvents;
using static GW2EIParser.Builders.JsonModels.JsonLog;

namespace GW2EIParser.Builders.JsonModels
{
    /// <summary>
    /// Class corresponding a damage distribution
    /// </summary>
    public class JsonDamageDist
    {
        /// <summary>
        /// Total damage done
        /// </summary>
        public int TotalDamage { get; set; }
        /// <summary>
        /// Minimum damage done
        /// </summary>
        public int Min { get; set; }
        /// <summary>
        /// Maximum damage done
        /// </summary>
        public int Max { get; set; }
        /// <summary>
        /// Number of hits
        /// </summary>
        public int Hits { get; set; }
        /// <summary>
        /// Number of crits
        /// </summary>
        public int Crit { get; set; }
        /// <summary>
        /// Number of glances
        /// </summary>
        public int Glance { get; set; }
        /// <summary>
        /// Number of downed
        /// </summary>
        public int Downed { get; set; }
        /// <summary>
        /// Number of flanks
        /// </summary>
        public int Flank { get; set; }
        /// <summary>
        /// Damage done against barrier, not necessarily included in total damage
        /// </summary>
        public int ShieldDamage { get; set; }
        /// <summary>
        /// ID of the damaging skill
        /// </summary>
        /// <seealso cref="JsonLog.Descriptions"/>
        public string Id { get; set; }

        public JsonDamageDist(List<AbstractDamageEvent> list, SkillItem skill, ParsedLog log, Dictionary<string, Desc> description)
        {
            bool isIndirectDamage = log.Buffs.BuffsByIds.ContainsKey(skill.ID);
            Max = int.MinValue;
            Min = int.MaxValue;
            foreach (AbstractDamageEvent dl in list)
            {
                isIndirectDamage = isIndirectDamage || dl is NonDirectDamageEvent;
                int curdmg = dl.Damage;
                TotalDamage += curdmg;
                if (curdmg < Min) { Min = curdmg; }
                if (curdmg > Max) { Max = curdmg; }
                Hits++;
                if (dl.HasCrit)
                {
                    Crit++;
                }
                if (dl.HasDowned)
                {
                    Downed++;
                }
                if (dl.HasGlanced)
                {
                    Glance++;
                }
                if (dl.IsFlanking)
                {
                    Flank++;
                }
                ShieldDamage += dl.ShieldDamage;
            }
            Id = (isIndirectDamage ? "b" : "s") + skill.ID;
            if (isIndirectDamage)
            {
                if (!description.ContainsKey(Id))
                {
                    if (log.Buffs.BuffsByIds.TryGetValue(skill.ID, out Buff buff))
                    {
                        description[Id] = new BuffDesc(buff);
                    }
                    else
                    {
                        var auxBoon = new Buff(skill.Name, skill.ID, skill.Icon);
                        description[Id] = new BuffDesc(auxBoon);
                    }
                }
            }
            else
            {
                if (!description.ContainsKey(Id))
                {
                    description[Id] = new SkillDesc(skill);
                }
            }
        }


        public static List<JsonDamageDist> BuildJsonDamageDists(List<AbstractDamageEvent> damageEvents, ParsedLog log, Dictionary<string, Desc> description)
        {
            var damageDist = new List<JsonDamageDist>();
            var dict = damageEvents.GroupBy(x => x.Skill).ToDictionary(x => x.Key, x => x.ToList());
            foreach (KeyValuePair<SkillItem, List<AbstractDamageEvent>> pair in dict)
            {
                SkillItem skill = pair.Key;
                List<AbstractDamageEvent> damageLogs = pair.Value;
                if (damageLogs.Count == 0)
                {
                    continue;
                }
                damageDist.Add(new JsonDamageDist(damageLogs, skill, log, description));
            }
            return damageDist;
        }
    }
}
