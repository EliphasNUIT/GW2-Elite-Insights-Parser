using GW2EIParser.EIData;

namespace GW2EIParser.Parser.ParsedData.CombatEvents
{
    public class BuffStackResetEvent : AbstractBuffStackEvent
    {
        private readonly int _resetToDuration;
        public BuffStackResetEvent(CombatItem evtcItem, AgentData agentData, SkillData skillData, long offset) : base(evtcItem, agentData, skillData, offset)
        {
            BuffInstance = evtcItem.Pad;
            _resetToDuration = evtcItem.Value;
        }

        public override void UpdateSimulator(AbstractBuffSimulator simulator)
        {
            
        }
    }
}
