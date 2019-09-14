using GW2EIParser.EIData;

namespace GW2EIParser.Parser.ParsedData.CombatEvents
{
    public class BuffStackActiveEvent : AbstractBuffStackEvent
    {

        public BuffStackActiveEvent(CombatItem evtcItem, AgentData agentData, SkillData skillData, long offset) : base(evtcItem, agentData, skillData, offset)
        {
            BuffInstance = (uint)evtcItem.DstAgent;
        }

        public override void UpdateSimulator(AbstractBuffSimulator simulator)
        {
            simulator.Activate(BuffInstance);
        }
        public override int CompareTo(AbstractBuffEvent abe)
        {
            if (abe is BuffStackActiveEvent)
            {
                return 0;
            }
            return -1;
        }
    }
}
