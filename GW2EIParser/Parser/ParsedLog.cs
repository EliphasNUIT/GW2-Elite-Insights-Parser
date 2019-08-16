﻿using System.Collections.Generic;
using System.Linq;
using GW2EIParser.Exceptions;
using GW2EIParser.Models;
using GW2EIParser.Logic;
using GW2EIParser.EIData;
using GW2EIParser.Parser.ParsedData;

namespace GW2EIParser.Parser
{
    public class ParsedLog
    {
        private readonly List<Mob> _auxMobs = new List<Mob>();

        public LogData LogData { get; }
        public FightData FightData { get; }
        public AgentData AgentData { get; }
        public SkillData SkillData { get; }
        public CombatData CombatData { get; }
        public List<Player> PlayerList { get; }
        public HashSet<AgentItem> PlayerAgents { get; }
        public bool IsBenchmarkMode => FightData.Logic.Mode == FightLogic.ParseMode.Golem;
        public Dictionary<string, List<Player>> PlayerListBySpec { get; }
        public DamageModifiersContainer DamageModifiers { get; }
        public BuffsContainer Buffs { get; }
        public bool CanCombatReplay => CombatData.HasMovementData;

        public readonly MechanicData MechanicData;
        public readonly Statistics Statistics;

        public ParsedLog(string buildVersion, FightData fightData, AgentData agentData, SkillData skillData, 
                List<CombatItem> combatItems, List<Player> playerList)
        {
            FightData = fightData;
            AgentData = agentData;
            SkillData = skillData;
            PlayerList = playerList;
            //
            PlayerListBySpec = playerList.GroupBy(x => x.Prof).ToDictionary(x => x.Key, x => x.ToList());
            PlayerAgents = new HashSet<AgentItem>(playerList.Select(x => x.AgentItem));
            CombatData = new CombatData(combatItems, FightData, AgentData, SkillData, playerList);
            LogData = new LogData(buildVersion, CombatData, combatItems);
            //
            UpdateFightData();
            //
            Buffs = new BuffsContainer(LogData.GW2Version);
            DamageModifiers = new DamageModifiersContainer(LogData.GW2Version);
            MechanicData = FightData.Logic.GetMechanicData();
            Statistics = new Statistics(CombatData, PlayerList, Buffs);      
        }

        private void UpdateFightData()
        {
            FightData.Logic.CheckSuccess(CombatData, AgentData, FightData, PlayerAgents);
            if (FightData.FightDuration <= 2200)
            {
                throw new TooShortException();
            }
            if (Properties.Settings.Default.SkipFailedTries && !FightData.Success)
            {
                throw new SkipException();
            }
            FightData.SetCM(CombatData, AgentData, FightData);
        }

        /// <summary>
        /// Find the corresponding actor, creates one otherwise
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public AbstractActor FindActor(AgentItem a)
        {
            AbstractActor res = PlayerList.FirstOrDefault(x => x.AgentItem == a);
            if (res == null)
            {
                foreach (Player p in PlayerList)
                {
                    Dictionary<string, Minions> minionsDict = p.GetMinions(this);
                    foreach (Minions minions in minionsDict.Values)
                    {
                        res = minions.FirstOrDefault(x => x.AgentItem == a);
                        if (res != null)
                        {
                            return res;
                        }
                    }
                }
                res = FightData.Logic.Targets.FirstOrDefault(x => x.AgentItem == a);
                if (res == null)
                {
                    res = FightData.Logic.TrashMobs.FirstOrDefault(x => x.AgentItem == a);
                    if (res == null)
                    {
                        res = _auxMobs.FirstOrDefault(x => x.AgentItem == a);
                        if (res == null)
                        {
                            _auxMobs.Add(new Mob(a));
                            res = _auxMobs.Last();
                        }
                    }
                }
            }
            return res;
        }
    }
}
