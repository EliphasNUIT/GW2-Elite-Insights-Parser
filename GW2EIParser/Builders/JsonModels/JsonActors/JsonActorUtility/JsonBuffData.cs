using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using GW2EIParser.EIData;
using GW2EIParser.Parser;
using GW2EIParser.Parser.ParsedData;
using GW2EIParser.Parser.ParsedData.CombatEvents;
using static GW2EIParser.Builders.JsonModels.JsonLog;

namespace GW2EIParser.Builders.JsonModels
{
    public class JsonBuffData
    {

        // Buffs
        /*public class JsonBuffs
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
                    string uniqueID = GetActorID(ag, log, description);
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
        }*/
        /// <summary>
        /// Represents a stack status item for buffs
        /// </summary>
        public class JsonBuffStackStatus
        {
            public class JsonBuffStackStatusData
            {
                [DefaultValue(null)]
                public long Start { get; set; }
                public long Duration { get; set; }
            }

            public class JsonBuffStackStatusSources
            {
                public string Src { get; set; }
                public long Duration { get; set; }
                public string SeedSrc { get; set; }
            }

            List<JsonBuffStackStatusData> StackData { get; set; }
            List<List<JsonBuffStackStatusSources>> StackStatus { get; set; }
            public JsonBuffStackStatus(List<BuffSimulationItem> sourceBasedBoonChart, ParsedLog log, Dictionary<string, Desc> description)
            {
                StackData = new List<JsonBuffStackStatusData>();
                StackStatus = new List<List<JsonBuffStackStatusSources>>();
                foreach (BuffSimulationItem item in sourceBasedBoonChart)
                {
                    StackData.Add(new JsonBuffStackStatusData { Start = item.Start, Duration = item.Duration });
                    StackStatus.Add(item.GetStackStatusList(log, description));
                }
            }
        }

        public class JsonBuffRemoveItem
        {
            public string UniqueID { get; set; }
            [DefaultValue(null)]
            public long Time { get; set; }
            public long RemovedDuration { get; set; }
            public int RemovedStacks { get; set; }

            public JsonBuffRemoveItem(BuffRemoveAllEvent brae, ParsedLog log, Dictionary<string, Desc> description)
            {
                UniqueID = GetActorID(brae.To, log, description);
                Time = brae.Time;
                RemovedDuration = brae.RemovedDuration;
                RemovedStacks = brae.RemovedStacks;
            }
        }

        public class JsonBuffWasteItem
        {
            public string UniqueID { get; set; }
            [DefaultValue(null)]
            public long Time { get; set; }
            public long WastedDuration { get; set; }

            public JsonBuffWasteItem(BuffSimulationItemWasted item, ParsedLog log, Dictionary<string, Desc> description)
            {
                UniqueID = GetActorID(item.Src, log, description);
                Time = item.Time;
                WastedDuration = item.Waste;
            }
        }

        public static (/*List<Dictionary<string, JsonBuffs>>,*/ Dictionary<string, List<int>>, Dictionary<string, JsonBuffStackStatus>, Dictionary<string, List<JsonBuffWasteItem>>, Dictionary<string, List<JsonBuffWasteItem>>) GetJsonBuffs(AbstractSingleActor actor, ParsedLog log, Dictionary<string, Desc> description)
        {
            //var buffs = new List<Dictionary<string, JsonBuffs>>();
            var buffStates = new Dictionary<string, List<int>>();
            var buffStackStates = new Dictionary<string, JsonBuffStackStatus>();
            var buffWastedStates = new Dictionary<string, List<JsonBuffWasteItem>>();
            var buffOverstackStates = new Dictionary<string, List<JsonBuffWasteItem>>();
            Dictionary<long, BuffsGraphModel> buffGraphs = actor.GetBuffGraphs(log);
            /*for (int i = 0; i < log.FightData.GetPhases(log).Count; i++)
            {
                BuffDistributionDictionary buffDistribution = actor.GetBuffDistribution(log, i);
                Dictionary<long, long> buffPresence = actor.GetBuffPresence(log, i);
                var buffDict = new Dictionary<string, JsonBuffs>();
*/
            foreach (long buffID in buffGraphs.Keys)
            {
                Buff buff = log.Buffs.BuffsByIds[buffID];
                //Dictionary<AgentItem, BuffDistributionItem> dict = buffDistribution[buffID];

                string id = "b" + buffID;
                if (!description.ContainsKey(id))
                {
                    description[id] = new BuffDesc(buff);
                }
                if (!buffStates.ContainsKey(id) && buffGraphs.TryGetValue(buffID, out BuffsGraphModel bgm))
                {
                    buffStates[id] = bgm.GetStatesList();
                    if (bgm.IsSourceBased)
                    {
                        buffStackStates[id] = bgm.GetStackStatusList(log, description);
                        (buffOverstackStates[id], buffWastedStates[id]) = bgm.GetWasteStatusList(log, description);
                    }
                }
                /*var jsonBuff = new JsonBuffs(buff, log, dict, buffPresence, description);
                buffDict.Add(id, jsonBuff);
            }

            buffs.Add(buffDict);*/
            }

            return (/*buffs, */buffStates, buffStackStates, buffOverstackStates, buffWastedStates);
        }

