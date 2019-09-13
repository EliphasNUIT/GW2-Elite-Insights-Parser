using GW2EIParser.EIData;

namespace GW2EIParser.Parser.ParsedData.CombatEvents
{
    public class BuffApplyEvent : AbstractBuffEvent
    {
        public bool Initial { get; }
        public int AppliedDuration { get; }

        private readonly uint _buffInstance;
        private readonly bool _addedActive;

        public BuffApplyEvent(CombatItem evtcItem, AgentData agentData, SkillData skillData, long offset) : base(evtcItem, skillData, offset)
        {
            Initial = evtcItem.IsStateChange == ParseEnum.EvtcStateChange.BuffInitial;
            AppliedDuration = evtcItem.Value;
            By = agentData.GetAgentByInstID(evtcItem.SrcInstid, evtcItem.LogTime);
            ByMaster = evtcItem.SrcMasterInstid > 0 ? agentData.GetAgentByInstID(evtcItem.SrcMasterInstid, evtcItem.LogTime) : null;
            To = agentData.GetAgentByInstID(evtcItem.DstInstid, evtcItem.LogTime);
            _buffInstance = evtcItem.Pad;
            _addedActive = evtcItem.IsShields > 0;
        }

        public BuffApplyEvent(AgentItem by, AgentItem to, long time, int duration, SkillItem buffSkill) : base(buffSkill, time)
        {
            AppliedDuration = duration;
            By = by;
            To = to;
        }

        public override bool IsBoonSimulatorCompliant(long fightEnd)
        {
            return BuffID != ProfHelper.NoBuff;
        }

        public override void TryFindSrc(ParsedLog log)
        {
        }

        public override void UpdateSimulator(BuffSimulator simulator)
        {
            simulator.Add(AppliedDuration, By, Time);
        }
    }
}
