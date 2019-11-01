﻿using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIParser.EIData;
using GW2EIParser.Parser;
using GW2EIParser.Parser.ParsedData;
using GW2EIParser.Parser.ParsedData.CombatEvents;
using static GW2EIParser.Parser.ParseEnum.NPCIDs;

namespace GW2EIParser.Logic
{
    public class Sabir : RaidLogic
    {
        public Sabir(ushort triggerID) : base(triggerID)
        {
            MechanicList.AddRange(new List<Mechanic>()
            {
                new SkillOnPlayerMechanic(56202, "Dire Drafts", new MechanicPlotlySetting("circle","rgb(255,120,0)"), "B.Tornado", "Hit by big tornado", "Big Tornado Hit", 500, (de, log) => de.HasDowned || de.HasKilled),
                new SkillOnPlayerMechanic(56643, "Unbridled Tempest", new MechanicPlotlySetting("hexagon","rgb(255,0,150)"), "Shockwave", "Hit by Shockwave", "Shockwave Hit", 0, (de, log) => de.HasDowned || de.HasKilled),
                new SkillOnPlayerMechanic(56372, "Fury of the Storm", new MechanicPlotlySetting("circle","rgb(50,0,200)"), "Arena AoE", "Hit by Arena wide AoE", "Arena AoE hit", 0, (de, log) => de.HasDowned || de.HasKilled ),
                new HitOnPlayerMechanic(56055, "Dynamic Deterrent", new MechanicPlotlySetting("y-up-open","rgb(255,0,150)"), "Pushed", "Pushed by rotating breakbar", "Pushed", 0, (de, log) => !de.To.HasBuff(log, 1122, de.Time)),
                new EnemyCastStartMechanic(56349, "Regenerative Breakbar", new MechanicPlotlySetting("diamond-wide","rgb(255,0,255)"), "Reg.Breakbar","Regenerating Breakbar", "Regenerative Breakbar", 0),
                new EnemyBuffRemoveMechanic(56100, "Regenerative Breakbar Broken", new MechanicPlotlySetting("diamond-wide","rgb(0,160,150)"), "Reg.Breakbar Brkn", "Regenerative Breakbar Broken", "Regenerative Breakbar Broken", 2000),
                new EnemyBuffApplyMechanic(56172, "Rotating Breakbar", new MechanicPlotlySetting("diamond-tall","rgb(255,0,255)"), "Rot.Breakbar","Rotating Breakbar", "Rotating Breakbar", 0),
                new EnemyBuffRemoveMechanic(56172, "Rotating Breakbar Broken", new MechanicPlotlySetting("diamond-tall","rgb(0,160,150)"), "Rot.Breakbar Brkn","Rotating Breakbar Broken", "Rotating Breakbar Broken", 0),
            });
            // rotating cc 56403
            // interesting stuff 56372 (big AoE?)
            Extension = "sabir";
            Icon = "https://wiki.guildwars2.com/images/f/fc/Mini_Air_Djinn.png";
        }

        protected override List<ushort> GetFightNPCsIDs()
        {
            return new List<ushort>()
            {
                (ushort)ParseEnum.NPCIDs.Sabir,
                (ushort)ParalyzingWisp,
                (ushort)VoltaicWisp,
                (ushort)SmallKillerTornado,
                (ushort)SmallJumpyTornado,
                (ushort)BigKillerTornado
            };
        }

        public override List<AbstractDamageEvent> SpecialDamageEventProcess(Dictionary<AgentItem, List<AbstractDamageEvent>> damageBySrc, Dictionary<AgentItem, List<AbstractDamageEvent>> damageByDst, Dictionary<long, List<AbstractDamageEvent>> damageById, long offset, SkillData skillData)
        {
            NegateDamageAgainstBarrier(NPCs.Select(x => x.AgentItem).ToList(), damageByDst);
            return new List<AbstractDamageEvent>();
        }

        public override List<PhaseData> GetPhases(ParsedLog log, bool requirePhases)
        {
            List<PhaseData> phases = GetInitialPhase(log);
            NPC mainTarget = NPCs.Find(x => x.ID == (ushort)ParseEnum.NPCIDs.Sabir);
            if (mainTarget == null)
            {
                throw new InvalidOperationException("Main target of the fight not found");
            }
            phases[0].Targets.Add(mainTarget);
            if (!requirePhases)
            {
                return phases;
            }
            List<AbstractCastEvent> cls = mainTarget.GetCastLogs(log, 0, log.FightData.FightDuration);
            var wallopingWinds = cls.Where(x => x.SkillId == 56094).ToList();
            long start = 0, end = 0;
            for (int i = 0; i < wallopingWinds.Count; i++)
            {
                AbstractCastEvent wW = wallopingWinds[i];
                end = wW.Time;
                var phase = new PhaseData(start, end)
                {
                    Name = "Phase " + (i + 1)
                };
                phase.Targets.Add(mainTarget);
                phases.Add(phase);
                AbstractCastEvent nextAttack = cls.FirstOrDefault(x => x.Time >= end + wW.ActualDuration && (x.SkillId == 56620 || x.SkillId == 56629 || x.SkillId == 56307));
                if (nextAttack == null)
                {
                    break;
                }
                start = nextAttack.Time;
                if (i == wallopingWinds.Count - 1)
                {
                    phase = new PhaseData(start, log.FightData.FightDuration)
                    {
                        Name = "Phase " + (i + 2)
                    };
                    phase.Targets.Add(mainTarget);
                    phases.Add(phase);
                }
            }

            return phases;
        }

        protected override CombatReplayMap GetCombatMapInternal()
        {
            return new CombatReplayMap("https://i.imgur.com/zs9yPuG.png",
                            (4365, 3972),
                            (-14122, 142, -9199, 4640),
                            (-21504, -21504, 24576, 24576),
                            (33530, 34050, 35450, 35970));
        }

        public override void ComputeNPCCombatReplayActors(NPC npc, ParsedLog log, CombatReplay replay)
        {
            int crStart = (int)replay.TimeOffsets.start;
            int crEnd = (int)replay.TimeOffsets.end;
            switch (npc.ID)
            {
                case (ushort)BigKillerTornado:
                    replay.Decorations.Add(new CircleDecoration(true, 0, 420, (crStart, crEnd), "rgba(255, 150, 0, 0.4)", new AgentConnector(npc)));
                    break;
                case (ushort)SmallKillerTornado:
                    replay.Decorations.Add(new CircleDecoration(true, 0, 120, (crStart, crEnd), "rgba(255, 150, 0, 0.4)", new AgentConnector(npc)));
                    break;
                case (ushort)SmallJumpyTornado:
                case (ushort)ParalyzingWisp:
                case (ushort)VoltaicWisp:
                    break;
                default:
                    break;

            }
        }

        public override int IsCM(CombatData combatData, AgentData agentData, FightData fightData)
        {
            return HPBasedCM(combatData, (ushort)ParseEnum.NPCIDs.Deimos, 32e6);
        }
    }
}

