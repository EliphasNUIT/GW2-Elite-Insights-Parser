using System.Collections.Generic;
using System.Linq;
using GW2EIParser.EIData;
using GW2EIParser.Parser;
using GW2EIParser.Parser.ParsedData;
using static GW2EIParser.Builders.JsonModels.JsonLog;

namespace GW2EIParser.Builders.JsonModels
{
    public class JsonStatistics
    {
        /// <summary>
        /// Defensive stats 
        /// </summary>
        public class JsonDefense
        {
            /// <summary>
            /// Total damage taken
            /// </summary>
            public long TakenDamage { get; set; }
            /// <summary>
            /// Number of hits taken
            /// </summary>
            public long TakenCount { get; set; }
            /// <summary>
            /// Number of blocks
            /// </summary>
            public int BlockedCount { get; set; }
            /// <summary>
            /// Number of evades
            /// </summary>
            public int EvadedCount { get; set; }
            /// <summary>
            /// Number of time an incoming attack was negated by invul
            /// </summary>
            public int InvulnedCount { get; set; }
            /// <summary>
            /// Damage negated by invul
            /// </summary>
            public int InvulDamage { get; set; }
            /// <summary>
            /// Damage done against barrier
            /// </summary>
            public int BarrierDamage { get; set; }
            /// <summary>
            /// Number of hits done against barrier
            /// </summary>
            public int BarrierCount { get; set; }
            /// <summary>
            /// Number of time interrupted
            /// </summary>
            public int InterruptedCount { get; set; }
        }

        /// <summary>
        /// Defensive stats
        /// </summary>
        public class JsonDefenseAll : JsonDefense
        {
            /// <summary>
            /// Number of dodges
            /// </summary>
            public int DodgeCount { get; set; }
            /// <summary>
            /// Number of time downed
            /// </summary>
            public int DownCount { get; set; }
            /// <summary>
            /// Time passed in downstate
            /// </summary>
            public int DownDuration { get; set; }
            /// <summary>
            /// Number of time died
            /// </summary>
            public int DeadCount { get; set; }
            /// <summary>
            /// Time passed in dead state
            /// </summary>
            public int DeadDuration { get; set; }
            /// <summary>
            /// Number of time disconnected
            /// </summary>
            public int DcCount { get; set; }
            /// <summary>
            /// Time passed in disconnected state
            /// </summary>
            public int DcDuration { get; set; }
        }
        /// <summary>
        /// DPS stats
        /// </summary>
        public class JsonDPS
        {
            /// <summary>
            /// Total dps
            /// </summary>
            public int Dps { get; set; }
            /// <summary>
            /// Total damage
            /// </summary>
            public int Damage { get; set; }
            /// <summary>
            /// Total condi dps
            /// </summary>
            public int CondiDps { get; set; }
            /// <summary>
            /// Total condi damage
            /// </summary>
            public int CondiDamage { get; set; }
            /// <summary>
            /// Total power dps
            /// </summary>
            public int PowerDps { get; set; }
            /// <summary>
            /// Total power damage
            /// </summary>
            public int PowerDamage { get; set; }
            /// <summary>
            /// Total actor only dps, missing if same as Dps
            /// </summary>
            public int ActorDps { get; set; }
            /// <summary>
            /// Total actor only damage, missing if same as Damage
            /// </summary>
            public int ActorDamage { get; set; }
            /// <summary>
            /// Total actor only condi dps, missing if same as CondiDps
            /// </summary>
            public int ActorCondiDps { get; set; }
            /// <summary>
            /// Total actor only condi damage, missing if same as CondiDamage
            /// </summary>
            public int ActorCondiDamage { get; set; }
            /// <summary>
            /// Total actor only power dps, missing if same as PowerDps
            /// </summary>
            public int ActorPowerDps { get; set; }
            /// <summary>
            /// Total actor only power damage, missing if same as PowerDamage
            /// </summary>
            public int ActorPowerDamage { get; set; }

        }

        /// <summary>
        /// Gameplay stats
        /// </summary>
        public class JsonGameplay
        {
            /// <summary>
            /// Number of damage hit
            /// </summary>
            public int DamageCount { get; set; }
            /// <summary>
            /// Number of direct damage hit
            /// </summary>
            public int DirectDamageCount { get; set; }
            /// <summary>
            /// Number of critable hit
            /// </summary>
            public int CritableDirectDamageCount { get; set; }
            /// <summary>
            /// Number of crit
            /// </summary>
            public int CriticalCount { get; set; }
            /// <summary>
            /// Total critical damage
            /// </summary>
            public int CriticalDamage { get; set; }
            /// <summary>
            /// Number of hits while flanking
            /// </summary>
            public int FlankingCount { get; set; }
            /// <summary>
            /// Number of glanced hits
            /// </summary>
            public int GlanceCount { get; set; }
            /// <summary>
            /// Number of missed hits
            /// </summary>
            public int Missed { get; set; }
            /// <summary>
            /// Number of hits that interrupted a skill
            /// </summary>
            public int Interrupts { get; set; }
            /// <summary>
            /// Number of hits against invulnerable targets
            /// </summary>
            public int Invulned { get; set; }
            /// <summary>
            /// Damage done against barrier
            /// </summary>
            public int BarrierDamage { get; set; }
            /// <summary>
            /// Number of hits against barrier
            /// </summary>
            public int BarrierCount { get; set; }
        }

