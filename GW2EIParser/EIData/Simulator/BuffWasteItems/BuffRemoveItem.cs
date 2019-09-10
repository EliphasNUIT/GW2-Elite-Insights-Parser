using GW2EIParser.Parser.ParsedData;

namespace GW2EIParser.EIData
{
    public class BuffRemoveItem : BuffWasteItem
    {
        public AgentItem By { get; }

        public BuffRemoveItem(AgentItem src, AgentItem by,  long waste, long time, long id) : base(src, waste, time, id)
        {
            By = by;
        }
    }
}
