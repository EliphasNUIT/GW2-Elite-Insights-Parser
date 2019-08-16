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

        protected override void SetBoonStatusCleanseWasteData(ParsedLog log, BoonSimulator simulator, long boonid, bool updateCondiPresence)
        {
        }

        protected override void SetBoonStatusGenerationData(ParsedLog log, BoonSimulationItem simul, long boonid)
        {
        }

        protected override void InitBoonStatusData(ParsedLog log)
        {
        }
    }
}
