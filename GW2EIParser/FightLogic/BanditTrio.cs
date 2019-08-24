using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIParser.EIData;
using GW2EIParser.Parser;
using GW2EIParser.Parser.ParsedData;
using GW2EIParser.Parser.ParsedData.CombatEvents;
using static GW2EIParser.Parser.ParseEnum.EvtcNPCIDs;

namespace GW2EIParser.Logic
{
    public class BanditTrio : RaidLogic
    {
        public BanditTrio(ushort triggerID) : base(triggerID)
        {
            MechanicList.AddRange(new List<Mechanic>()
            {
            new PlayerBoonApplyMechanic(34108, "Shell-Shocked", new MechanicPlotlySetting("circle-open","rgb(0,128,0)"), "Launchd","Shell-Shocked (Launched from pad)", "Shell-Shocked",0),
            new HitOnPlayerMechanic(34448, "Overhead Smash", new MechanicPlotlySetting("triangle-left","rgb(200,140,0)"), "Smash","Overhead Smash (CC Attack Berg)", "CC Smash",0),
            new HitOnPlayerMechanic(34383, "Hail of Bullets", new MechanicPlotlySetting("triangle-right-open","rgb(255,0,0)"), "Zane Cone","Hail of Bullets (Zane Cone Shot)", "Hail of Bullets",0),
            new HitOnPlayerMechanic(34344, "Fiery Vortex", new MechanicPlotlySetting("circle-open","rgb(255,200,0)"), "Tornado","Fiery Vortex (Tornado)", "Tornado",250),
            });
            Extension = "trio";
            GenericFallBackMethod = FallBackMethod.None;
            Icon = "https://i.imgur.com/UZZQUdf.png";
        }

        protected override List<ushort> GetSuccessCheckIds()
        {
            return new List<ushort>
            {
                (ushort)ParseEnum.EvtcNPCIDs.Narella
            };
        }

        protected override List<ushort> GetFightNPCsIDs()
        {
            return new List<ushort>
            {
                (ushort)ParseEnum.EvtcNPCIDs.Berg,
                (ushort)ParseEnum.EvtcNPCIDs.Zane,
                (ushort)ParseEnum.EvtcNPCIDs.Narella,
                (ushort)BanditSaboteur,
                (ushort)Warg,
                (ushort)CagedWarg,
                (ushort)BanditAssassin,
                (ushort)BanditSapperTrio ,
                (ushort)BanditDeathsayer,
                (ushort)BanditBrawler,
                (ushort)BanditBattlemage,
                (ushort)BanditCleric,
                (ushort)BanditBombardier,
                (ushort)BanditSniper,
                (ushort)NarellaTornado,
                (ushort)OilSlick,
                (ushort)Prisoner1,
                (ushort)Prisoner2
            };
        }

        protected override CombatReplayMap GetCombatMapInternal()
        {
            return new CombatReplayMap("https://i.imgur.com/cVuaOc5.png",
                            (2494, 2277),
                            (-2900, -12251, 2561, -7265),
                            (-12288, -27648, 12288, 27648),
                            (2688, 11906, 3712, 14210));
        }

        public override void CheckSuccess(CombatData combatData, AgentData agentData, FightData fightData, HashSet<AgentItem> playerAgents)
        {
            base.CheckSuccess(combatData, agentData, fightData, playerAgents);
            if (!fightData.Success)
            {
                List<AgentItem> prisoners = agentData.GetAgentsByID((ushort)Prisoner2);
                List<DeadEvent> prisonerDeaths = new List<DeadEvent>();
                foreach (AgentItem prisoner in prisoners)
                {
                    prisonerDeaths.AddRange(combatData.GetDeadEvents(prisoner));
                }
                if (prisonerDeaths.Count == 0)
                {
                    SetSuccessByCombatExit(new HashSet<ushort>(GetSuccessCheckIds()), combatData, fightData, playerAgents);
                }
            }
        }

