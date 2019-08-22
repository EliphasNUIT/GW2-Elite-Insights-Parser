using GW2EIParser.Parser;
using GW2EIParser.Parser.ParsedData;
using GW2EIParser.Parser.ParsedData.CombatEvents;
using System;
using System.Collections.Generic;
using System.Linq;
using static GW2EIParser.Models.Statistics;

namespace GW2EIParser.EIData
{
    public abstract class AbstractMasterActor : AbstractSingleActor
    {
        // Minions
        private Dictionary<string, Minions> _minions;
        // Replay
        protected CombatReplay CombatReplay;

        protected AbstractMasterActor(AgentItem agent) : base(agent)
        {

        }
        protected override void SetDamageLogs(ParsedLog log)
        {
            AddDamageLogs(log.CombatData.GetDamageData(AgentItem));
            Dictionary<string, Minions> minionsList = GetMinions(log);
            foreach (Minions mins in minionsList.Values)
            {
                DamageLogs.AddRange(mins.GetDamageLogs(null, log, 0, log.FightData.FightDuration));
            }
            DamageLogs.Sort((x, y) => x.Time.CompareTo(y.Time));
        }
        // Minions
        public Dictionary<string, Minions> GetMinions(ParsedLog log)
        {
            if (_minions == null)
            {
                _minions = new Dictionary<string, Minions>();
                List<AgentItem> combatMinion = log.AgentData.GetAgentByType(AgentItem.AgentType.NPC).Where(x => x.MasterAgent == AgentItem).ToList();
                Dictionary<string, Minions> auxMinions = new Dictionary<string, Minions>();
                foreach (AgentItem agent in combatMinion)
                {
                    string id = agent.Name;
                    if (auxMinions.TryGetValue(id, out Minions values))
                    {
                        values.AddMinion(new Minion(agent));
                    } else
                    {
                        auxMinions[id] = new Minions(id.GetHashCode(), new Minion(agent));
                    }
                }
                foreach (KeyValuePair<string, Minions> pair in auxMinions)
                {
                    if (pair.Value.GetDamageLogs(null, log, 0, log.FightData.FightDuration).Count > 0 || pair.Value.GetCastLogs(log, 0, log.FightData.FightDuration).Count > 0)
                    {
                        _minions[pair.Key] = pair.Value;
                    }
                }
            }
            return _minions;
        }


        // Combat Replay
        protected void SetMovements(ParsedLog log)
        {
            foreach (AbstractMovementEvent movementEvent in log.CombatData.GetMovementData(AgentItem))
            {
                movementEvent.AddPoint3D(CombatReplay);
            }
        }
        public List<int> GetCombatReplayTimes(ParsedLog log)
        {
            if (CombatReplay == null)
            {
                InitCombatReplay(log);
            }
            return CombatReplay.Times;
        }

        public List<Point3D> GetCombatReplayPolledPositions(ParsedLog log)
        {
            if (CombatReplay == null)
            {
                InitCombatReplay(log);
            }
            return CombatReplay.PolledPositions;
        }

        protected abstract void InitCombatReplay(ParsedLog log);

        protected void TrimCombatReplay(ParsedLog log)
        {
            DespawnEvent despawnCheck = log.CombatData.GetDespawnEvents(AgentItem).LastOrDefault();
            SpawnEvent spawnCheck = log.CombatData.GetSpawnEvents(AgentItem).LastOrDefault();
            DeadEvent deathCheck = log.CombatData.GetDeadEvents(AgentItem).LastOrDefault();
            if (deathCheck != null)
            {
                CombatReplay.Trim(log.FightData.ToFightSpace(AgentItem.FirstAwareLogTime), deathCheck.Time);
            }
            else if (despawnCheck != null && (spawnCheck == null || spawnCheck.Time < despawnCheck.Time))
            {
                CombatReplay.Trim(log.FightData.ToFightSpace(AgentItem.FirstAwareLogTime), despawnCheck.Time);
            }
            else
            {
                CombatReplay.Trim(log.FightData.ToFightSpace(AgentItem.FirstAwareLogTime), log.FightData.ToFightSpace(AgentItem.LastAwareLogTime));
            }
        }

        public List<GenericActor> GetCombatReplayActors(ParsedLog log)
        {
            if (!log.CanCombatReplay || IsFakeActor)
            {
                // no combat replay support on fight
                return null;
            }
            if (CombatReplay == null)
            {
                InitCombatReplay(log);
            }
            if (CombatReplay.NoActors)
            {
                CombatReplay.NoActors = false;
                InitAdditionalCombatReplayData(log);
            }
            return CombatReplay.Actors;
        }

        public int GetCombatReplayID(ParsedLog log)
        {
            if (CombatReplay == null)
            {
                InitCombatReplay(log);
            }
            return (InstID + "_" + CombatReplay.TimeOffsets.start + "_" + CombatReplay.TimeOffsets.end).GetHashCode();
        }
        protected abstract void InitAdditionalCombatReplayData(ParsedLog log);


        public abstract class AbstractMasterActorSerializable
        {
            public string Img { get; set; }
            public string Type { get; set; }
            public int ID { get; set; }
            public double[] Positions { get; set; }
        }

        public abstract AbstractMasterActorSerializable GetCombatReplayJSON(CombatReplayMap map, ParsedLog log);
    }
}