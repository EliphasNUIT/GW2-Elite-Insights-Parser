using GW2EIParser.EIData;

namespace GW2EIParser.Parser.ParsedData.CombatEvents
{
    public abstract class AbstractBuffStackEvent : AbstractBuffEvent
    {
        protected uint BuffInstance {get; set;}

        public AbstractBuffStackEvent(CombatItem evtcItem, AgentData agentData, SkillData skillData, long offset) : base(evtcItem, skillData, offset)
        {
            To = agentData.GetAgent(evtcItem.SrcAgent, evtcItem.LogTime);
        }

        public override bool IsBoonSimulatorCompliant(long fightEnd, bool hasStackIDs)
        {
            return BuffID != ProfHelper.NoBuff && hasStackIDs && BuffInstance != 0;
        }

        public override void TryFindSrc(ParsedLog log)
        {
        }
    }
}
