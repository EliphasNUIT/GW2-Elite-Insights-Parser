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
            
        }
    }
}
