namespace GW2EIParser.Parser.ParsedData.CombatEvents
{
    public class DirectDamageEvent : AbstractDamageEvent
    {
        public DirectDamageEvent(CombatItem evtcItem, AgentData agentData, SkillData skillData, long offset) : base(evtcItem, agentData, skillData, offset)
        {
            Damage = evtcItem.Value;
            ParseEnum.EvtcPhysicalResult result = ParseEnum.GetPhysicalResult(evtcItem.Result);
            IsAbsorbed = result == ParseEnum.EvtcPhysicalResult.Absorb;
            IsBlind = result == ParseEnum.EvtcPhysicalResult.Blind;
            IsBlocked = result == ParseEnum.EvtcPhysicalResult.Block;
            HasCrit = result == ParseEnum.EvtcPhysicalResult.Crit;
            HasDowned = result == ParseEnum.EvtcPhysicalResult.Downed;
            IsEvaded = result == ParseEnum.EvtcPhysicalResult.Evade;
            HasGlanced = result == ParseEnum.EvtcPhysicalResult.Glance;
            HasHit = result == ParseEnum.EvtcPhysicalResult.Normal || result == ParseEnum.EvtcPhysicalResult.Crit || result == ParseEnum.EvtcPhysicalResult.Glance || result == ParseEnum.EvtcPhysicalResult.KillingBlow; //Downed and Interrupt omitted for now due to double procing mechanics || result == ParseEnum.PhysicalResult.Downed || result == ParseEnum.PhysicalResult.Interrupt;
            HasKilled = result == ParseEnum.EvtcPhysicalResult.KillingBlow;
            HasInterrupted = result == ParseEnum.EvtcPhysicalResult.Interrupt;
            ShieldDamage = evtcItem.IsShields > 0 ? (int)evtcItem.OverstackValue : 0;
        }

        public override bool IsCondi(ParsedLog log)
        {
            return false;
        }
    }
}
