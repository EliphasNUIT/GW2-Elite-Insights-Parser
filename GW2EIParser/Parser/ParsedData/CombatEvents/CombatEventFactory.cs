﻿using System.Collections.Generic;
using System.Linq;

namespace GW2EIParser.Parser.ParsedData.CombatEvents
{
    public static class CombatEventFactory
    {

        public static Dictionary<AgentItem, List<AbstractMovementEvent>> CreateMovementEvents(List<CombatItem> movementEvents, AgentData agentData, long offset)
        {
            var res = new Dictionary<AgentItem, List<AbstractMovementEvent>>();
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
                    if (res.TryGetValue(evt.AgentItem, out List<AbstractMovementEvent> list))
                    {
                        list.Add(evt);
                    }
                    else
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
                        var enterCombatEvt = new EnterCombatEvent(c, agentData, offset);
                        GeneralHelper.Add(statusEvents.EnterCombatEvents, enterCombatEvt.Src, enterCombatEvt);
                        break;
                    case ParseEnum.EvtcStateChange.ExitCombat:
                        var exitCombatEvt = new ExitCombatEvent(c, agentData, offset);
                        GeneralHelper.Add(statusEvents.ExitCombatEvents, exitCombatEvt.Src, exitCombatEvt);
                        break;
                    case ParseEnum.EvtcStateChange.ChangeUp:
                        var aliveEvt = new AliveEvent(c, agentData, offset);
                        GeneralHelper.Add(statusEvents.AliveEvents, aliveEvt.Src, aliveEvt);
                        break;
                    case ParseEnum.EvtcStateChange.ChangeDead:
                        var deadEvt = new DeadEvent(c, agentData, offset);
                        GeneralHelper.Add(statusEvents.DeadEvents, deadEvt.Src, deadEvt);
                        break;
                    case ParseEnum.EvtcStateChange.ChangeDown:
                        var downEvt = new DownEvent(c, agentData, offset);
                        GeneralHelper.Add(statusEvents.DownEvents, downEvt.Src, downEvt);
                        break;
                    case ParseEnum.EvtcStateChange.Spawn:
                        var spawnEvt = new SpawnEvent(c, agentData, offset);
                        GeneralHelper.Add(statusEvents.SpawnEvents, spawnEvt.Src, spawnEvt);
                        break;
                    case ParseEnum.EvtcStateChange.Despawn:
                        var despawnEvt = new DespawnEvent(c, agentData, offset);
                        GeneralHelper.Add(statusEvents.DespawnEvents, despawnEvt.Src, despawnEvt);
                        break;
                    case ParseEnum.EvtcStateChange.HealthUpdate:
                        var healthEvt = new HealthUpdateEvent(c, agentData, offset);
                        GeneralHelper.Add(statusEvents.HealthUpdateEvents, healthEvt.Src, healthEvt);
                        break;
                    case ParseEnum.EvtcStateChange.LogStart:
                        metaDataEvents.LogStartEvents.Add(new LogStartEvent(c, offset));
                        break;
                    case ParseEnum.EvtcStateChange.LogEnd:
                        metaDataEvents.LogEndEvents.Add(new LogEndEvent(c, offset));
                        break;
                    case ParseEnum.EvtcStateChange.MaxHealthUpdate:
                        var maxHealthEvt = new MaxHealthUpdateEvent(c, agentData, offset);
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
                        var tcEvt = new TeamChangeEvent(c, agentData, offset);
                        GeneralHelper.Add(statusEvents.TeamChangeEvents, tcEvt.Src, tcEvt);
                        break;
                    case ParseEnum.EvtcStateChange.AttackTarget:
                        var aTEvt = new AttackTargetEvent(c, agentData, offset);
                        GeneralHelper.Add(statusEvents.AttackTargetEvents, aTEvt.Src, aTEvt);
                        break;
                    case ParseEnum.EvtcStateChange.Targetable:
                        var tarEvt = new TargetableEvent(c, agentData, offset);
                        GeneralHelper.Add(statusEvents.TargetableEvents, tarEvt.Src, tarEvt);
                        break;
                    case ParseEnum.EvtcStateChange.MapID:
                        metaDataEvents.MapIDEvents.Add(new MapIDEvent(c, offset));
                        break;
                    case ParseEnum.EvtcStateChange.Guild:
                        var gEvt = new GuildEvent(c, agentData, offset);
                        GeneralHelper.Add(statusEvents.GuildEvents, gEvt.Src, gEvt);
                        break;
                }
            }
        }

        public static List<WeaponSwapEvent> CreateWeaponSwapEvents(List<CombatItem> swapEvents, AgentData agentData, SkillData skillData, long offset)
        {
            var res = new List<WeaponSwapEvent>();
            foreach (CombatItem swapEvent in swapEvents)
            {
                res.Add(new WeaponSwapEvent(swapEvent, agentData, skillData, offset));
            }
            return res;
        }

        public static List<AbstractBuffEvent> CreateBuffEvents(List<CombatItem> buffEvents, AgentData agentData, SkillData skillData, long offset)
        {
            var res = new List<AbstractBuffEvent>();
            var dict = new Dictionary<uint, SkillItem>();
            foreach (CombatItem c in buffEvents)
            {
                switch (c.IsStateChange)
                {
                    case ParseEnum.EvtcStateChange.StackActive:
                        if (c.SkillID == 0)
                        {
                            SkillItem buffSkill = dict[(uint)c.DstAgent];
                            c.OverrideSkillID(buffSkill.ID);
                        }
                        res.Add(new BuffStackResetEvent(c, agentData, skillData, offset));
                        break;
                    case ParseEnum.EvtcStateChange.StackReset:
                        res.Add(new BuffStackResetEvent(c, agentData, skillData, offset));
                        break;
                    default:
                        switch (c.IsBuffRemove)
                        {
                            case ParseEnum.EvtcBuffRemove.None:
                                if (c.IsOffcycle > 0)
                                {
                                    res.Add(new BuffExtensionEvent(c, agentData, skillData, offset));
                                }
                                else
                                {
                                    var toAdd = new BuffApplyEvent(c, agentData, skillData, offset);
                                    res.Add(toAdd);
                                    dict[toAdd.BuffInstance] = toAdd.BuffSkill;
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
                        break;
                }
            }
            return res;
        }

        public static List<AnimatedCastEvent> CreateCastEvents(List<CombatItem> castEvents, AgentData agentData, SkillData skillData, long offset)
        {
            var res = new List<AnimatedCastEvent>();
            var castEventsBySrcAgent = castEvents.GroupBy(x => x.SrcAgent).ToDictionary(x => x.Key, x => x.ToList());
            foreach (KeyValuePair<ulong, List<CombatItem>> pair in castEventsBySrcAgent)
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
                    }
                    else
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
            var res = new List<AbstractDamageEvent>();
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
