using GW2EIParser.EIData;

namespace GW2EIParser.Parser.ParsedData.CombatEvents
{
    public class BuffApplyEvent : AbstractBuffEvent
    {
        public bool Initial { get; }
        public int AppliedDuration { get; }

        private uint _overstackDuration;

        public uint BuffInstance { get; }
        public bool AddedActive { get; }

        public BuffApplyEvent(CombatItem evtcItem, AgentData agentData, SkillData skillData, long offset) : base(evtcItem, skillData, offset)
        {
            Initial = evtcItem.IsStateChange == ParseEnum.StateChange.BuffInitial;
            AppliedDuration = evtcItem.Value;
            By = agentData.GetAgentByInstID(evtcItem.SrcInstid, evtcItem.LogTime);
            ByMaster = evtcItem.SrcMasterInstid > 0 ? agentData.GetAgentByInstID(evtcItem.SrcMasterInstid, evtcItem.LogTime) : null;
            To = agentData.GetAgentByInstID(evtcItem.DstInstid, evtcItem.LogTime);
            BuffInstance = evtcItem.Pad;
            AddedActive = evtcItem.IsShields > 0;
            _overstackDuration = evtcItem.OverstackValue;
        }

        public BuffApplyEvent(AgentItem by, AgentItem to, long time, int duration, SkillItem buffSkill, uint id) : base(buffSkill, time)
        {
            AppliedDuration = duration;
            By = by;
            To = to;
            BuffInstance = id;
        }

        public override bool IsBuffSimulatorCompliant(long fightEnd, bool hasStackIDs)
        {
            return BuffID != ProfHelper.NoBuff;
        }

        public override void TryFindSrc(ParsedLog log)
        {
        }

        public override void UpdateSimulator(AbstractBuffSimulator simulator)
        {
            simulator.Add(AppliedDuration, By, Time, BuffInstance, _overstackDuration);
        }

        public override int CompareTo(AbstractBuffEvent abe)
        {
            if (abe is BuffApplyEvent && !(abe is BuffExtensionEvent))
            {
                return 0;
            }
            return -1;
        }
    }
}
