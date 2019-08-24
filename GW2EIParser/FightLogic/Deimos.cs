﻿using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIParser.EIData;
using GW2EIParser.Parser;
using GW2EIParser.Parser.ParsedData;
using GW2EIParser.Parser.ParsedData.CombatEvents;
using static GW2EIParser.Parser.ParseEnum.EvtcNPCIDs;

namespace GW2EIParser.Logic
{
    public class Deimos : RaidLogic
    {

        private long _specialSplitLogTime = 0;

        public Deimos(ushort triggerID) : base(triggerID)
        {
            MechanicList.AddRange(new List<Mechanic>
            {
            new HitOnPlayerMechanic(37716, "Rapid Decay", new MechanicPlotlySetting("circle-open","rgb(0,0,0)"), "Oil","Rapid Decay (Black expanding oil)", "Black Oil",0),
            new FirstHitOnPlayerMechanic(37716, "Rapid Decay", new MechanicPlotlySetting("circle","rgb(0,0,0)"), "Oil Trigger","Rapid Decay Trigger (Black expanding oil)", "Black Oil Trigger",0),
            new EnemyCastStartMechanic(37846, "Off Balance", new MechanicPlotlySetting("diamond-tall","rgb(0,160,150)"), "TP CC","Off Balance (Saul TP Breakbar)", "Saul TP Start",0),
            new EnemyCastEndMechanic(37846, "Off Balance", new MechanicPlotlySetting("diamond-tall","rgb(255,0,0)"), "TP CC Fail","Failed Saul TP CC", "Failed CC (TP)",0, (ce,log) => ce.ActualDuration >= 2200),
            new EnemyCastEndMechanic(37846, "Off Balance", new MechanicPlotlySetting("diamond-tall","rgb(0,160,0)"), "TP CCed","Saul TP CCed", "CCed (TP)",0, (ce, log) => ce.ActualDuration < 2200),
            new EnemyCastStartMechanic(38272, "Boon Thief", new MechanicPlotlySetting("diamond-wide","rgb(0,160,150)"), "Thief CC","Boon Thief (Saul Breakbar)", "Boon Thief Start",0),
            new EnemyCastEndMechanic(38272, "Boon Thief", new MechanicPlotlySetting("diamond-wide","rgb(255,0,0)"), "Thief CC Fail","Failed Boon Thief CC", "Failed CC (Thief)",0,(ce,log) => ce.ActualDuration >= 4400),
            new EnemyCastEndMechanic(38272, "Boon Thief", new MechanicPlotlySetting("diamond-wide","rgb(0,160,0)"), "Thief CCed","Boon Thief CCed", "CCed (Thief)",0,(ce, log) => ce.ActualDuration < 4400),
            new HitOnPlayerMechanic(38208, "Annihilate", new MechanicPlotlySetting("hexagon","rgb(255,200,0)"), "Pizza","Annihilate (Cascading Pizza attack)", "Boss Smash",0),
            new HitOnPlayerMechanic(37929, "Annihilate", new MechanicPlotlySetting("hexagon","rgb(255,200,0)"), "Pizza","Annihilate (Cascading Pizza attack)", "Boss Smash",0),
            new HitOnPlayerMechanic(37980, "Demonic Shock Wave", new MechanicPlotlySetting("triangle-right-open","rgb(255,0,0)"), "10% RSmash","Knockback (right hand) in 10% Phase", "10% Right Smash",0),
            new HitOnPlayerMechanic(38046, "Demonic Shock Wave", new MechanicPlotlySetting("triangle-left-open","rgb(255,0,0)"), "10% LSmash","Knockback (left hand) in 10% Phase", "10% Left Smash",0),
            new HitOnPlayerMechanic(37982, "Demonic Shock Wave", new MechanicPlotlySetting("bowtie","rgb(255,0,0)"), "10% Double Smash","Knockback (both hands) in 10% Phase", "10% Double Smash",0),
            new PlayerBoonApplyMechanic(37733, "Tear Instability", new MechanicPlotlySetting("diamond","rgb(0,128,128)"), "Tear","Collected a Demonic Tear", "Tear",0),
            new HitOnPlayerMechanic(37613, "Mind Crush", new MechanicPlotlySetting("square","rgb(0,0,255)"), "Mind Crush","Hit by Mind Crush without Bubble Protection", "Mind Crush",0, (de,log) => de.Damage > 0),
            new PlayerBoonApplyMechanic(38187, "Weak Minded", new MechanicPlotlySetting("square-open","rgb(200,140,255)"), "Weak Mind","Weak Minded (Debuff after Mind Crush)", "Weak Minded",0),
            new PlayerBoonApplyMechanic(37730, "Chosen by Eye of Janthir", new MechanicPlotlySetting("circle","rgb(0,255,0)"), "Green","Chosen by the Eye of Janthir", "Chosen (Green)",0),
            new PlayerBoonApplyMechanic(38169, "Teleported", new MechanicPlotlySetting("circle-open","rgb(0,255,0)"), "TP","Teleport to/from Demonic Realm", "Teleport",0),
            new EnemyBoonApplyMechanic(38224, "Unnatural Signet", new MechanicPlotlySetting("square-open","rgb(0,255,255)"), "DMG Debuff","Double Damage Debuff on Deimos", "+100% Dmg Buff",0)
            });
            Extension = "dei";
            GenericFallBackMethod = FallBackMethod.None;
            Icon = "https://wiki.guildwars2.com/images/e/e0/Mini_Ragged_White_Mantle_Figurehead.png";
        }

