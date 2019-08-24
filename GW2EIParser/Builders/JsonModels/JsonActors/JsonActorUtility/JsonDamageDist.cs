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
        /// <seealso cref="JsonLog.SkillMap"/>
        /// <seealso cref="JsonLog.BuffMap"/>
        public long Id { get; set; }
        /// <summary>
        /// True if indirect damage
        /// </summary>
        public bool IndirectDamage { get; set; }

        public JsonDamageDist(List<AbstractDamageEvent> list, bool indirectDamage, long id)
        {
            IndirectDamage = indirectDamage;
            Hits = list.Count;
            TotalDamage = list.Sum(x => x.Damage);
            Min = list.Min(x => x.Damage);
            Max = list.Max(x => x.Damage);
            Flank = IndirectDamage ? 0 : list.Count(x => x.IsFlanking);
            Crit = IndirectDamage ? 0 : list.Count(x => x.HasCrit);
            Glance = IndirectDamage ? 0 : list.Count(x => x.HasGlanced);
            ShieldDamage = list.Sum(x => x.ShieldDamage);
            Id = id;
        }


        public static List<JsonDamageDist> BuildJsonDamageDists(List<AbstractDamageEvent> damageEvents, ParsedLog log, Dictionary<string, SkillDesc> skillMap, Dictionary<string, BuffDesc> buffMap)
        {
            List<JsonDamageDist> damageDist = new List<JsonDamageDist>();
            Dictionary<SkillItem, List<AbstractDamageEvent>> dict = damageEvents.GroupBy(x => x.Skill).ToDictionary(x => x.Key, x => x.ToList());
            foreach (KeyValuePair<SkillItem, List<AbstractDamageEvent>> pair in dict)
            {
                SkillItem skill = pair.Key;
                long id = pair.Key.ID;
                List<AbstractDamageEvent> damageLogs = pair.Value;
                if (damageLogs.Count == 0)
                {
                    continue;
                }
                bool indirect = damageLogs.Exists(x => x is NonDirectDamageEvent);
                if (indirect)
                {
                    if (!buffMap.ContainsKey("b" + id))
                    {
                        if (log.Buffs.BuffsByIds.TryGetValue(id, out Buff buff))
                        {
                            buffMap["b" + id] = new BuffDesc(buff);
                        }
                        else
                        {
                            Buff auxBoon = new Buff(skill.Name, id, skill.Icon);
                            buffMap["b" + id] = new BuffDesc(auxBoon);
                        }
                    }
                }
                else
                {
                    if (!skillMap.ContainsKey("s" + id))
                    {
                        skillMap["s" + id] = new SkillDesc(skill);
                    }
                }
                string prefix = indirect ? "b" : "s";
                damageDist.Add(new JsonDamageDist(damageLogs, indirect, id));
            }
            return damageDist;
        }
    }
}
