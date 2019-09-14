﻿using System.Collections.Generic;
using GW2EIParser.EIData;
using GW2EIParser.Parser;
using GW2EIParser.Parser.ParsedData;
using static GW2EIParser.Parser.ParseEnum.NPCIDs;

namespace GW2EIParser.Logic
{
    public class Artsariiv : FractalLogic
    {
        public Artsariiv(ushort triggerID) : base(triggerID)
        {
            MechanicList.AddRange(new List<Mechanic>
            {
            new PlayerBoonApplyMechanic(38880, "Corporeal Reassignment", new MechanicPlotlySetting("diamond-tall","rgb(255,0,0)"), "Skull","Exploding Skull mechanic application","Corporeal Reassignment",0),
            new HitOnPlayerMechanic(38977, "Vault", new MechanicPlotlySetting("triangle-down-open","rgb(255,200,0)"), "Vault","Vault from Big Adds", "Vault (Add)",0),
            new HitOnPlayerMechanic(39925, "Slam", new MechanicPlotlySetting("circle","rgb(255,140,0)"), "Slam","Slam (Vault) from Boss", "Vault (Arts)",0),
            new HitOnPlayerMechanic(39469, "Teleport Lunge", new MechanicPlotlySetting("star-triangle-down-open","rgb(255,140,0)"), "3 Jump","Triple Jump Mid->Edge", "Triple Jump",0),
            new HitOnPlayerMechanic(39035, "Astral Surge", new MechanicPlotlySetting("circle-open","rgb(255,200,0)"), "Floor Circle","Different sized spiraling circles", "1000 Circles",0),
            new HitOnPlayerMechanic(39029, "Red Marble", new MechanicPlotlySetting("circle","rgb(255,0,0)"), "Marble","Red KD Marble after Jump", "Red Marble",0),
            new HitOnPlayerMechanic(39863, "Red Marble", new MechanicPlotlySetting("circle","rgb(255,0,0)"), "Marble","Red KD Marble after Jump", "Red Marble",0),
            new PlayerBoonApplyMechanic(791, "Fear", new MechanicPlotlySetting("square-open","rgb(255,0,0)"), "Eye","Hit by the Overhead Eye Fear", "Eye (Fear)" ,0, (ba, log) => ba.AppliedDuration == 3000), //not triggered under stab, still get blinded/damaged, seperate tracking desired?
            new SpawnMechanic(17630, "Spark", new MechanicPlotlySetting("star","rgb(0,255,255)"),"Spark","Spawned a Spark (missed marble)", "Spark",0),
            });
            Extension = "arts";
            Icon = "https://wiki.guildwars2.com/images/b/b4/Artsariiv.jpg";
        }

        protected override CombatReplayMap GetCombatMapInternal()
        {
            return new CombatReplayMap("https://i.imgur.com/4wmuc8B.png",
                            (914, 914),
                            (8991, 112, 11731, 2812),
                            (-24576, -24576, 24576, 24576),
                            (11204, 4414, 13252, 6462));
        }

        protected override List<ushort> GetFightNPCsIDs()
        {
            return new List<ushort>
            {
                (ushort)ParseEnum.NPCIDs.Artsariiv,
                (ushort)TemporalAnomaly,
                (ushort)Spark,
                (ushort)Artsariiv1,
                (ushort)Artsariiv2,
                (ushort)Artsariiv3
            };
        }

        public override void CheckSuccess(CombatData combatData, AgentData agentData, FightData fightData, HashSet<AgentItem> playerAgents)
        {
            SetSuccessByBuffCount(combatData, fightData, playerAgents, NPCs.Find(x => x.ID == TriggerID), 762, 4);
        }
    }
}