        protected override CombatReplayMap GetCombatMapInternal()
        {
            return new CombatReplayMap("https://i.imgur.com/GCwOVVE.png",
                            (4400, 5753),
                            (-9542, 1932, -7004, 5250),
                            (-27648, -9216, 27648, 12288),
                            (11774, 4480, 14078, 5376));
        }

        protected override HashSet<ushort> GetUniqueTargetIDs()
        {
            return new HashSet<ushort>
            {
                (ushort)ParseEnum.EvtcNPCIDs.Deimos,
                (ushort)Thief,
                (ushort)Drunkard,
                (ushort)Gambler,
            };
        }

        private void SetUniqueID(AgentItem target, HashSet<ulong> gadgetAgents, AgentData agentData, List<CombatItem> combatData)
        {
            // get unique id for the fusion
            ushort instID = 0;
            Random rnd = new Random();
            while (agentData.InstIDValues.Contains(instID) || instID == 0)
            {
                instID = (ushort)rnd.Next(ushort.MaxValue / 2, ushort.MaxValue);
            }
            target.InstID = instID;
            agentData.Refresh();
            HashSet<ulong> allAgents = new HashSet<ulong>(gadgetAgents)
            {
                target.Agent
            };
            foreach (CombatItem c in combatData)
            {
                if (gadgetAgents.Contains(c.SrcAgent) && c.IsStateChange == ParseEnum.EvtcStateChange.MaxHealthUpdate)
                {
                    continue;
                }
                if (allAgents.Contains(c.SrcAgent))
                {
                    c.OverrideSrcValues(target.Agent, target.InstID);

                }
                if (allAgents.Contains(c.DstAgent))
                {
                    c.OverrideDstValues(target.Agent, target.InstID);
                }
            }
        }

        public override List<AbstractBuffEvent> SpecialBuffEventProcess(Dictionary<AgentItem, List<AbstractBuffEvent>> buffsByDst, Dictionary<AgentItem, List<AbstractBuffEvent>> buffsBySrc, Dictionary<long, List<AbstractBuffEvent>> buffsById, long offset, SkillData skillData)
        {
            NPC target = NPCs.Find(x => x.ID == TriggerID);
            if (target == null)
            {
                throw new InvalidOperationException("Target for success by combat exit not found");
            }
            List<AbstractBuffEvent> res = new List<AbstractBuffEvent>();
            if (buffsById.TryGetValue(38224, out var list))
            {
                foreach (AbstractBuffEvent bfe in list)
                {
                    if (bfe is BuffApplyEvent)
                    {
                        AbstractBuffEvent removal = list.FirstOrDefault(x => x is BuffRemoveAllEvent && x.Time > bfe.Time && x.Time < bfe.Time + 30000);
                        if (removal == null)
                        {
                            res.Add(new BuffRemoveAllEvent(GeneralHelper.UnknownAgent, target.AgentItem, bfe.Time + 10000, 0, skillData.Get(38224), 0, 0));
                            res.Add(new BuffRemoveManualEvent(GeneralHelper.UnknownAgent, target.AgentItem, bfe.Time + 10000, 0, skillData.Get(38224)));
                        }
                    }
                    else if (bfe is BuffRemoveAllEvent)
                    {
                        AbstractBuffEvent apply = list.FirstOrDefault(x => x is BuffApplyEvent && x.Time < bfe.Time && x.Time > bfe.Time - 30000);
                        if (apply == null)
                        {
                            res.Add(new BuffApplyEvent(GeneralHelper.UnknownAgent, target.AgentItem, bfe.Time - 10000, 10000, skillData.Get(38224)));
                        }
                    }
                }
            }
            return res;
        }

