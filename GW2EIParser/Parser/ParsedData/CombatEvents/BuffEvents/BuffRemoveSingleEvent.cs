using GW2EIParser.EIData;

namespace GW2EIParser.Parser.ParsedData.CombatEvents
{
    public class BuffRemoveSingleEvent : AbstractBuffRemoveEvent
    {
        private readonly ParseEnum.EvtcIFF _iff;
        public BuffRemoveSingleEvent(CombatItem evtcItem, AgentData agentData, SkillData skillData, long offset) : base(evtcItem, agentData, skillData, offset)
        {
            _iff = evtcItem.IFF;
        }

        public BuffRemoveSingleEvent(AgentItem by, AgentItem to, long time, int removedDuration, SkillItem buffSkill, uint id, ParseEnum.EvtcIFF iff) : base(by, to, time, removedDuration, buffSkill)
        {
            _iff = iff;
            BuffInstance = id;
        }

        public override bool IsBoonSimulatorCompliant(long fightEnd, bool hasStackIDs)
        {
            return BuffID != ProfHelper.NoBuff &&
                !(_iff == ParseEnum.EvtcIFF.Unknown && By == GeneralHelper.UnknownAgent && !hasStackIDs) && // weird single stack remove
                !(RemovedDuration <= 50 && RemovedDuration != 0 && !hasStackIDs) &&// low value single stack remove that can mess up with the simulator if server delay
                 Time <= fightEnd - 50; // don't take into account removal that are close to the end of the fight
        }

        public override void UpdateSimulator(AbstractBuffSimulator simulator)
        {
            simulator.Remove(By, RemovedDuration, Time, ParseEnum.EvtcBuffRemove.Single, BuffInstance);
        }

        public override int CompareTo(AbstractBuffEvent abe)
        {
            if (abe is BuffRemoveSingleEvent)
            {
                return 0;
            }
            return 1;
        }
    }
}
