using System;
using System.Collections.Generic;
using GW2EIParser.Parser.ParsedData;
using GW2EIParser.Parser.ParsedData.CombatEvents;
using Newtonsoft.Json;
using static GW2EIParser.Builders.JsonModels.JsonLog;

namespace GW2EIParser.Builders.JsonModels
{
    /// <summary>
    /// Class corresponding to a skill
    /// </summary>
    public class JsonRotation
    {
        private class JsonRotationItemConverter : JsonConverter
        {
            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                var jsonRotation = (JsonRotationItem)value;
                //writer.WriteStartArray();
                writer.WriteValue(jsonRotation.CastTime);
                writer.WriteValue(jsonRotation.Duration);
                writer.WriteValue(jsonRotation.TimeGained);
                writer.WriteValue(jsonRotation.Quickness);
                //writer.WriteEndArray();
            }

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
                JsonSerializer serializer)
            {
                throw new NotSupportedException();
            }

            public override bool CanRead => false;

            public override bool CanConvert(Type objectType)
            {
                return objectType == typeof(JsonRotationItem);
            }
        }

        /// <summary>
        /// Class corresponding to a skill
        /// </summary>
        [JsonConverter(typeof(JsonRotationItemConverter))]
        public class JsonRotationItem
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

            public JsonRotationItem(AbstractCastEvent cl)
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
        /// <seealso cref="JsonLog.Descriptions"/>
        public string Id { get; set; }
        /// <summary>
        /// List of casted skills
        /// </summary>
        /// <seealso cref="JsonRotationItem"/>
        public List<JsonRotationItem> Skills { get; set; }

        public static List<JsonRotation> BuildRotation(List<AbstractCastEvent> cls, Dictionary<string, Desc> description)
        {
            var dict = new Dictionary<long, List<JsonRotationItem>>();
            foreach (AbstractCastEvent cl in cls)
            {
                SkillItem skill = cl.Skill;
                string skillName = skill.Name;
                if (!description.ContainsKey("s" + cl.SkillId))
                {
                    description["s" + cl.SkillId] = new SkillDesc(skill);
                }
                var jSkill = new JsonRotationItem(cl);
                if (dict.TryGetValue(cl.SkillId, out List<JsonRotationItem> list))
                {
                    list.Add(jSkill);
                }
                else
                {
                    dict[cl.SkillId] = new List<JsonRotationItem>()
                    {
                        jSkill
                    };
                }
            }
            var res = new List<JsonRotation>();
            foreach (KeyValuePair<long, List<JsonRotationItem>> pair in dict)
            {
                res.Add(new JsonRotation()
                {
                    Id = "s" + pair.Key,
                    Skills = pair.Value
                });
            }
            return res;
        }
    }
}