        public override void CheckSuccess(CombatData combatData, AgentData agentData, FightData fightData, HashSet<AgentItem> playerAgents)
        {
            base.CheckSuccess(combatData, agentData, fightData, playerAgents);
            if (!fightData.Success && _specialSplitLogTime > 0)
            {
                NPC target = NPCs.Find(x => x.ID == TriggerID);
                if (target == null)
                {
                    throw new InvalidOperationException("Target for success by combat exit not found");
                }
                List<AttackTargetEvent> attackTargets = combatData.GetAttackTargetEvents(target.AgentItem);
                if (attackTargets.Count == 0)
                {
                    return;
                }
                long specialSplitTime = fightData.ToFightSpace(_specialSplitLogTime);
                AgentItem attackTarget = attackTargets.Last().AttackTarget;
                List<ExitCombatEvent> playerExits = new List<ExitCombatEvent>();
                foreach (AgentItem a in playerAgents)
                {
                    playerExits.AddRange(combatData.GetExitCombatEvents(a));
                }
                ExitCombatEvent lastPlayerExit = playerExits.Count > 0 ? playerExits.MaxBy(x => x.Time) : null;
                TargetableEvent notAttackableEvent = combatData.GetTargetableEvents(attackTarget).LastOrDefault(x => !x.Targetable && x.Time > specialSplitTime);
                AbstractDamageEvent lastDamageTaken = combatData.GetDamageTakenData(target.AgentItem).LastOrDefault(x => (x.Damage > 0) && (playerAgents.Contains(x.From) || playerAgents.Contains(x.MasterFrom)));
                if (notAttackableEvent != null && lastDamageTaken != null && lastPlayerExit != null)
                {
                    fightData.SetSuccess(lastPlayerExit.Time > notAttackableEvent.Time + 1000, fightData.ToLogSpace(lastDamageTaken.Time));
                }
            }
        }

        private long AttackTargetSpecialParse(CombatItem targetable, AgentData agentData, List<CombatItem> combatData, HashSet<ulong> gadgetAgents)
        {
            if (targetable == null)
            {
                return 0;
            }
            long firstAware = targetable.LogTime;
            AgentItem targetAgent = agentData.GetAgentByInstID(targetable.SrcInstid, targetable.LogTime);
            if (targetAgent != GeneralHelper.UnknownAgent)
            {
                try
                {
                    string[] names = targetAgent.Name.Split('-');
                    if (ushort.TryParse(names[2], out ushort masterInstid))
                    {
                        CombatItem structDeimosDamageEvent = combatData.FirstOrDefault(x => x.LogTime >= firstAware && x.IFF == ParseEnum.EvtcIFF.Foe && x.DstInstid == masterInstid && x.IsStateChange == ParseEnum.EvtcStateChange.None && x.IsBuffRemove == ParseEnum.EvtcBuffRemove.None &&
                                ((x.IsBuff == 1 && x.BuffDmg >= 0 && x.Value == 0) ||
                                (x.IsBuff == 0 && x.Value >= 0)));
                        if (structDeimosDamageEvent != null)
                        {
                            gadgetAgents.Add(structDeimosDamageEvent.DstAgent);
                        }
                        CombatItem armDeimosDamageEvent = combatData.FirstOrDefault(x => x.LogTime >= firstAware && (x.SkillID == 37980 || x.SkillID == 37982 || x.SkillID == 38046) && x.SrcAgent != 0 && x.SrcInstid != 0);
                        if (armDeimosDamageEvent != null)
                        {
                            gadgetAgents.Add(armDeimosDamageEvent.SrcAgent);
                        }
                    };
                }
                catch
                {
                    // nothing to do
                }
            }
            return firstAware;
        }

