using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIParser.EIData;
using GW2EIParser.Parser;
using GW2EIParser.Parser.ParsedData.CombatEvents;
using Newtonsoft.Json;
using static GW2EIParser.Builders.JsonModels.JsonLog;

namespace GW2EIParser.Builders.JsonModels
{
    /// <summary>
    /// Class corresponding a damage distribution
    /// </summary>
    public class JsonDamageDistData
    {
        private class JsonDamageItemConverter : JsonConverter
        {
            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                var jsonDamageDistData = (JsonDamageItem)value;
                //writer.WriteStartArray();
                writer.WriteValue(jsonDamageDistData.Id);
                writer.WriteValue(jsonDamageDistData.AgentID);
                writer.WriteValue(jsonDamageDistData.Time);
                writer.WriteValue(jsonDamageDistData.Damage);
                writer.WriteValue(jsonDamageDistData.ShieldDamage);
                writer.WriteValue(jsonDamageDistData.EncodedBooleans);
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
                return objectType == typeof(JsonDamageItem);
            }
        }

        [JsonConverter(typeof(JsonDamageItemConverter))]
        public class JsonDamageItem
        {
            public string Id { get; set; }

            public string AgentID { get; set; }

            public long Time { get; set; }

            public int Damage { get; set; }
            public int ShieldDamage { get; set; }

            public long EncodedBooleans { get; set; }

            public JsonDamageItem(AbstractDamageEvent evt, ParsedLog log, Dictionary<string, Desc> description)
            {
                if (evt is NonDirectDamageEvent)
                {
                    Id = "b" + evt.SkillId;
                    if (!description.ContainsKey(Id))
                    {
                        if (!log.Buffs.BuffsByIds.TryGetValue(evt.SkillId, out Buff buff))
                        {
                            buff = new Buff(evt.Skill.Name, evt.SkillId, evt.Skill.Icon);
                        }
                        description[Id] = new BuffDesc(buff);
                    }
                }
                else
                {
                    Id = "s" + evt.SkillId;
                    if (!description.ContainsKey(Id))
                    {
                        description[Id] = new SkillDesc(evt.Skill);
                    }
                }
                Time = evt.Time;
                Damage = evt.Damage;
                ShieldDamage = evt.ShieldDamage;
                EncodedBooleans = 0;
                if (evt.IsCondi(log))
                {
                    EncodedBooleans |= AbstractDamageEvent.ECondi;
                }
                if (evt.IsOverNinety)
                {
                    EncodedBooleans |= AbstractDamageEvent.EIsOverNinety;
                }
                if (evt.AgainstUnderFifty)
                {
                    EncodedBooleans |= AbstractDamageEvent.EAgainstUnderFifty;
                }
                if (evt.IsMoving)
                {
                    EncodedBooleans |= AbstractDamageEvent.EIsMoving;
                }
                if (evt.IsFlanking)
                {
                    EncodedBooleans |= AbstractDamageEvent.EIsFlanking;
                }
                if (evt.HasHit)
                {
                    EncodedBooleans |= AbstractDamageEvent.EHasHit;
                }
                if (evt.HasCrit)
                {
                    EncodedBooleans |= AbstractDamageEvent.EHasCrit;
                }
                if (evt.HasGlanced)
                {
                    EncodedBooleans |= AbstractDamageEvent.EHasGlanced;
                }
                if (evt.IsBlind)
                {
                    EncodedBooleans |= AbstractDamageEvent.EIsBlind;
                }
                if (evt.IsAbsorbed)
                {
                    EncodedBooleans |= AbstractDamageEvent.EIsAbsorbed;
                }
                if (evt.HasInterrupted)
                {
                    EncodedBooleans |= AbstractDamageEvent.EHasInterrupted;
                }
                if (evt.HasDowned)
                {
                    EncodedBooleans |= AbstractDamageEvent.EHasDowned;
                }
                if (evt.HasKilled)
                {
                    EncodedBooleans |= AbstractDamageEvent.EHasKilled;
                }
                if (evt.IsBlocked)
                {
                    EncodedBooleans |= AbstractDamageEvent.EIsBlocked;
                }
                if (evt.IsEvaded)
                {
                    EncodedBooleans |= AbstractDamageEvent.EIsEvaded;
                }
            }
        }

        public class JsonDamageItemDone : JsonDamageItem
        {
            public JsonDamageItemDone(AbstractDamageEvent evt, ParsedLog log, Dictionary<string, Desc> description) : base(evt, log, description)
            {
                AgentID = GetActorID(evt.To, log, description);
            }
        }

        public class JsonDamageItemTaken : JsonDamageItem
        {
            public JsonDamageItemTaken(AbstractDamageEvent evt, ParsedLog log, Dictionary<string, Desc> description) : base(evt, log, description)
            {
                AgentID = GetActorID(evt.From, log, description);
            }
        }

        public List<JsonDamageItemDone> DamageEvents { get; set; }

        public List<JsonDamageItemTaken> DamageTakenEvents { get; set; }

        public JsonDamageDistData(ParsedLog log, AbstractActor actor, Dictionary<string, Desc> description)
        {
            /*List<PhaseData> phases = log.FightData.GetPhases(log);
            TotalDamageDists = new List<List<JsonDamageDist>>();
            TotalDamageTakenDists = new List<List<JsonDamageDist>>();
            foreach (PhaseData phase in phases)
            {
                TotalDamageDists.Add(JsonDamageDist.BuildJsonDamageDists(actor.GetJustActorDamageLogs(null, log, phase.Start, phase.End), log, description));
                TotalDamageTakenDists.Add(JsonDamageDist.BuildJsonDamageDists(actor.GetDamageTakenLogs(null, log, phase.Start, phase.End), log, description));
            }*/
            //
            DamageEvents = actor.GetJustActorDamageLogs(null, log, 0, log.FightData.FightDuration).Select(x => new JsonDamageItemDone(x, log, description)).ToList();
            DamageTakenEvents = actor.GetDamageTakenLogs(null, log, 0, log.FightData.FightDuration).Select(x => new JsonDamageItemTaken(x, log, description)).ToList();
        }
    }
}
