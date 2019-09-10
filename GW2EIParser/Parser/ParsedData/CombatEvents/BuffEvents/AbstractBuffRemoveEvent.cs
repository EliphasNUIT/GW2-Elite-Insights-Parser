﻿namespace GW2EIParser.Parser.ParsedData.CombatEvents
{
    public abstract class AbstractBuffRemoveEvent : AbstractBuffEvent
    {
        public int RemovedDuration { get; }

        public AbstractBuffRemoveEvent(CombatItem evtcItem, AgentData agentData, SkillData skillData, long offset) : base(evtcItem, skillData, offset)
        {
            RemovedDuration = evtcItem.Value;
            By = agentData.GetAgentByInstID(evtcItem.DstInstid, evtcItem.LogTime);
            ByMaster = evtcItem.DstMasterInstid > 0 ? agentData.GetAgentByInstID(evtcItem.DstMasterInstid, evtcItem.LogTime) : null;
            To = agentData.GetAgentByInstID(evtcItem.SrcInstid, evtcItem.LogTime);
        }

        public AbstractBuffRemoveEvent(AgentItem by, AgentItem to, long time, int removedDuration, SkillItem buffSkill) : base(buffSkill, time)
        {
            RemovedDuration = removedDuration;
            By = by;
            To = to;
        }

        public override void TryFindSrc(ParsedLog log)
        {
        }
    }
}