        public override void SpecialParse(FightData fightData, AgentData agentData, List<CombatItem> combatData)
        {
            ComputeFightTargets(agentData, combatData);
            // Find target
            NPC target = NPCs.Find(x => x.ID == (ushort)ParseEnum.EvtcNPCIDs.Deimos);
            if (target == null)
            {
                throw new InvalidOperationException("Main target of the fight not found");
            }
            // enter combat
            CombatItem enterCombat = combatData.FirstOrDefault(x => x.SrcInstid == target.InstID && x.IsStateChange == ParseEnum.EvtcStateChange.EnterCombat && x.SrcInstid == target.InstID && x.LogTime <= target.LastAwareLogTime && x.LogTime >= target.FirstAwareLogTime);
            if (enterCombat != null)
            {
                fightData.OverrideStart(enterCombat.LogTime);
            }
            // Remove deimos despawn events as they are useless and mess with combat replay
            combatData.RemoveAll(x => x.IsStateChange == ParseEnum.EvtcStateChange.Despawn && x.SrcAgent == target.Agent);
            // invul correction
            CombatItem invulApp = combatData.FirstOrDefault(x => x.DstInstid == target.InstID && x.IsBuff != 0 && x.BuffDmg == 0 && x.Value > 0 && x.SkillID == 762);
            if (invulApp != null)
            {
                invulApp.OverrideValue((int)(target.LastAwareLogTime - invulApp.LogTime));
            }
            // Deimos gadgets
            CombatItem targetable = combatData.LastOrDefault(x => x.IsStateChange == ParseEnum.EvtcStateChange.Targetable && x.LogTime > combatData.First().LogTime && x.DstAgent > 0);
            HashSet<ulong> gadgetAgents = new HashSet<ulong>();
            long firstAware = AttackTargetSpecialParse(targetable, agentData, combatData, gadgetAgents);
            // legacy method
            if (firstAware == 0)
            {
                CombatItem armDeimosDamageEvent = combatData.FirstOrDefault(x => x.LogTime >= target.LastAwareLogTime && (x.SkillID == 37980 || x.SkillID == 37982 || x.SkillID == 38046) && x.SrcAgent != 0 && x.SrcInstid != 0);
                if (armDeimosDamageEvent != null)
                {
                    List<AgentItem> deimosGadgets = agentData.GetAgentByType(AgentItem.AgentType.Gadget).Where(x => x.Name.Contains("Deimos") && x.LastAwareLogTime > armDeimosDamageEvent.LogTime).ToList();
                    if (deimosGadgets.Count > 0)
                    {
                        firstAware = deimosGadgets.Max(x => x.FirstAwareLogTime);
                        gadgetAgents = new HashSet<ulong>(deimosGadgets.Select(x => x.Agent));
                    }
                }
            }
            if (gadgetAgents.Count > 0)
            {
                _specialSplitLogTime = (firstAware >= target.LastAwareLogTime ? firstAware : target.LastAwareLogTime);
                SetUniqueID(target.AgentItem, gadgetAgents, agentData, combatData);
            }
            target.AgentItem.LastAwareLogTime = combatData.Last().LogTime;
            target.OverrideName("Deimos");
        }

