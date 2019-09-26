using GW2EIParser.EIData;

namespace GW2EIParser.Parser.ParsedData.CombatEvents
{
    public class BuffStackActiveEvent : AbstractBuffStackEvent
    {

        public BuffStackActiveEvent(CombatItem evtcItem, AgentData agentData, SkillData skillData, long offset) : base(evtcItem, agentData, skillData, offset)
        {
            BuffInstance = (uint)evtcItem.DstAgent;
        }

        public BuffStackActiveEvent(AgentItem to, SkillItem buffSkill, long time, uint buffInstance) : base(to, buffSkill, time)
        {
            BuffInstance = buffInstance;
        }

        public override void UpdateSimulator(AbstractBuffSimulator simulator)
        {
            simulator.Activate(BuffInstance);
        }

        public override bool IsBuffSimulatorCompliant(long fightEnd, bool hasStackIDs)
        {
            return BuffID != ProfHelper.NoBuff && hasStackIDs && BuffInstance != 0;
        }
        public override int CompareTo(AbstractBuffEvent abe)
        {
            if (abe is BuffStackActiveEvent)
            {
                return 0;
            }
            return 1;
        }
    }
}