        public class JsonGameplayAll : JsonGameplay
        {
            /// <summary>
            /// Number of time you interrupted your cast
            /// </summary>
            public int Wasted { get; set; }
            /// <summary>
            /// Time wasted by interrupting your cast
            /// </summary>
            public double TimeWasted { get; set; }
            /// <summary>
            /// Number of time you skipped an aftercast
            /// </summary>
            public int Saved { get; set; }
            /// <summary>
            /// Time saved while skipping aftercast
            /// </summary>
            public double TimeSaved { get; set; }
            /// <summary>
            /// Average amount of boons
            /// </summary>
            public double AvgBoons { get; set; }
            /// <summary>
            /// Average amount of boons over active time
            /// </summary>
            public double AvgActiveBoons { get; set; }
            /// <summary>
            /// Average amount of conditions
            /// </summary>
            public double AvgConditions { get; set; }
            /// <summary>
            /// Average amount of conditions over active time
            /// </summary>
            public double AvgActiveConditions { get; set; }
            /// <summary>
            /// Number of time a weapon swap happened
            /// </summary>
            public int SwapCount { get; set; }
        }

        /// <summary>
        /// Support stats
        /// </summary>
        public class JsonSupport
        {
            /// <summary>
            /// Number of time a condition was removed
            /// </summary>
            public long CondiCleanse { get; set; }
            /// <summary>
            /// Total time of condition removed
            /// </summary>
            public double CondiCleanseTime { get; set; }
            /// <summary>
            /// Number of time a boon was removed
            /// </summary>
            public long BoonStrips { get; set; }
            /// <summary>
            /// Total time of boons removed
            /// </summary>
            public double BoonStripsTime { get; set; }
        }

        /// <summary>
        /// Support stats
        /// </summary>
        public class JsonSupportAll : JsonSupport
        {
            /// <summary>
            /// Number of time ressurected someone
            /// </summary>
            public long Resurrects { get; set; }
            /// <summary>
            /// Time passed on ressurecting
            /// </summary>
            public double ResurrectTime { get; set; }
        }



        // Buffs
        public class JsonBuffs
        {
            public double Uptime { get; set; }
            public Dictionary<string, double> Generation { get; } = new Dictionary<string, double>();
            public Dictionary<string, double> Overstack { get; } = new Dictionary<string, double>();
            public Dictionary<string, double> Wasted { get; } = new Dictionary<string, double>();
            public Dictionary<string, double> ByExtension { get; } = new Dictionary<string, double>();
            public Dictionary<string, double> Extended { get; } = new Dictionary<string, double>();
            public Dictionary<string, double> UnknownExtended { get; } = new Dictionary<string, double>();
            public double Presence { get; set; }

            public JsonBuffs(Buff buff, ParsedLog log, Dictionary<AgentItem, BuffDistributionItem> dict, Dictionary<long, long> presence, Dictionary<string, Desc> description)
            {
                double multiplier = 100.0;
                if (buff.Type == Buff.BuffType.Intensity)
                {
                    multiplier = 1.0;
                    if (presence.TryGetValue(buff.ID, out long pres) && pres > 0)
                    {
                        Presence = 100.0 * pres;
                    }
                }
                Uptime = multiplier * dict.Sum(x => x.Value.Generation);
                foreach (AgentItem ag in dict.Keys)
                {
                    // Players are always in description
                    // Find actor will return null if agent is not present in the known agent pool
                    if (ag.Type != AgentItem.AgentType.Player && log.FindActor(ag, false, false) == null)
                    {
                        // special case for WvW
                        string descID = ag.Type == AgentItem.AgentType.EnemyPlayer ? ag.UniqueID : "npc" + ag.ID;
                        if (!description.ContainsKey(descID))
                        {
                            // create a dummy npc for the description
                            var dummyNPC = new NPC(ag, false);
                            description[descID] = new NPCDesc(dummyNPC, log);
                        }
                    }
                    string uniqueID = ag.UniqueID;
                    BuffDistributionItem item = dict[ag];
                    if (item.Generation > 0)
                    {
                        Generation[uniqueID] = multiplier * item.Generation;
                    }
                    if (item.Overstack > 0)
                    {
                        Overstack[uniqueID] = multiplier * item.Overstack;
                    }
                    if (item.Wasted > 0)
                    {
                        Wasted[uniqueID] = multiplier * item.Wasted;
                    }
                    if (item.ByExtension > 0)
                    {
                        ByExtension[uniqueID] = multiplier * item.ByExtension;
                    }
                    if (item.Extended > 0)
                    {
                        Extended[uniqueID] = multiplier * item.Extended;
                    }
                    if (item.UnknownExtended > 0)
                    {
                        UnknownExtended[uniqueID] = multiplier * item.UnknownExtended;
                    }
                }
            }
        }
        /// <summary>
        /// Represents a stack status item for buffs
        /// </summary>
        public class JsonBuffStackStatus
        {
            public class JsonBuffStackStatusItem
            {
                /// <summary>
                /// Unique id of the source
                /// </summary>
                public string SourceId { get; set; }
                /// <summary>
                /// Duration of the item, if missing that means duration == <seealso cref="JsonBuffStackStatus.Duration"/>
                /// </summary>
                public long Duration { get; set; }
            }
            /// <summary>
            /// Start time of the stack
            /// </summary>
            public long Start { get; set; }
            /// <summary>
            /// Duration of the stack
            /// </summary>
            public long Duration { get; set; }
            /// <summary>
            /// Sources of the stack
            /// </summary>
            public List<JsonBuffStackStatusItem> Sources { get; set; } 
        }