        //public List<Dictionary<string, JsonBuffs>> Buffs { get; set; }
        /// <summary>
        /// Dictionary per buff that contains an array of int that represents the number of buff status \n
        /// Value[2*i] will be the time, value[2*i+1] will be the number of buff present from value[2*i] to value[2*(i+1)] \n
        /// If i corresponds to the last element that means the status did not change for the remainder of the fight \n
        /// </summary>
        public Dictionary<string, List<int>> BuffStates { get; set; }

        /// <summary>
        /// Dictionary per buff that contains <see cref="JsonBuffStackStatus"/>/>
        /// </summary>
        public Dictionary<string, JsonBuffStackStatus> BuffStackStates { get; set; }
        /// <summary>
        /// Dictionary per boon that contains an array of <see cref="JsonBuffRemoveItem"/>
        /// </summary>
        public Dictionary<string, List<JsonBuffRemoveItem>> BoonRemoveStatus { get; set; }
        /// <summary>
        /// Dictionary per condition that contains an array of <see cref="JsonBuffRemoveItem"/>
        /// </summary>
        public Dictionary<string, List<JsonBuffRemoveItem>> ConditionRemoveStatus { get; set; }

        public Dictionary<string, List<JsonBuffWasteItem>> BuffWasteStates { get; set; }
        public Dictionary<string, List<JsonBuffWasteItem>> BuffOverstackStates { get; set; }

        private static void SetBuffRemoveItems(List<Buff> buffsToUse, Dictionary<long, List<BuffRemoveAllEvent>> buffRemoveAlls, Dictionary<string, List<JsonBuffRemoveItem>> toFill, ParsedLog log, Dictionary<string, Desc> description)
        {
            foreach (Buff buffToUse in buffsToUse)
            {
                if (buffRemoveAlls.TryGetValue(buffToUse.ID, out List<BuffRemoveAllEvent> buffRemoves))
                {
                    var id = "b" + buffToUse.ID;
                    if (!description.ContainsKey(id))
                    {
                        description[id] = new BuffDesc(buffToUse);
                    }
                    toFill[id] = buffRemoves.Select(x => new JsonBuffRemoveItem(x, log, description)).ToList();
                }
            }
        }

        public JsonBuffData(ParsedLog log, AbstractSingleActor actor, Dictionary<string, Desc> description)
        {
            (/*Buffs, */BuffStates, BuffStackStates, BuffOverstackStates, BuffWasteStates) = GetJsonBuffs(actor, log, description);
            Dictionary<long, List<BuffRemoveAllEvent>> buffRemoveAlls = actor.GetBuffRemoveAllByID(log);
            //
            ConditionRemoveStatus = new Dictionary<string, List<JsonBuffRemoveItem>>();
            List<Buff> conditions = log.Buffs.BuffsByNature[Buff.BuffNature.Condition];
            SetBuffRemoveItems(conditions, buffRemoveAlls, ConditionRemoveStatus, log, description);
            if (!ConditionRemoveStatus.Any())
            {
                ConditionRemoveStatus = null;
            }
            //
            BoonRemoveStatus = new Dictionary<string, List<JsonBuffRemoveItem>>();
            List<Buff> boons = log.Buffs.BuffsByNature[Buff.BuffNature.Condition];
            SetBuffRemoveItems(boons, buffRemoveAlls, BoonRemoveStatus, log, description);
            if (!BoonRemoveStatus.Any())
            {
                ConditionRemoveStatus = null;
            }
        }

    }
}
