using GW2EIParser.Parser.ParsedData;

namespace GW2EIParser.EIData
{
    public class BuffSimulationItemWasted
    {
        public AgentItem Src { get; }
        public long Waste { get; }
        public long Time { get; }

        public long ID { get; }

        public BuffSimulationItemWasted(AgentItem src, long waste, long time, long id)
        {
            Src = src;
            Waste = waste;
            Time = time;
            ID = id;
        }
    }
}
