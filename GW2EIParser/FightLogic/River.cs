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
    public class River : RaidLogic
    {
        public River(ushort triggerID) : base(triggerID)
        {
            MechanicList.AddRange(new List<Mechanic>
            {
                new HitOnPlayerMechanic(48272, "Bombshell", new MechanicPlotlySetting("circle","rgb(255,125,0)"),"Bomb Hit", "Hit by Hollowed Bomber Exlosion", "Hit by Bomb", 0 ),
                new HitOnPlayerMechanic(47258, "Timed Bomb", new MechanicPlotlySetting("square","rgb(255,125,0)"),"Stun Bomb", "Stunned by Mini Bomb", "Stun Bomb", 0, (de, log) => !de.To.HasBuff(log, 1122, de.Time)),
            }
            );
            GenericFallBackMethod = FallBackMethod.None;
            Extension = "river";
            Icon = "https://wiki.guildwars2.com/images/thumb/7/7b/Gold_River_of_Souls_Trophy.jpg/220px-Gold_River_of_Souls_Trophy.jpg";
        }

        protected override CombatReplayMap GetCombatMapInternal()
        {
            return new CombatReplayMap("https://i.imgur.com/YBtiFnH.png",
                            (4145, 1603),
                            (-12201, -4866, 7742, 2851),
                            (-21504, -12288, 24576, 12288),
                            (19072, 15484, 20992, 16508));
        }

        protected override List<ushort> GetFightNPCsIDs()
        {
            return new List<ushort>
            {
                (ushort)Desmina,
                (ushort)Enervator,
                (ushort)HollowedBomber,
                (ushort)RiverOfSouls,
                (ushort)SpiritHorde1,
                (ushort)SpiritHorde2,
                (ushort)SpiritHorde3
            };
        }

        protected override HashSet<ushort> GetFriendlyNPCsIDs()
        {
            return new HashSet<ushort>
            {
                (ushort)Desmina,
            };
        }

        public override void CheckSuccess(CombatData combatData, AgentData agentData, FightData fightData, HashSet<AgentItem> playerAgents)
        {
            base.CheckSuccess(combatData, agentData, fightData, playerAgents);
            if (!fightData.Success)
            {
                NPC desmina = NPCs.Find(x => x.ID == (ushort)ParseEnum.EvtcNPCIDs.Desmina);
                if (desmina == null)
                {
                    throw new InvalidOperationException("Main target of the fight not found");
                }
                ExitCombatEvent ooc = combatData.GetExitCombatEvents(desmina.AgentItem).LastOrDefault();
                if (ooc != null)
                {
                    long time = 0;
                    foreach (NPC mob in NPCs.Where(x => x.ID == (ushort)SpiritHorde3))
                    {
                        DespawnEvent dspwnHorde = combatData.GetDespawnEvents(mob.AgentItem).LastOrDefault();
                        if (dspwnHorde != null)
                        {
                            time = Math.Max(dspwnHorde.Time, time);
                        }
                    }
                    DespawnEvent dspwn = combatData.GetDespawnEvents(desmina.AgentItem).LastOrDefault();
                    if (time != 0 && dspwn == null && time <= fightData.ToFightSpace(desmina.LastAwareLogTime))
                    {
                        fightData.SetSuccess(true, fightData.ToLogSpace(time));
                    }
                }
            }
        }

        public override void SpecialParse(FightData fightData, AgentData agentData, List<CombatItem> combatData)
        {
            // The walls spawn at the start of the encounter, we fix it by overriding their first aware to the first velocity change event
            List<AgentItem> riverOfSouls = agentData.GetAgentsByID((ushort)RiverOfSouls);
            bool sortCombatList = false;
            foreach (AgentItem riverOfSoul in riverOfSouls)
            {
                CombatItem firstMovement = combatData.FirstOrDefault(x => x.IsStateChange == ParseEnum.EvtcStateChange.Velocity && x.SrcInstid == riverOfSoul.InstID && x.LogTime <= riverOfSoul.LastAwareLogTime);
                if (firstMovement != null)
                {
                    // update start
                    riverOfSoul.FirstAwareLogTime = firstMovement.LogTime - 10;
                    foreach (CombatItem c in combatData)
                    {
                        if (c.SrcInstid == riverOfSoul.InstID && c.LogTime < riverOfSoul.FirstAwareLogTime && (c.IsStateChange == ParseEnum.EvtcStateChange.Position || c.IsStateChange == ParseEnum.EvtcStateChange.Rotation))
                        {
                            sortCombatList = true;
                            c.OverrideTime(riverOfSoul.FirstAwareLogTime);
                        }
                    }
                }
                else
                {
                    // otherwise remove the agent from the pool
                    agentData.RemoveAgent(riverOfSoul);
                }
            }
            // make sure the list is still sorted by time after overrides
            if (sortCombatList)
            {
                combatData.Sort((x, y) => x.LogTime.CompareTo(y.LogTime));
            }
            ComputeFightNPCs(agentData, combatData);
        }

        public override void ComputePlayerCombatReplayActors(Player p, ParsedLog log, CombatReplay replay)
        {
            // TODO bombs dual following circle actor (one growing, other static) + dual static circle actor (one growing with min radius the final radius of the previous, other static). Missing buff id
        }

        public override void ComputeNPCCombatReplayActors(NPC npc, ParsedLog log, CombatReplay replay)
        {
            NPC desmina = NPCs.Find(x => x.ID == (ushort)ParseEnum.EvtcNPCIDs.Desmina);
            if (desmina == null)
            {
                throw new InvalidOperationException("Main target of the fight not found");
            }
            int start = (int)replay.TimeOffsets.start;
            int end = (int)replay.TimeOffsets.end;
            switch (npc.ID)
            {
                case (ushort)HollowedBomber:
                    var bomberman = npc.GetCastLogs(log, 0, log.FightData.FightDuration).Where(x => x.SkillId == 48272).ToList();
                    foreach (AbstractCastEvent bomb in bomberman)
                    {
                        int startCast = (int)bomb.Time;
                        int endCast = startCast + bomb.ActualDuration;
                        int expectedEnd = Math.Max(startCast + bomb.ExpectedDuration, endCast);
                        replay.Actors.Add(new CircleDecoration(true, 0, 480, (startCast, endCast), "rgba(180,250,0,0.3)", new AgentConnector(npc)));
                        replay.Actors.Add(new CircleDecoration(true, expectedEnd, 480, (startCast, endCast), "rgba(180,250,0,0.3)", new AgentConnector(npc)));
                    }
                    break;
                case (ushort)RiverOfSouls:
                    if (replay.Rotations.Count > 0)
                    {
                        replay.Actors.Add(new FacingRectangleDecoration((start, end), new AgentConnector(npc), replay.PolledRotations, 160, 390, "rgba(255,100,0,0.5)"));
                    }
                    break;
                case (ushort)Enervator:
                // TODO Line actor between desmina and enervator. Missing skillID
                case (ushort)SpiritHorde1:
                case (ushort)SpiritHorde2:
                case (ushort)SpiritHorde3:
                    break;
                default:
                    break;
            }

        }

        public override string GetFightName()
        {
            return "River of Souls";
        }
    }
}
