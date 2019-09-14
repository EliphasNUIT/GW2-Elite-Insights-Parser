using GW2EIParser.Parser;

namespace GW2EIParser.EIData
{
    public abstract class Connector
    {
        public abstract object GetConnectedTo(CombatReplayMap map, ParsedLog log);
    }
}
