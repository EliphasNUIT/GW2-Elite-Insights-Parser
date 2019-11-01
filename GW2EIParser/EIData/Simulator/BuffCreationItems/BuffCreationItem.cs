using GW2EIParser.Parser.ParsedData;

namespace GW2EIParser.EIData
{
    public class BuffCreationItem
    {
        public AgentItem Src { get; }
        public long Added { get; }
        public long Time { get; }

        public long ID { get; }

        public BuffCreationItem(AgentItem src, long added, long time, long id)
        {
            Src = src;
            Added = added;
            Time = time;
            ID = id;
        }
    }
}

