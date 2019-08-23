using System.Collections.Generic;
using System.Linq;

namespace GW2EIParser.Parser.ParsedData.CombatEvents
{
    public static class CombatEventFactory
    {

        public static Dictionary<AgentItem, List<AbstractMovementEvent>> CreateMovementEvents(List<CombatItem> movementEvents, AgentData agentData, long offset)
        {
            Dictionary<AgentItem, List<AbstractMovementEvent>> res = new Dictionary<AgentItem, List<AbstractMovementEvent>>();
            foreach (CombatItem c in movementEvents)
            {
                AbstractMovementEvent evt = null;
                switch (c.IsStateChange)
                {
                    case ParseEnum.EvtcStateChange.Velocity:
                        evt = new VelocityEvent(c, agentData, offset);
                        break;
                    case ParseEnum.EvtcStateChange.Rotation:
                        evt = new RotationEvent(c, agentData, offset);
                        break;
                    case ParseEnum.EvtcStateChange.Position:
                        evt = new PositionEvent(c, agentData, offset);
                        break;
                    default:
                        break;
                }
                if (evt != null)
                {
                    if (res.TryGetValue(evt.AgentItem, out var list))
                    {
                        list.Add(evt);
                    } else
                    {
                        res[evt.AgentItem] = new List<AbstractMovementEvent>()
                        {
                            evt
                        };
                    }
                }
            }
            return res;
        }

        public static void CreateStateChangeEvents(List<CombatItem> stateChangeEvents, MetaEventsContainer metaDataEvents, StatusEventsContainer statusEvents, AgentData agentData, long offset) 
        {
            foreach (CombatItem c in stateChangeEvents)
            {
                switch (c.IsStateChange)
                {
                    case ParseEnum.EvtcStateChange.EnterCombat:
                        EnterCombatEvent enterCombatEvt = new EnterCombatEvent(c, agentData, offset);
                        GeneralHelper.Add(statusEvents.EnterCombatEvents, enterCombatEvt.Src, enterCombatEvt);
                        break;
                    case ParseEnum.EvtcStateChange.ExitCombat:
                        ExitCombatEvent exitCombatEvt = new ExitCombatEvent(c, agentData, offset);
                        GeneralHelper.Add(statusEvents.ExitCombatEvents, exitCombatEvt.Src, exitCombatEvt);
                        break;
                    case ParseEnum.EvtcStateChange.ChangeUp:
                        AliveEvent aliveEvt = new AliveEvent(c, agentData, offset);
                        GeneralHelper.Add(statusEvents.AliveEvents, aliveEvt.Src, aliveEvt);
                        break;
                    case ParseEnum.EvtcStateChange.ChangeDead:
                        DeadEvent deadEvt = new DeadEvent(c, agentData, offset);
                        GeneralHelper.Add(statusEvents.DeadEvents, deadEvt.Src, deadEvt);
                        break;
                    case ParseEnum.EvtcStateChange.ChangeDown:
                        DownEvent downEvt = new DownEvent(c, agentData, offset);
                        GeneralHelper.Add(statusEvents.DownEvents, downEvt.Src, downEvt);
                        break;
                    case ParseEnum.EvtcStateChange.Spawn:
                        SpawnEvent spawnEvt = new SpawnEvent(c, agentData, offset);
                        GeneralHelper.Add(statusEvents.SpawnEvents, spawnEvt.Src, spawnEvt);
                        break;
                    case ParseEnum.EvtcStateChange.Despawn:
                        DespawnEvent despawnEvt = new DespawnEvent(c, agentData, offset);
                        GeneralHelper.Add(statusEvents.DespawnEvents, despawnEvt.Src, despawnEvt);
                        break;
                    case ParseEnum.EvtcStateChange.HealthUpdate:
                        HealthUpdateEvent healthEvt = new HealthUpdateEvent(c, agentData, offset);
                        GeneralHelper.Add(statusEvents.HealthUpdateEvents, healthEvt.Src, healthEvt);
                        break;
                    case ParseEnum.EvtcStateChange.LogStart:
                        metaDataEvents.LogStartEvents.Add(new LogStartEvent(c, offset));
                        break;
                    case ParseEnum.EvtcStateChange.LogEnd:
                        metaDataEvents.LogEndEvents.Add(new LogEndEvent(c, offset));
                        break;
                    case ParseEnum.EvtcStateChange.MaxHealthUpdate:
                        MaxHealthUpdateEvent maxHealthEvt = new MaxHealthUpdateEvent(c, agentData, offset);
                        GeneralHelper.Add(statusEvents.MaxHealthUpdateEvents, maxHealthEvt.Src, maxHealthEvt);
                        break;
                    case ParseEnum.EvtcStateChange.PointOfView:
                        metaDataEvents.PointOfViewEvents.Add(new PointOfViewEvent(c, agentData, offset));
                        break;
                    case ParseEnum.EvtcStateChange.Language:
                        metaDataEvents.LanguageEvents.Add(new LanguageEvent(c, offset));
                        break;
                    case ParseEnum.EvtcStateChange.GWBuild:
                        metaDataEvents.BuildEvents.Add(new BuildEvent(c, offset));
                        break;
                    case ParseEnum.EvtcStateChange.ShardId:
                        metaDataEvents.ShardEvents.Add(new ShardEvent(c, offset));
                        break;
                    case ParseEnum.EvtcStateChange.Reward:
                        metaDataEvents.RewardEvents.Add(new RewardEvent(c, offset));
                        break;
                    case ParseEnum.EvtcStateChange.TeamChange:
                        TeamChangeEvent tcEvt = new TeamChangeEvent(c, agentData, offset);
                        GeneralHelper.Add(statusEvents.TeamChangeEvents, tcEvt.Src, tcEvt);
                        break;
                    case ParseEnum.EvtcStateChange.AttackTarget:
                        AttackTargetEvent aTEvt = new AttackTargetEvent(c, agentData, offset);
                        GeneralHelper.Add(statusEvents.AttackTargetEvents, aTEvt.Src, aTEvt);
                        break;
                    case ParseEnum.EvtcStateChange.Targetable:
                        TargetableEvent tarEvt = new TargetableEvent(c, agentData, offset);
                        GeneralHelper.Add(statusEvents.TargetableEvents, tarEvt.Src, tarEvt);
                        break;
                    case ParseEnum.EvtcStateChange.MapID:
                        metaDataEvents.MapIDEvents.Add(new MapIDEvent(c, offset));
                        break;
                    case ParseEnum.EvtcStateChange.Guild:
                        GuildEvent gEvt = new GuildEvent(c, agentData, offset);
                        GeneralHelper.Add(statusEvents.GuildEvents, gEvt.Src, gEvt);
                        break;
                }
            }
        }