        public override List<PhaseData> GetPhases(ParsedLog log, bool requirePhases)
        {
            long start = 0;
            long end = 0;
            long fightDuration = log.FightData.FightDuration;
            List<PhaseData> phases = GetInitialPhase(log);
            NPC mainTarget = NPCs.Find(x => x.ID == (ushort)ParseEnum.EvtcNPCIDs.Deimos);
            if (mainTarget == null)
            {
                throw new InvalidOperationException("Main target of the fight not found");
            }
            phases[0].Targets.Add(mainTarget);
            if (!requirePhases)
            {
                return phases;
            }
            // Determined + additional data on inst change
            AbstractBuffEvent invulDei = log.CombatData.GetBuffData(762).Find(x => x is BuffApplyEvent && x.To == mainTarget.AgentItem);
            if (invulDei != null)
            {
                end = invulDei.Time;
                phases.Add(new PhaseData(start, end));
                start = (_specialSplitLogTime > 0 ? log.FightData.ToFightSpace(_specialSplitLogTime) : fightDuration);
                //mainTarget.AddCustomCastLog(end, -6, (int)(start - end), ParseEnum.Activation.None, (int)(start - end), ParseEnum.Activation.None, log);
            }
            else if (_specialSplitLogTime > 0)
            {
                long specialTime = log.FightData.ToFightSpace(_specialSplitLogTime);
                end = specialTime;
                phases.Add(new PhaseData(start, end));
                start = specialTime;
            }
            if (fightDuration - start > 5000 && start >= phases.Last().End)
            {
                phases.Add(new PhaseData(start, fightDuration));
            }
            string[] names = { "100% - 10%", "10% - 0%" };
            for (int i = 1; i < phases.Count; i++)
            {
                phases[i].Name = names[i - 1];
                phases[i].Targets.Add(mainTarget);
            }
            foreach (NPC tar in NPCs)
            {
                if (tar.ID == (ushort)Thief || tar.ID == (ushort)Drunkard || tar.ID == (ushort)Gambler)
                {
                    string name = (tar.ID == (ushort)Thief ? "Thief" : (tar.ID == (ushort)Drunkard ? "Drunkard" : (tar.ID == (ushort)Gambler ? "Gambler" : "")));
                    PhaseData tarPhase = new PhaseData(log.FightData.ToFightSpace(tar.FirstAwareLogTime) - 1000, Math.Min(log.FightData.ToFightSpace(tar.LastAwareLogTime) + 1000, fightDuration));
                    tarPhase.Targets.Add(tar);
                    tarPhase.OverrideTimes(log);
                    // override first then add Deimos so that it does not disturb the override process
                    tarPhase.Targets.Add(mainTarget);
                    tarPhase.Name = name;
                    phases.Add(tarPhase);
                }
            }
            List<AbstractBuffEvent> signets = GetFilteredList(log.CombatData, 38224, mainTarget, true);
            long sigStart = 0;
            long sigEnd = 0;
            int burstID = 1;
            for (int i = 0; i < signets.Count; i++)
            {
                AbstractBuffEvent signet = signets[i];
                if (signet is BuffApplyEvent)
                {
                    sigStart = Math.Max(signet.Time + 1, 0);
                }
                else
                {
                    sigEnd = Math.Min(signet.Time - 1, fightDuration);
                    PhaseData burstPhase = new PhaseData(sigStart, sigEnd)
                    {
                        Name = "Burst " + burstID++
                    };
                    burstPhase.Targets.Add(mainTarget);
                    phases.Add(burstPhase);
                }
            }
            phases.Sort((x, y) => x.Start.CompareTo(y.Start));
            phases.RemoveAll(x => x.Targets.Count == 0);
            return phases;
        }

        protected override List<ushort> GetFightNPCsIDs()
        {
            return new List<ushort>
            {
                (ushort)ParseEnum.EvtcNPCIDs.Deimos,
                (ushort)Thief,
                (ushort)Drunkard,
                (ushort)Gambler,
                (ushort)Saul,
                (ushort)GamblerClones,
                (ushort)GamblerReal,
                (ushort)Greed,
                (ushort)Pride,
                (ushort)Oil,
                (ushort)Tear,
                (ushort)Hands
            };
        }

