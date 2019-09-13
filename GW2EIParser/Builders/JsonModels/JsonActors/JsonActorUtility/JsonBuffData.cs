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
                public long ID { get; set; }

                public JsonBuffStackStatusSources(BuffSimulationItemDuration item, ParsedLog log, Dictionary<string, Desc> description, bool noDuration)
                {
                    Src = GetActorID(item.Src, log, description);
                    if (item.IsExtension)
                    {
                        SeedSrc = GetActorID(item.SeedSrc, log, description);
                    }
                    Duration = noDuration ? 0 : item.Duration;
                    ID = item.ID;
                }
            }

            public List<JsonBuffStackStatusData> StackData { get; set; }
            public List<List<JsonBuffStackStatusSources>> StackStatus { get; set; }
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

        public abstract class JsonBuffWasteItem
        {
            public string SrcID { get; set; }
            [DefaultValue(null)]
            public long Time { get; set; }
            public long Wasted { get; set; }
            public long ID { get; set; }

            protected JsonBuffWasteItem(BuffWasteItem item, ParsedLog log, Dictionary<string, Desc> description)
            {
                SrcID = GetActorID(item.Src, log, description);
                Time = item.Time;
                Wasted = item.Waste;
                ID = item.ID;
            }
        }

        public class JsonBuffOverstackItem : JsonBuffWasteItem
        {
            public JsonBuffOverstackItem(BuffOverstackItem item, ParsedLog log, Dictionary<string, Desc> description) : base(item, log, description)
            {
                ID = 0;
            }
        }

        public class JsonBuffOverrideItem : JsonBuffWasteItem
        {
            public string CauseID { get; }
            public JsonBuffOverrideItem(BuffOverrideItem item, ParsedLog log, Dictionary<string, Desc> description) : base(item, log, description)
            {
                CauseID = GetActorID(item.By, log, description);
            }
        }

        public class JsonBuffRemoveItem : JsonBuffWasteItem
        {
            public string RemoveSrcID { get; }
            public JsonBuffRemoveItem(BuffRemoveItem item, ParsedLog log, Dictionary<string, Desc> description) : base(item, log, description)
            {
                RemoveSrcID = GetActorID(item.By, log, description);
            }
        }

        public class JsonCreationItem
        {
            public string SrcID { get; set; }
            [DefaultValue(null)]
            public long Time { get; set; }
            public long Added { get; set; }
            public long ID { get; set; }

            public JsonCreationItem(BuffCreationItem item, ParsedLog log, Dictionary<string, Desc> description)
            {
                SrcID = GetActorID(item.Src, log, description);
                Time = item.Time;
                Added = item.Added;
                ID = item.ID;
            }
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
        /// Dictionary per buff that contains an array of <see cref="JsonBuffRemoveItem"/>
        /// </summary>
        public Dictionary<string, List<JsonBuffRemoveItem>> BuffRemoveStatus { get; set; }

        public Dictionary<string, List<JsonBuffOverrideItem>> BuffOverrideStates { get; set; }
        public Dictionary<string, List<JsonBuffOverstackItem>> BuffOverstackStates { get; set; }
        public Dictionary<string, List<JsonCreationItem>> BuffAddedStates { get; set; }
        public Dictionary<string, List<JsonCreationItem>> BuffExtendedStates { get; set; }

        private static void RemoveNullsFromDictionary<T>(Dictionary<string, List<T>> dict)
        {
            var nullkeys = dict.Where(pair => pair.Value == null)
                       .Select(pair => pair.Key)
                       .ToList();
            foreach (string key in nullkeys)
            {
                dict.Remove(key);
            }
        }

        public JsonBuffData(ParsedLog log, AbstractSingleActor actor, Dictionary<string, Desc> description)
        {
            BuffStates = new Dictionary<string, List<int>>();
            BuffStackStates = new Dictionary<string, JsonBuffStackStatus>();
            BuffOverrideStates = new Dictionary<string, List<JsonBuffOverrideItem>>();
            BuffRemoveStatus = new Dictionary<string, List<JsonBuffRemoveItem>>();
            BuffOverstackStates = new Dictionary<string, List<JsonBuffOverstackItem>>();
            BuffAddedStates = new Dictionary<string, List<JsonCreationItem>>();
            BuffExtendedStates = new Dictionary<string, List<JsonCreationItem>>();
            Dictionary<long, BuffsGraphModel> buffGraphs = actor.GetBuffGraphs(log);
            foreach (long buffID in buffGraphs.Keys)
            {
                Buff buff = log.Buffs.BuffsByIds[buffID];

                string id = "b" + buffID;
                if (!description.ContainsKey(id))
                {
                    description[id] = new BuffDesc(buff);
                }
                if (!BuffStates.ContainsKey(id) && buffGraphs.TryGetValue(buffID, out BuffsGraphModel bgm))
                {
                    BuffStates[id] = bgm.GetStatesList();
                    if (bgm.IsSourceBased)
                    {
                        BuffStackStates[id] = bgm.GetStackStatusList(log, description);
                        (BuffOverstackStates[id], BuffOverrideStates[id], BuffRemoveStatus[id]) = bgm.GetWasteStatusList(log, description);
                        (BuffAddedStates[id], BuffExtendedStates[id]) = bgm.GetCreationStatusList(log, description);
                    }
                }
            }
            //
            RemoveNullsFromDictionary(BuffOverstackStates);
            RemoveNullsFromDictionary(BuffOverrideStates);
            RemoveNullsFromDictionary(BuffRemoveStatus);
            RemoveNullsFromDictionary(BuffAddedStates);
            RemoveNullsFromDictionary(BuffExtendedStates);
        }

    }
}
