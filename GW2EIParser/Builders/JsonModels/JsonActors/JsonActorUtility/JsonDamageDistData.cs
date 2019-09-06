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
        public class JsonDamageItem
        {
            private const long Condi = 1;
            private const long IsOverNinety = 1 << 1;
            private const long AgainstUnderFifty = 1 << 2;
            private const long IsMoving = 1 << 3;
            private const long IsFlanking = 1 << 4;
            private const long HasHit = 1 << 5;
            private const long HasCrit = 1 << 6;
            private const long HasGlanced = 1 << 7;
            private const long IsBlind = 1 << 8;
            private const long IsAbsorbed = 1 << 9;
            private const long HasInterrupted = 1 << 10;
            private const long HasDowned = 1 << 11;
            private const long HasKilled = 1 << 12;
            private const long IsBlocked = 1 << 13;
            private const long IsEvaded = 1 << 14;

            public string Id { get; set; }
            /// <summary>
            /// ID of the relevant agent, destination for damage done, source for damage taken
            /// </summary>
            public string AgentID { get; set; }

            public long Time { get; set; }

            public int Damage { get; set; }
            public int ShieldDamage { get; set; }

            public long EncodedBooleans { get; set; }

            public JsonDamageItem(AbstractDamageEvent evt, ParsedLog log, Dictionary<string, Desc> description, bool taken)
            {
                if (evt is NonDirectDamageEvent)
                {
                    Id = "b" + evt.SkillId;
                }
                else
                {
                    Id = "s" + evt.SkillId;
                }
                AgentID = GetNPCID(taken ? evt.From : evt.To, log, description);
                Time = evt.Time;
                Damage = evt.Damage;
                ShieldDamage = evt.ShieldDamage;
                EncodedBooleans = 0;
                if (evt.IsCondi(log))
                {
                    EncodedBooleans |= Condi;
                }
                if (evt.IsOverNinety)
                {
                    EncodedBooleans |= IsOverNinety;
                }
                if (evt.AgainstUnderFifty)
                {
                    EncodedBooleans |= AgainstUnderFifty;
                }
                if (evt.IsMoving)
                {
                    EncodedBooleans |= IsMoving;
                }
                if (evt.IsFlanking)
                {
                    EncodedBooleans |= IsFlanking;
                }
                if (evt.HasHit)
                {
                    EncodedBooleans |= HasHit;
                }
                if (evt.HasCrit)
                {
                    EncodedBooleans |= HasCrit;
                }
                if (evt.HasGlanced)
                {
                    EncodedBooleans |= HasGlanced;
                }
                if (evt.IsBlind)
                {
                    EncodedBooleans |= IsBlind;
                }
                if (evt.IsAbsorbed)
                {
                    EncodedBooleans |= IsAbsorbed;
                }
                if (evt.HasInterrupted)
                {
                    EncodedBooleans |= HasInterrupted;
                }
                if (evt.HasDowned)
                {
                    EncodedBooleans |= HasDowned;
                }
                if (evt.HasKilled)
                {
                    EncodedBooleans |= HasKilled;
                }
                if (evt.IsBlocked)
                {
                    EncodedBooleans |= IsBlocked;
                }
                if (evt.IsEvaded)
                {
                    EncodedBooleans |= IsEvaded;
                }
            }
        }


        /// <summary>
        /// Total Damage distribution array \n
        /// Length == # of phases
        /// </summary>
        /// <seealso cref="JsonDamageDist"/>
        public List<List<JsonDamageDist>> TotalDamageDists { get; set; }
        /// <summary>
        /// Damage taken array
        /// Length == # of phases
        /// </summary>
        /// <seealso cref="JsonDamageDist"/>
        public List<List<JsonDamageDist>> TotalDamageTakenDists { get; set; }

        public List<JsonDamageItem> DamageEvents { get; set; }

        public List<JsonDamageItem> DamageTakenEvents { get; set; }

        public JsonDamageDistData(ParsedLog log, AbstractActor actor, Dictionary<string, Desc> description)
        {
            List<PhaseData> phases = log.FightData.GetPhases(log);
            TotalDamageDists = new List<List<JsonDamageDist>>();
            TotalDamageTakenDists = new List<List<JsonDamageDist>>();
            foreach (PhaseData phase in phases)
            {
                TotalDamageDists.Add(JsonDamageDist.BuildJsonDamageDists(actor.GetJustActorDamageLogs(null, log, phase.Start, phase.End), log, description));
                TotalDamageTakenDists.Add(JsonDamageDist.BuildJsonDamageDists(actor.GetDamageTakenLogs(null, log, phase.Start, phase.End), log, description));
            }
            //
            DamageEvents = actor.GetJustActorDamageLogs(null, log, 0, log.FightData.FightDuration).Select(x => new JsonDamageItem(x, log, description, false)).ToList();
            DamageTakenEvents = actor.GetDamageTakenLogs(null, log, 0, log.FightData.FightDuration).Select(x => new JsonDamageItem(x, log, description, true)).ToList();
        }
    }
}
