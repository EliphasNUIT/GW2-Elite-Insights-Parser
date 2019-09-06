using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIParser.EIData;
using GW2EIParser.Exceptions;
using GW2EIParser.Logic;
using GW2EIParser.Parser.ParsedData;

namespace GW2EIParser.Parser
{
    public class ParsedLog
    {
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

        private Dictionary<AgentItem, AbstractSingleActor> _agentToActorDictionary;

        public MechanicData MechanicData { get; }

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
            //
            Buffs = new BuffsContainer(LogData.GW2Version);
            DamageModifiers = new DamageModifiersContainer(LogData.GW2Version);
            MechanicData = FightData.Logic.GetMechanicData();
            combatItems.Clear();
        }

        private void AddToDictionary(AbstractSingleActor actor)
        {
            _agentToActorDictionary[actor.AgentItem] = actor;
            foreach (Minions minions in actor.GetMinions(this).Values)
            {
                foreach (NPC npc in minions.MinionList)
                {
                    AddToDictionary(npc);
                }
            }
        }

        private void InitActorDictionaries()
        {
            if (_agentToActorDictionary == null)
            {
                _agentToActorDictionary = new Dictionary<AgentItem, AbstractSingleActor>();
                foreach (Player p in PlayerList)
                {
                    AddToDictionary(p);
                }
                foreach (NPC npc in FightData.Logic.NPCs)
                {
                    AddToDictionary(npc);
                }
            }
        }

        /// <summary>
        /// Find the corresponding actor
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public AbstractSingleActor FindActor(AgentItem a, bool searchPlayers, bool throwOnFail = true)
        {
            if (a == null || a == GeneralHelper.UnknownAgent || a.Type == AgentItem.AgentType.EnemyPlayer || (!searchPlayers && a.Type == AgentItem.AgentType.Player))
            {
                return null;
            }
            InitActorDictionaries();
            if (!_agentToActorDictionary.TryGetValue(a, out AbstractSingleActor actor))
            {
                if (throwOnFail)
                {
                    throw new InvalidOperationException("Requested actor with id " + a.ID + " and name " + a.Name + " is missing");
                }
                else
                {
                    return null;
                }
            }
            return actor;          
        }
    }
}
