﻿using GW2EIParser.EIData;

namespace GW2EIParser.Parser.ParsedData.CombatEvents
{
    public class BuffExtensionEvent : BuffApplyEvent
    {
        private readonly long _oldValue;

        public BuffExtensionEvent(CombatItem evtcItem, AgentData agentData, SkillData skillData, long offset) : base(evtcItem, agentData, skillData, offset)
        {
            if (By == GeneralHelper.UnknownAgent)
            {
                By = null;
            }
            _oldValue = evtcItem.OverstackValue - evtcItem.Value;
        }

        public override void TryFindSrc(ParsedLog log)
        {
            if (By == null)
            {
                By = log.Buffs.TryFindSrc(To, Time, AppliedDuration, log, BuffID);
            }
        }

        public override void UpdateSimulator(BuffSimulator simulator)
        {
            simulator.Extend(AppliedDuration, _oldValue, By, Time);
        }
    }
}