        public static (List<Dictionary<string, JsonBuffs>>, Dictionary<string, List<int>>, Dictionary<string, List<JsonBuffStackStatus>>) GetJsonBuffs(AbstractSingleActor actor, ParsedLog log, Dictionary<string, Desc> description)
        {
            var buffs = new List<Dictionary<string, JsonBuffs>>();
            var buffStates = new Dictionary<string, List<int>>();
            var buffStackStates = new Dictionary<string, List<JsonBuffStackStatus>>();
            Dictionary<long, BuffsGraphModel> buffGraphs = actor.GetBuffGraphs(log);
            for (int i = 0; i < log.FightData.GetPhases(log).Count; i++)
            {
                BuffDistributionDictionary buffDistribution = actor.GetBuffDistribution(log, i);
                Dictionary<long, long> buffPresence = actor.GetBuffPresence(log, i);
                var buffDict = new Dictionary<string, JsonBuffs>();

                foreach (long buffID in buffDistribution.Keys)
                {
                    Buff buff = log.Buffs.BuffsByIds[buffID];
                    Dictionary<AgentItem, BuffDistributionItem> dict = buffDistribution[buffID];

                    string id = "b" + buffID;
                    if (!description.ContainsKey(id))
                    {
                        description[id] = new BuffDesc(buff);
                    }
                    if (!buffStates.ContainsKey(id) && buffGraphs.TryGetValue(buffID, out BuffsGraphModel bgm))
                    {
                        buffStates[id] = bgm.GetStatesList();
                        buffStackStates[id] = bgm.GetStackStatusList();
                    }
                    var jsonBuff = new JsonBuffs(buff, log, dict, buffPresence, description);
                    buffDict.Add(id, jsonBuff);
                }

                buffs.Add(buffDict);
            }

            return (buffs, buffStates, buffStackStates);
        }


        /// <summary>
        /// Stats against all  \n
        /// Length == # of phases
        /// </summary>
        /// <seealso cref="JsonGameplayAll"/>
        public List<JsonGameplayAll> GameplayAll { get; set; }


        /// <summary>
        /// Defensive stats \n
        /// Length == # of phases
        /// </summary>
        /// <seealso cref="JsonDefenseAll"/>
        public List<JsonDefenseAll> DefenseAll { get; set; }

        /// <summary>
        /// Support stats \n
        /// Length == # of phases
        /// </summary>
        /// <seealso cref="JsonSupport"/>
        public List<JsonSupportAll> SupportAll { get; set; }

        /// <summary>
        /// Array of Total DPS stats \n
        /// Length == # of phases
        /// </summary>
        /// <seealso cref="JsonDPS"/>
        public List<JsonDPS> DpsAll { get; set; }

        public List<Dictionary<string, JsonBuffs>> Buffs { get; set; }
        public Dictionary<string, List<int>> BuffStates { get; set; }
        public Dictionary<string, List<JsonBuffStackStatus>> BuffStackStates { get; set; }

        public JsonStatistics(ParsedLog log, AbstractSingleActor actor, Dictionary<string, Desc> description)
        {
            DpsAll = actor.GetDPS(log);
            GameplayAll = actor.GetStats(log);
            DefenseAll = actor.GetDefenses(log);
            SupportAll = actor.GetSupport(log);
            (Buffs,BuffStates, BuffStackStates) = GetJsonBuffs(actor, log, description);
        }

    }
}
