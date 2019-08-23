using GW2EIParser.Parser;

namespace GW2EIParser.EIData
{
    public class AgentConnector : Connector
    {
        private AbstractMasterActor _agent;

        public AgentConnector(AbstractMasterActor agent)
        {
            _agent = agent;
        }

        public override object GetConnectedTo(CombatReplayMap map, ParsedLog log)
        {
            return _agent.GetCombatReplayID(log);
        }
    }
}
