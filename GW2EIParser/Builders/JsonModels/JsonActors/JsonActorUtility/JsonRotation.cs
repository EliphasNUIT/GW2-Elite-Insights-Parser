using GW2EIParser.Parser.ParsedData;
using GW2EIParser.Parser.ParsedData.CombatEvents;
using System.Collections.Generic;
using static GW2EIParser.Builders.JsonModels.JsonLog;

namespace GW2EIParser.Builders.JsonModels
{
    /// <summary>
    /// Class corresponding to a skill
    /// </summary>
    public class JsonRotation
    {
        /// <summary>
        /// Class corresponding to a skill
        /// </summary>
        public class JsonSkill
        {
            /// <summary>
            /// Time at which the skill was cast
            /// </summary>
            public int CastTime { get; set; }
            /// <summary>
            /// Duration of the animation
            /// </summary>
            public int Duration { get; set; }
            /// <summary>
            /// Gained time from the animation, could be negative, which means time was lost
            /// </summary>
            public int TimeGained { get; set; }
            /// <summary>
            /// Animation started while under quickness
            /// </summary>
            public bool Quickness { get; set; }

            public JsonSkill(AbstractCastEvent cl)
            {
                int timeGained = 0;
                if (cl.ReducedAnimation && cl.ActualDuration < cl.ExpectedDuration)
                {
                    timeGained = cl.ExpectedDuration - cl.ActualDuration;
                }
                else if (cl.Interrupted)
                {
                    timeGained = -cl.ActualDuration;
                }
                CastTime = (int)cl.Time;
                Duration = cl.ActualDuration;
                TimeGained = timeGained;
                Quickness = cl.UnderQuickness;
            }
        }

        /// <summary>
        /// ID of the skill
        /// </summary>
        /// <seealso cref="JsonLog.SkillMap"/>
        public long Id { get; set; }
        /// <summary>
        /// List of casted skills
        /// </summary>
        /// <seealso cref="JsonSkill"/>
        public List<JsonSkill> Skills { get; set; }

        public static List<JsonRotation> BuildRotation(List<AbstractCastEvent> cls, Dictionary<string, SkillDesc> skillMap)
        {
            Dictionary<long, List<JsonSkill>> dict = new Dictionary<long, List<JsonSkill>>();
            foreach (AbstractCastEvent cl in cls)
            {
                SkillItem skill = cl.Skill;
                string skillName = skill.Name;
                if (!skillMap.ContainsKey("s" + cl.SkillId))
                {
                    skillMap["s" + cl.SkillId] = new SkillDesc(skill);
                }
                JsonSkill jSkill = new JsonSkill(cl);
                if (dict.TryGetValue(cl.SkillId, out var list))
                {
                    list.Add(jSkill);
                }
                else
                {
                    dict[cl.SkillId] = new List<JsonSkill>()
                    {
                        jSkill
                    };
                }
            }
            List<JsonRotation> res = new List<JsonRotation>();
            foreach (var pair in dict)
            {
                res.Add(new JsonRotation()
                {
                    Id = pair.Key,
                    Skills = pair.Value
                });
            }
            return res;
        }
    }
}
