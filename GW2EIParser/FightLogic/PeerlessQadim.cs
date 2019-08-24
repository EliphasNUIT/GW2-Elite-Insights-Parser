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
    public class PeerlessQadim : RaidLogic
    {
        public PeerlessQadim(ushort triggerID) : base(triggerID)
        {
            MechanicList.AddRange(new List<Mechanic>()
            {
                new HitOnPlayerMechanic(56541, "Pylon Debris Field", new MechanicPlotlySetting("circle-open-dot","rgb(255,150,0)"), "P.Magma", "Hit by Pylon Magma", "Pylon Magma", 0),
                new HitOnPlayerMechanic(56020, "Energized Affliction", new MechanicPlotlySetting("circle-open","rgb(0,255,0)"), "E.Affliction", "Energized Affliction", "Energized Affliction", 0),
                new HitOnPlayerMechanic(56134, "Force of Retaliation", new MechanicPlotlySetting("circle-open","rgb(0,0,0)"), "Pushed", "Pushed by Shockwave", "Shockwave Push", 0, (de, log) => !de.To.HasBuff(log, 1122, de.Time)),
                new HitOnPlayerMechanic(56441, "Force of Havoc", new MechanicPlotlySetting("square-open","rgb(150,0,250)"), "P.Rectangle", "Hit by Purple Rectangle", "Purple Rectangle", 0),
                new HitOnPlayerMechanic(56145, "Chaos Called", new MechanicPlotlySetting("circle-x-open","rgb(150,0,250)"), "Pattern.H", "Hit by Energy on Pattern", "Pattern Energy Hit", 0),
                new HitOnPlayerMechanic(56527, "Rain of Chaos", new MechanicPlotlySetting("star-square","rgb(150,0,250)"), "Lightning.H", "Hit by Expanding Lightning", "Lightning Hit", 0),
                new HitOnPlayerMechanic(56656, "Brandstorm Lightning", new MechanicPlotlySetting("triangle","rgb(150,0,250)"), "S.Lightning.H", "Hit by Small Lightning", "Small Lightning Hit", 0),
                //new HitOnPlayerMechanic(56254, "Exponential Repercussion", new MechanicPlotlySetting("diamond-open","rgb(150,0,250)"), "Shield.H", "Hit by Energy Shield", "Shield Hit", 0),// to check
                new HitOnPlayerMechanic(56180, "Residual Impact", new MechanicPlotlySetting("circle-open","rgb(250,150,0)"), "Magma.F", "Hit by Magma Field", "Magma Field", 500),
                new HitOnPlayerMechanic(56378, "Residual Impact", new MechanicPlotlySetting("circle-open","rgb(250,150,0)",10), "S.Magma.F", "Hit by Small Magma Field", "Small Magma Field", 500),
                new HitOnPlayerMechanic(56616, "Battering Blitz", new MechanicPlotlySetting("bowtie","rgb(250,150,0)"), "Rush.H", "Hit by Qadim Rush", "Qadim Rush", 500),
                new HitOnPlayerMechanic(56332, "Caustic Chaos", new MechanicPlotlySetting("triangle-right","rgb(250,0,0)"), "A.Projectile.H", "Hit by Aimed Projectile", "Aimed Projectile", 0),
                new HitByEnemyMechanic(56598, "Shower of Chaos", new MechanicPlotlySetting("circle","rgb(0,0,0)"), "Orb.D", "Pylon Orb not caught", "Shower of Chaos", 1000),
                new PlayerBoonApplyMechanic(56510, "Fixated", new MechanicPlotlySetting("star","rgb(255,0,255)"), "Fixated", "Fixated", "Fixated", 0),
                new HitOnPlayerMechanic(56543, "Caustic Chaos", new MechanicPlotlySetting("triangle-right-open","rgb(250,0,0)"), "A.Projectile.E", "Hit by Aimed Projectile Explosion", "Aimed Projectile Explosion", 0),
                new PlayerBoonApplyMechanic(56118, "Sapping Surge", new MechanicPlotlySetting("y-down-open","rgb(250,0,50)"), "B.Tether", "25% damage reduction", "Bad Tether", 0),
            });
            Extension = "prlqadim";
            Icon = "https://wiki.guildwars2.com/images/8/8b/Mini_Qadim_the_Peerless.png";
        }

        protected override List<ushort> GetFightNPCsIDs()
        {
            return new List<ushort>()
            {
                (ushort)ParseEnum.EvtcNPCIDs.PeerlessQadim,
                (ushort)Pylon1,
                (ushort)Pylon2,
                (ushort)EntropicDistortion,
                (ushort)BigKillerTornado,
                (ushort)EnergyOrb,
            };
        }

        public override List<AbstractBuffEvent> SpecialBuffEventProcess(Dictionary<AgentItem, List<AbstractBuffEvent>> buffsByDst, Dictionary<AgentItem, List<AbstractBuffEvent>> buffsBySrc, Dictionary<long, List<AbstractBuffEvent>> buffsById, long offset, SkillData skillData)
        {
            List<AbstractBuffEvent> res = new List<AbstractBuffEvent>();
            if (buffsById.TryGetValue(56118, out var list))
            {
                Dictionary<AgentItem, List<AbstractBuffEvent>> sappingSurgeByDst = list.GroupBy(x => x.To).ToDictionary(x => x.Key, x => x.ToList());
                foreach (var pair in sappingSurgeByDst.Where(x => x.Value.Exists(y => y is BuffRemoveSingleEvent)))
                {
                    List<AbstractBuffEvent> sglRemovals = pair.Value.Where(x => x is BuffRemoveSingleEvent).ToList();
                    foreach (AbstractBuffEvent sglRemoval in sglRemovals)
                    {
                        AbstractBuffEvent ba = pair.Value.LastOrDefault(x => x is BuffApplyEvent && Math.Abs(x.Time - sglRemoval.Time) < 5);
                        if (ba != null)
                        {
                            res.Add(new BuffRemoveAllEvent(sglRemoval.By, pair.Key, ba.Time - 1, int.MaxValue, ba.BuffSkill, 1, int.MaxValue));
                            res.Add(new BuffRemoveManualEvent(sglRemoval.By, pair.Key, ba.Time - 1, int.MaxValue, ba.BuffSkill));
                        }
                    }
                }
            }
            return res;
        }

        public override List<PhaseData> GetPhases(ParsedLog log, bool requirePhases)
        {
            List<PhaseData> phases = GetInitialPhase(log);
            NPC mainTarget = NPCs.Find(x => x.ID == (ushort)ParseEnum.EvtcNPCIDs.PeerlessQadim);
            if (mainTarget == null)
            {
                throw new InvalidOperationException("Main target of the fight not found");
            }
            phases[0].Targets.Add(mainTarget);
            if (!requirePhases)
            {
                return phases;
            }
            List<long> phaseStarts = new List<long>();
            List<long> phaseEnds = new List<long>();
            //
            List<AbstractBuffEvent> magmaDrops = log.CombatData.GetBuffData(56475).Where(x => x is BuffApplyEvent).ToList();
            foreach (AbstractBuffEvent magmaDrop in magmaDrops)
            {
                if (phaseEnds.Count > 0)
                {
                    if (Math.Abs(phaseEnds.Last() - magmaDrop.Time) > 1000)
                    {
                        phaseEnds.Add(magmaDrop.Time);
                    }
                }
                else
                {
                    phaseEnds.Add(magmaDrop.Time);
                }
            }
            List<AbstractCastEvent> pushes = log.CombatData.GetCastDataById(56405);
            if (pushes.Count > 0)
            {
                AbstractCastEvent push = pushes[0];
                phaseStarts.Add(push.Time + push.ActualDuration);
                foreach (long magmaDrop in phaseEnds)
                {
                    push = pushes.FirstOrDefault(x => x.Time >= magmaDrop);
                    if (push == null)
                    {
                        break;
                    }
                    phaseStarts.Add(push.Time + push.ActualDuration);
                }
            }
            // rush to pylon
            phaseEnds.AddRange(log.CombatData.GetCastDataById(56616).Select(x => x.Time).ToList());
            phaseEnds.Add(log.FightData.FightDuration);
            // tp to middle after pylon destruction
            phaseStarts.AddRange(log.CombatData.GetCastDataById(56375).Select(x => x.Time + x.ActualDuration));
            if (phaseEnds.Count < phaseStarts.Count)
            {
                return phases;
            }
            for (int i = 0; i < phaseStarts.Count; i++)
            {
                PhaseData phase = new PhaseData(phaseStarts[i], phaseEnds[i])
                {
                    Name = "Phase " + (i + 1)
                };
                phase.Targets.Add(mainTarget);
                phases.Add(phase);
            }
            return phases;
        }

        protected override CombatReplayMap GetCombatMapInternal()
        {
            return new CombatReplayMap("https://i.imgur.com/PgkZMYE.png",
                            (6822, 6822),
                            (-968, 7480, 4226, 12676),
                            (-21504, -21504, 24576, 24576),
                            (33530, 34050, 35450, 35970));
        }

        public override void ComputeNPCCombatReplayActors(NPC npc, ParsedLog log, CombatReplay replay)
        {
            int crStart = (int)replay.TimeOffsets.start;
            int crEnd = (int)replay.TimeOffsets.end;
            List<AbstractCastEvent> cls = npc.GetCastLogs(log, 0, log.FightData.FightDuration);
            switch (npc.ID)
            {
                case (ushort)ParseEnum.EvtcNPCIDs.PeerlessQadim:
                    List<AbstractCastEvent> cataCycle = cls.Where(x => x.SkillId == 56329).ToList();

                    foreach (AbstractCastEvent c in cataCycle)
                    {
                        int magmaRadius = 850;
                        int start = (int)c.Time;
                        int end = start + c.ActualDuration;
                        Point3D pylonPosition = replay.PolledPositions.LastOrDefault(x => x.Time <= end);
                        replay.Actors.Add(new CircleDecoration(true, 0, magmaRadius, (start, end), "rgba(255, 220, 50, 0.15)", new PositionConnector(pylonPosition)));
                        replay.Actors.Add(new CircleDecoration(true, end, magmaRadius, (start, end), "rgba(255, 220, 50, 0.25)", new PositionConnector(pylonPosition)));
                        replay.Actors.Add(new CircleDecoration(true, 0, magmaRadius, (end, (int)log.FightData.FightDuration), "rgba(255, 220, 0, 0.5)", new PositionConnector(pylonPosition)));
                    }
                    break;
                case (ushort)EntropicDistortion:
                    //sapping surge, red tether
                    List<AbstractBuffEvent> sappingSurge = GetFilteredList(log.CombatData, 56118, npc, true);
                    int surgeStart = 0;
                    AbstractSingleActor source = null;
                    foreach (AbstractBuffEvent c in sappingSurge)
                    {
                        if (c is BuffApplyEvent)
                        {
                            NPC qadim = NPCs.Find(x => x.ID == (ushort)ParseEnum.EvtcNPCIDs.PeerlessQadim);
                            surgeStart = (int)c.Time;
                            source = (AbstractSingleActor)log.PlayerList.FirstOrDefault(x => x.AgentItem == c.By) ?? qadim;
                        }
                        else
                        {
                            int surgeEnd = (int)c.Time;
                            if (source != null)
                            {
                                replay.Actors.Add(new LineDecoration(0, (surgeStart, surgeEnd), "rgba(255, 0, 0, 0.3)", new AgentConnector(npc), new AgentConnector(source)));
                            }
                        }
                    }
                    break;
                case (ushort)BigKillerTornado:
                    replay.Actors.Add(new CircleDecoration(true, 0, 450, (crStart, crEnd), "rgba(255, 150, 0, 0.4)", new AgentConnector(npc)));
                    break;
                case (ushort)Pylon1:
                    break;
                case (ushort)Pylon2:
                    break;
                case (ushort)EnergyOrb:
                    break;
                default:
                    break;
            }

        }

        public override void ComputePlayerCombatReplayActors(Player p, ParsedLog log, CombatReplay replay)
        {
            // fixated
            List<AbstractBuffEvent> fixated = GetFilteredList(log.CombatData, 56510, p, true);
            int fixatedStart = 0;
            foreach (AbstractBuffEvent c in fixated)
            {
                if (c is BuffApplyEvent)
                {
                    fixatedStart = Math.Max((int)c.Time, 0);
                }
                else
                {
                    int fixatedEnd = (int)c.Time;
                    replay.Actors.Add(new CircleDecoration(true, 0, 120, (fixatedStart, fixatedEnd), "rgba(255, 80, 255, 0.3)", new AgentConnector(p)));
                }
            }
            // Magma drop
            List<AbstractBuffEvent> magmaDrop = GetFilteredList(log.CombatData, 56475, p, true);
            int magmaDropStart = 0;
            int magmaRadius = 420;
            int magmaOffset = 4000;
            foreach (AbstractBuffEvent c in magmaDrop)
            {
                if (c is BuffApplyEvent)
                {
                    magmaDropStart = (int)c.Time;
                }
                else
                {
                    int magmaDropEnd = (int)c.Time;
                    replay.Actors.Add(new CircleDecoration(true, 0, magmaRadius, (magmaDropStart, magmaDropEnd), "rgba(255, 150, 0, 0.15)", new AgentConnector(p)));
                    replay.Actors.Add(new CircleDecoration(true, magmaDropEnd, magmaRadius, (magmaDropStart, magmaDropEnd), "rgba(255, 150, 0, 0.25)", new AgentConnector(p)));
                    Point3D magmaNextPos = replay.PolledPositions.FirstOrDefault(x => x.Time >= magmaDropEnd);
                    Point3D magmaPrevPos = replay.PolledPositions.LastOrDefault(x => x.Time <= magmaDropEnd);
                    if (magmaNextPos != null || magmaPrevPos != null)
                    {
                        replay.Actors.Add(new CircleDecoration(true, 0, magmaRadius, (magmaDropEnd, magmaDropEnd + magmaOffset), "rgba(255, 220, 50, 0.15)", new InterpolatedPositionConnector(magmaPrevPos, magmaNextPos, magmaDropEnd)));
                        replay.Actors.Add(new CircleDecoration(true, magmaDropEnd + magmaOffset, magmaRadius, (magmaDropEnd, magmaDropEnd + magmaOffset), "rgba(255, 220, 50, 0.25)", new InterpolatedPositionConnector(magmaPrevPos, magmaNextPos, magmaDropEnd)));
                        replay.Actors.Add(new CircleDecoration(true, 0, magmaRadius, (magmaDropEnd + magmaOffset, (int)log.FightData.FightDuration), "rgba(255, 220, 50, 0.5)", new InterpolatedPositionConnector(magmaPrevPos, magmaNextPos, magmaDropEnd)));
                    }
                }

            }
            //sapping surge, bad red tether
            List<AbstractBuffEvent> sappingSurge = GetFilteredList(log.CombatData, 56118, p, true);
            int surgeStart = 0;
            AbstractSingleActor source = null;
            foreach (AbstractBuffEvent c in sappingSurge)
            {
                if (c is BuffApplyEvent)
                {
                    NPC qadim = NPCs.Find(x => x.ID == (ushort)ParseEnum.EvtcNPCIDs.PeerlessQadim);
                    surgeStart = (int)c.Time;
                    source = (AbstractSingleActor)log.PlayerList.FirstOrDefault(x => x.AgentItem == c.By) ?? qadim;
                }
                else
                {
                    int surgeEnd = (int)c.Time;
                    if (source != null)
                    {
                        replay.Actors.Add(new LineDecoration(0, (surgeStart, surgeEnd), "rgba(255, 0, 0, 0.4)", new AgentConnector(p), new AgentConnector(source)));
                    }
                }
            }
            // kinetic abundance, good (blue) tether
            List<AbstractBuffEvent> kineticAbundance = GetFilteredList(log.CombatData, 56609, p, true);
            int kinStart = 0;
            AbstractSingleActor kinSource = null;
            foreach (AbstractBuffEvent c in kineticAbundance)
            {
                if (c is BuffApplyEvent)
                {
                    kinStart = (int)c.Time;
                    //kinSource = log.PlayerList.FirstOrDefault(x => x.AgentItem == c.By);
                    kinSource = (AbstractSingleActor)log.PlayerList.FirstOrDefault(x => x.AgentItem == c.By) ?? NPCs.FirstOrDefault(x => x.AgentItem == c.By);
                }
                else
                {
                    int kinEnd = (int)c.Time;
                    if (kinSource != null)
                    {
                        replay.Actors.Add(new LineDecoration(0, (kinStart, kinEnd), "rgba(0, 0, 255, 0.4)", new AgentConnector(p), new AgentConnector(kinSource)));
                    }
                }
            }
        }

        public override int IsCM(CombatData combatData, AgentData agentData, FightData fightData)
        {
            return HPBasedCM(combatData, agentData, (ushort)ParseEnum.EvtcNPCIDs.PeerlessQadim, 48e6);
        }
    }
}
