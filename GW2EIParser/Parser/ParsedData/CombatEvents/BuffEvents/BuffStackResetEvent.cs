﻿using GW2EIParser.EIData;

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

        public override bool IsBuffSimulatorCompliant(long fightEnd, bool hasStackIDs)
        {
            return false; // ignore reset event
        }

        public override void UpdateSimulator(AbstractBuffSimulator simulator)
        {
            simulator.Reset(BuffInstance, _resetToDuration);
        }
        public override int CompareTo(AbstractBuffEvent abe)
        {
            if (abe is BuffStackActiveEvent)
            {
                return 1;
            }
            if (abe is BuffStackResetEvent)
            {
                return 0;
            }
            return -1;
        }
    }
}
