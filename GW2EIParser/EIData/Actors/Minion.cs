using GW2EIParser.Parser;
using GW2EIParser.Parser.ParsedData;

namespace GW2EIParser.EIData
{
    public class Minion : AbstractSingleActor
    {

        public Minion(AgentItem agent) : base(agent)
        {
        }

        // Damage logs
        protected override void SetDamageLogs(ParsedLog log)
        {
            AddDamageLogs(log.CombatData.GetDamageData(AgentItem));
        }
    }
}
