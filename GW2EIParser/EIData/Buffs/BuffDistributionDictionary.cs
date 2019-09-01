using System.Collections.Generic;
using System.Linq;
using GW2EIParser.Parser.ParsedData;

namespace GW2EIParser.EIData
{
    public class BuffDistributionItem
    {
        public long Generation { get; set; }
        public long Overstack { get; set; }
        public long Wasted { get; set; }
        public long UnknownExtended { get; set; }
        public long ByExtension { get; set; }
        public long Extended { get; set; }

        public BuffDistributionItem(long generation, long overstack, long wasted, long unknownExtension, long byExtension, long extended)
        {
            Generation = generation;
            Overstack = overstack;
            Wasted = wasted;
            UnknownExtended = unknownExtension;
            ByExtension = byExtension;
            Extended = extended;
        }
    }

    public class BuffDistributionDictionary : Dictionary<long, Dictionary<AgentItem, BuffDistributionItem>>
    {
        /*
        public bool HasSrc(long boonid, AgentItem src)
        {
            return ContainsKey(boonid) && this[boonid].ContainsKey(src);
        }

        public long GetUptime(long boonid)
        {
            if (!ContainsKey(boonid))
            {
                return 0;
            }
            return this[boonid].Sum(x => x.Value.Generation);
        }

        public long GetGeneration(long boonid, AgentItem src)
        {
            if (!ContainsKey(boonid) || !this[boonid].ContainsKey(src))
            {
                return 0;
            }
            return this[boonid][src].Generation;
        }

        public long GetOverstack(long boonid, AgentItem src)
        {
            if (!ContainsKey(boonid) || !this[boonid].ContainsKey(src))
            {
                return 0;
            }
            return this[boonid][src].Overstack;
        }

        public long GetWaste(long boonid, AgentItem src)
        {
            if (!ContainsKey(boonid) || !this[boonid].ContainsKey(src))
            {
                return 0;
            }
            return this[boonid][src].Wasted;
        }

        public long GetUnknownExtended(long boonid, AgentItem src)
        {
            if (!ContainsKey(boonid) || !this[boonid].ContainsKey(src))
            {
                return 0;
            }
            return this[boonid][src].UnknownExtended;
        }

        public long GetByExtension(long boonid, AgentItem src)
        {
            if (!ContainsKey(boonid) || !this[boonid].ContainsKey(src))
            {
                return 0;
            }
            return this[boonid][src].ByExtension;
        }

        public long GetExtended(long boonid, AgentItem src)
        {
            if (!ContainsKey(boonid) || !this[boonid].ContainsKey(src))
            {
                return 0;
            }
            return this[boonid][src].Extended;
        }
        */
    }
}
