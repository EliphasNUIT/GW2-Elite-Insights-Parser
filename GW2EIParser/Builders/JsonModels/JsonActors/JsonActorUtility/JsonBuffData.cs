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
        /// <summary>
        /// Represents a stack status item for buffs
        /// </summary>
        public class JsonBuffStackStatus
        {
            public class JsonBuffStackStatusSources
            {
                public string Src { get; set; }
                public long Duration { get; set; }
                public string SeedSrc { get; set; }
                public long ID { get; set; }

                public JsonBuffStackStatusSources(BuffSimulationItemDuration item, ParsedLog log, Dictionary<string, Desc> description)
                {
                    Src = GetActorID(item.Src, log, description);
                    if (item.IsExtension)
                    {
                        SeedSrc = GetActorID(item.SeedSrc, log, description);
                    }
                    Duration = item.Duration;
                    ID = item.ID;
                }
            }

            public List<long> StackStarts { get; set; }
            public List<List<JsonBuffStackStatusSources>> StackStatus { get; set; }
            public JsonBuffStackStatus(List<BuffSimulationItem> sourceBasedBoonChart, ParsedLog log, Dictionary<string, Desc> description)
            {
                StackStarts = new List<long>();
                StackStatus = new List<List<JsonBuffStackStatusSources>>();
                foreach (BuffSimulationItem item in sourceBasedBoonChart)
                {
                    StackStarts.Add(item.Start);
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


        public class JsonBuffDataItem
        {

            /// <summary>
            /// an array of int that represents the number of buff status \n
            /// Value[2*i] will be the time, value[2*i+1] will be the number of buff present from value[2*i] to value[2*(i+1)] \n
            /// If i corresponds to the last element that means the status did not change for the remainder of the fight \n
            /// </summary>
            public List<int> BuffStates { get; set; }

            /// <summary>
            /// contains <see cref="JsonBuffStackStatus"/>/>
            /// </summary>
            public JsonBuffStackStatus BuffStackStates { get; set; }
            /// <summary>
            /// an array of <see cref="JsonBuffRemoveItem"/>
            /// </summary>
            public List<JsonBuffRemoveItem> BuffRemoveStatus { get; set; }

            public List<JsonBuffOverrideItem> BuffOverrideStates { get; set; }
            public List<JsonBuffOverstackItem> BuffOverstackStates { get; set; }
            public List<JsonCreationItem> BuffAddedStates { get; set; }
            public List<JsonCreationItem> BuffExtendedStates { get; set; }

            public string ID { get; set; }

            public JsonBuffDataItem(string id, BuffsGraphModel bgm, ParsedLog log, Dictionary<string, Desc> description)
            {
                ID = id;
                BuffStates = bgm.GetStatesList();
                if (bgm.IsSourceBased)
                {
                    BuffStackStates = bgm.GetStackStatusList(log, description);
                    (BuffOverstackStates, BuffOverrideStates, BuffRemoveStatus) = bgm.GetWasteStatusList(log, description);
                    (BuffAddedStates, BuffExtendedStates) = bgm.GetCreationStatusList(log, description);
                }
            }
        }

        public List<JsonBuffDataItem> Data { get; } = new List<JsonBuffDataItem>();

        public JsonBuffData(ParsedLog log, AbstractSingleActor actor, Dictionary<string, Desc> description)
        {
            Dictionary<long, BuffsGraphModel> buffGraphs = actor.GetBuffGraphs(log);
            foreach (long buffID in buffGraphs.Keys)
            {
                Buff buff = log.Buffs.BuffsByIds[buffID];

                string id = "b" + buffID;
                if (!description.ContainsKey(id))
                {
                    description[id] = new BuffDesc(buff);
                }
                Data.Add(new JsonBuffDataItem(id, buffGraphs[buffID], log, description));
            }
        }

    }
}