        public static void SetPhasePerTarget(NPC target, List<PhaseData> phases, ParsedLog log)
        {
            long fightDuration = log.FightData.FightDuration;
            EnterCombatEvent phaseStart = log.CombatData.GetEnterCombatEvents(target.AgentItem).LastOrDefault();
            if (phaseStart != null)
            {
                long start = phaseStart.Time;
                DeadEvent phaseEnd = log.CombatData.GetDeadEvents(target.AgentItem).LastOrDefault();
                long end = fightDuration;
                if (phaseEnd != null)
                {
                    end = phaseEnd.Time;
                }
                PhaseData phase = new PhaseData(start, Math.Min(end, log.FightData.FightDuration));
                phase.Targets.Add(target);
                phases.Add(phase);
            }
        }

        protected override HashSet<ushort> GetUniqueTargetIDs()
        {
            return new HashSet<ushort>
            {
                (ushort)ParseEnum.EvtcNPCIDs.Berg,
                (ushort)ParseEnum.EvtcNPCIDs.Zane,
                (ushort)ParseEnum.EvtcNPCIDs.Narella
            };
        }

        public override List<PhaseData> GetPhases(ParsedLog log, bool requirePhases)
        {
            List<PhaseData> phases = GetInitialPhase(log);
            NPC berg = NPCs.Find(x => x.ID == (ushort)ParseEnum.EvtcNPCIDs.Berg);
            if (berg == null)
            {
                throw new InvalidOperationException("Berg not found");
            }
            NPC zane = NPCs.Find(x => x.ID == (ushort)ParseEnum.EvtcNPCIDs.Zane);
            if (zane == null)
            {
                throw new InvalidOperationException("Zane");
            }
            NPC narella = NPCs.Find(x => x.ID == (ushort)ParseEnum.EvtcNPCIDs.Narella);
            if (narella == null)
            {
                throw new InvalidOperationException("Narella");
            }
            phases[0].Targets.AddRange(NPCs);
            if (!requirePhases)
            {
                return phases;
            }
            foreach (NPC target in NPCs)
            {
                SetPhasePerTarget(target, phases, log);
            }
            string[] phaseNames = { "Berg", "Zane", "Narella" };
            for (int i = 1; i < phases.Count; i++)
            {
                phases[i].Name = phaseNames[i - 1];
            }
            return phases;
        }

        public override string GetFightName()
        {
            return "Bandit Trio";
        }

        public override void ComputeNPCCombatReplayActors(NPC target, ParsedLog log, CombatReplay replay)
        {
            List<AbstractCastEvent> cls = target.GetCastLogs(log, 0, log.FightData.FightDuration);
            switch (target.ID)
            {
                case (ushort)ParseEnum.EvtcNPCIDs.Berg:
                    break;
                case (ushort)ParseEnum.EvtcNPCIDs.Zane:
                    List<AbstractCastEvent> bulletHail = cls.Where(x => x.SkillId == 34383).ToList();
                    foreach (AbstractCastEvent c in bulletHail)
                    {
                        int start = (int)c.Time;
                        int firstConeStart = start;
                        int secondConeStart = start + 800;
                        int thirdConeStart = start + 1600;
                        int firstConeEnd = firstConeStart + 400;
                        int secondConeEnd = secondConeStart + 400;
                        int thirdConeEnd = thirdConeStart + 400;
                        int radius = 1500;
                        Point3D facing = replay.Rotations.LastOrDefault(x => x.Time <= start);
                        if (facing != null)
                        {
                            replay.Actors.Add(new PieDecoration(true, 0, radius, facing, 28, (firstConeStart, firstConeEnd), "rgba(255,200,0,0.3)", new AgentConnector(target)));
                            replay.Actors.Add(new PieDecoration(true, 0, radius, facing, 54, (secondConeStart, secondConeEnd), "rgba(255,200,0,0.3)", new AgentConnector(target)));
                            replay.Actors.Add(new PieDecoration(true, 0, radius, facing, 81, (thirdConeStart, thirdConeEnd), "rgba(255,200,0,0.3)", new AgentConnector(target)));
                        }
                    }
                    break;

                case (ushort)ParseEnum.EvtcNPCIDs.Narella:
                    break;
                default:
                    break;
            }
        }
    }
}
