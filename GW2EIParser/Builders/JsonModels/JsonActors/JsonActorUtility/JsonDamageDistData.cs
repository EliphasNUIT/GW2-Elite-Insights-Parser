using System.Collections.Generic;
using System.Linq;
using GW2EIParser.EIData;
using GW2EIParser.Parser;
using GW2EIParser.Parser.ParsedData.CombatEvents;
using static GW2EIParser.Builders.JsonModels.JsonLog;

namespace GW2EIParser.Builders.JsonModels
{
    /// <summary>
    /// Class corresponding a damage distribution
    /// </summary>
    public class JsonDamageDistData
    {
        public abstract class JsonDamageItem
        {
            public string Id { get; set; }

            public long Time { get; set; }

            public int Damage { get; set; }
            public int ShieldDamage { get; set; }

            public long EncodedBooleans { get; set; }

            public JsonDamageItem(AbstractDamageEvent evt, ParsedLog log)
            {
                if (evt is NonDirectDamageEvent)
                {
                    Id = "b" + evt.SkillId;
                }
                else
                {
                    Id = "s" + evt.SkillId;
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

            public string DestinationID { get; set; }
            public JsonDamageItemDone(AbstractDamageEvent evt, ParsedLog log, Dictionary<string, Desc> description) : base(evt, log)
            {
                DestinationID = GetActorID(evt.To, log, description);
            }
        }

        public class JsonDamageItemTaken : JsonDamageItem
        {

            public string SourceID { get; set; }

            public JsonDamageItemTaken(AbstractDamageEvent evt, ParsedLog log, Dictionary<string, Desc> description) : base(evt, log)
            {
                SourceID = GetActorID(evt.From, log, description);
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