        public override void ComputeNPCCombatReplayActors(NPC npc, ParsedLog log, CombatReplay replay)
        {
            int crStart = (int)replay.TimeOffsets.start;
            int crEnd = (int)replay.TimeOffsets.end;
            List<AbstractCastEvent> cls = npc.GetCastLogs(log, 0, log.FightData.FightDuration);
            switch (npc.ID)
            {
                case (ushort)ParseEnum.EvtcNPCIDs.Deimos:
                    List<AbstractCastEvent> mindCrush = cls.Where(x => x.SkillId == 37613).ToList();
                    foreach (AbstractCastEvent c in mindCrush)
                    {
                        int start = (int)c.Time;
                        int end = start + 5000;
                        replay.Actors.Add(new CircleDecoration(true, end, 180, (start, end), "rgba(255, 0, 0, 0.5)", new AgentConnector(npc)));
                        replay.Actors.Add(new CircleDecoration(false, 0, 180, (start, end), "rgba(255, 0, 0, 0.5)", new AgentConnector(npc)));
                        if (!log.FightData.IsCM)
                        {
                            replay.Actors.Add(new CircleDecoration(true, 0, 180, (start, end), "rgba(0, 0, 255, 0.3)", new PositionConnector(new Point3D(-8421.818f, 3091.72949f, -9.818082e8f, 216))));
                        }
                    }
                    List<AbstractCastEvent> annihilate = cls.Where(x => (x.SkillId == 38208) || (x.SkillId == 37929)).ToList();
                    foreach (AbstractCastEvent c in annihilate)
                    {
                        int start = (int)c.Time;
                        int delay = 1000;
                        int end = start + 2400;
                        int duration = 120;
                        Point3D facing = replay.Rotations.FirstOrDefault(x => x.Time >= start);
                        if (facing == null)
                        {
                            continue;
                        }
                        for (int i = 0; i < 6; i++)
                        {
                            replay.Actors.Add(new PieDecoration(true, 0, 900, (int)Math.Round(Math.Atan2(facing.Y, facing.X) * 180 / Math.PI + i * 360 / 10), 360 / 10, (start + delay + i * duration, end + i * duration), "rgba(255, 200, 0, 0.5)", new AgentConnector(npc)));
                            replay.Actors.Add(new PieDecoration(false, 0, 900, (int)Math.Round(Math.Atan2(facing.Y, facing.X) * 180 / Math.PI + i * 360 / 10), 360 / 10, (start + delay + i * duration, end + i * 120), "rgba(255, 150, 0, 0.5)", new AgentConnector(npc)));
                            if (i % 5 != 0)
                            {
                                replay.Actors.Add(new PieDecoration(true, 0, 900, (int)Math.Round(Math.Atan2(facing.Y, facing.X) * 180 / Math.PI - i * 360 / 10), 360 / 10, (start + delay + i * duration, end + i * 120), "rgba(255, 200, 0, 0.5)", new AgentConnector(npc)));
                                replay.Actors.Add(new PieDecoration(false, 0, 900, (int)Math.Round(Math.Atan2(facing.Y, facing.X) * 180 / Math.PI - i * 360 / 10), 360 / 10, (start + delay + i * duration, end + i * 120), "rgba(255, 150, 0, 0.5)", new AgentConnector(npc)));
                            }
                        }
                    }
                    List<AbstractBuffEvent> signets = GetFilteredList(log.CombatData, 38224, npc, true);
                    int sigStart = 0;
                    int sigEnd = 0;
                    foreach (AbstractBuffEvent signet in signets)
                    {
                        if (signet is BuffApplyEvent)
                        {
                            sigStart = (int)signet.Time;
                        }
                        else
                        {
                            sigEnd = (int)signet.Time;
                            replay.Actors.Add(new CircleDecoration(true, 0, 120, (sigStart, sigEnd), "rgba(0, 200, 200, 0.5)", new AgentConnector(npc)));
                        }
                    }
                    break;
                case (ushort)Gambler:
                case (ushort)Thief:
                case (ushort)Drunkard:
                    break;
                case (ushort)Saul:
                case (ushort)GamblerClones:
                case (ushort)GamblerReal:
                case (ushort)Greed:
                case (ushort)Pride:
                case (ushort)Tear:
                    break;
                case (ushort)Hands:
                    replay.Actors.Add(new CircleDecoration(true, 0, 90, (crStart, crEnd), "rgba(255, 0, 0, 0.2)", new AgentConnector(npc)));
                    break;
                case (ushort)Oil:
                    int oilDelay = 3000;
                    replay.Actors.Add(new CircleDecoration(true, crStart + oilDelay, 200, (crStart, crStart + oilDelay), "rgba(255,100, 0, 0.5)", new AgentConnector(npc)));
                    replay.Actors.Add(new CircleDecoration(true, 0, 200, (crStart + oilDelay, crEnd), "rgba(0, 0, 0, 0.5)", new AgentConnector(npc)));
                    break;
                default:
                    break;
            }

        }

        public override void ComputePlayerCombatReplayActors(Player p, ParsedLog log, CombatReplay replay)
        {
            // teleport zone
            List<AbstractBuffEvent> tpDeimos = GetFilteredList(log.CombatData, 37730, p, true);
            int tpStart = 0;
            foreach (AbstractBuffEvent c in tpDeimos)
            {
                if (c is BuffApplyEvent)
                {
                    tpStart = (int)c.Time;
                }
                else
                {
                    int tpEnd = (int)c.Time;
                    replay.Actors.Add(new CircleDecoration(true, 0, 180, (tpStart, tpEnd), "rgba(0, 150, 0, 0.3)", new AgentConnector(p)));
                    replay.Actors.Add(new CircleDecoration(true, tpEnd, 180, (tpStart, tpEnd), "rgba(0, 150, 0, 0.3)", new AgentConnector(p)));
                }
            }
        }

        public override int IsCM(CombatData combatData, AgentData agentData, FightData fightData)
        {
            NPC target = NPCs.Find(x => x.ID == (ushort)ParseEnum.EvtcNPCIDs.Deimos);
            if (target == null)
            {
                throw new InvalidOperationException("Target for CM detection not found");
            }
            return (target.GetHealth(combatData) > 40e6) ? 1 : 0;
        }
    }
}
