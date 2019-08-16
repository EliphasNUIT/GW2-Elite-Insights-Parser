using GW2EIParser.Parser;
using GW2EIParser.Parser.ParsedData;

namespace GW2EIParser.EIData
{
    public class Minion : AbstractActor
    {

        public Minion(AgentItem agent) : base(agent)
        {
        }

        protected override void SetDamageLogs(ParsedLog log)
        {
            AddDamageLogs(log.CombatData.GetDamageData(AgentItem));
        }

        protected override void SetBuffStatusCleanseWasteData(ParsedLog log, BuffSimulator simulator, long boonid, bool updateCondiPresence)
        {
        }

        protected override void SetBuffStatusGenerationData(ParsedLog log, BuffSimulationItem simul, long boonid)
        {
        }

        protected override void InitBuffStatusData(ParsedLog log)
        {
        }
    }
}
