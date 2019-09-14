using GW2EIParser.Parser.ParsedData;

namespace GW2EIParser.EIData
{
    public class BuffOverrideItem : BuffWasteItem
    {
        public AgentItem By { get; }

        public BuffOverrideItem(AgentItem src, AgentItem by, long over, long time, long id) : base(src, over, time, id)
        {
            By = by;
        }
    }
}