        public static List<WeaponSwapEvent> CreateWeaponSwapEvents(List<CombatItem> swapEvents, AgentData agentData, SkillData skillData, long offset)
        {
            List<WeaponSwapEvent> res = new List<WeaponSwapEvent>();
            foreach (CombatItem swapEvent in swapEvents)
            {
                res.Add(new WeaponSwapEvent(swapEvent, agentData, skillData, offset));
            }
            return res;
        }

        public static List<AbstractBuffEvent> CreateBuffEvents(List<CombatItem> buffEvents, AgentData agentData, SkillData skillData, long offset)
        {
            List<AbstractBuffEvent> res = new List<AbstractBuffEvent>();
            foreach (CombatItem c in buffEvents)
            {
                switch (c.IsBuffRemove)
                {
                    case ParseEnum.EvtcBuffRemove.None:
                        if (c.IsOffcycle > 0)
                        {
                            res.Add(new BuffExtensionEvent(c, agentData, skillData, offset));
                        }
                        else
                        {
                            res.Add(new BuffApplyEvent(c, agentData, skillData, offset));
                        }
                        break;
                    case ParseEnum.EvtcBuffRemove.Single:
                        res.Add(new BuffRemoveSingleEvent(c, agentData, skillData, offset));
                        break;
                    case ParseEnum.EvtcBuffRemove.All:
                        res.Add(new BuffRemoveAllEvent(c, agentData, skillData, offset));
                        break;
                    case ParseEnum.EvtcBuffRemove.Manual:
                        res.Add(new BuffRemoveManualEvent(c, agentData, skillData, offset));
                        break;
                }
            }
            return res;
        }

        public static List<AnimatedCastEvent> CreateCastEvents(List<CombatItem> castEvents, AgentData agentData, SkillData skillData, long offset)
        {
            List<AnimatedCastEvent> res = new List<AnimatedCastEvent>();
            Dictionary<ulong, List<CombatItem>> castEventsBySrcAgent = castEvents.GroupBy(x => x.SrcAgent).ToDictionary(x => x.Key, x => x.ToList());
            foreach (var pair in castEventsBySrcAgent)
            {
                CombatItem startItem = null;
                foreach (CombatItem c in pair.Value)
                {
                    if (c.IsActivation.StartCasting())
                    {
                        // missing end
                        if (startItem != null)
                        {
                            res.Add(new AnimatedCastEvent(startItem, agentData, skillData, offset, c.LogTime));
                        }
                        startItem = c;
                    } else
                    {
                        if (startItem != null && startItem.SkillID == c.SkillID)
                        {
                            res.Add(new AnimatedCastEvent(startItem, c, agentData, skillData, offset));
                            startItem = null;
                        }
                    }
                }
            }
            res.Sort((x, y) => x.Time.CompareTo(y.Time));
            return res;
        }

        public static List<AbstractDamageEvent> CreateDamageEvents(List<CombatItem> damageEvents, AgentData agentData, SkillData skillData, long offset)
        {
            List<AbstractDamageEvent> res = new List<AbstractDamageEvent>();
            foreach (CombatItem c in damageEvents)
            {
                if ((c.IsBuff != 0 && c.Value == 0))
                {
                    res.Add(new NonDirectDamageEvent(c, agentData, skillData, offset));
                }
                else if (c.IsBuff == 0)
                {
                    res.Add(new DirectDamageEvent(c, agentData, skillData, offset));
                }
            }
            return res;
        }

    }
}
