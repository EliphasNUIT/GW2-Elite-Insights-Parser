using GW2EIParser.Parser.ParsedData;

namespace GW2EIParser.EIData
{
    public class BuffSimulationItemWasted : AbstractBuffSimulationItem
    {
        public AgentItem Src { get; }
        public long Waste { get; }
        public long Time { get; }

        public BuffSimulationItemWasted(AgentItem src, long waste, long time)
        {
            Src = src;
            Waste = waste;
            Time = time;
        }

        protected long GetValue(long start, long end)
        {
            return (start <= Time && Time <= end) ? Waste : 0;
        }
    }
}
