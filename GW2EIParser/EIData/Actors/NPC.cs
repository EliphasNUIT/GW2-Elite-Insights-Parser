using System.Collections.Generic;
using System.Linq;
using GW2EIParser.Parser;
using GW2EIParser.Parser.ParsedData;
using GW2EIParser.Parser.ParsedData.CombatEvents;
using static GW2EIParser.Builders.JsonModels.JsonCombatReplayActors;

namespace GW2EIParser.EIData
{
    public class NPC : AbstractSingleActor
    {
        private int _health = -1;
        // Constructors
        public NPC(AgentItem agent, bool friendly) : base(agent)
        {
            Friendly = friendly;
        }

        public void OverrideName(string name)
        {
            Character = name;
        }
        // Health
        public int GetHealth(CombatData combatData)
        {
            if (_health == -1)
            {
                List<MaxHealthUpdateEvent> maxHpUpdates = combatData.GetMaxHealthUpdateEvents(AgentItem);
                _health = maxHpUpdates.Count > 0 ? maxHpUpdates.Max(x => x.MaxHealth) : 1;
            }
            return _health;
        }
        public void SetManualHealth(int health)
        {
            _health = health;
        }

        // Combat Replay
        protected override void InitAdditionalCombatReplayData(ParsedLog log)
        {
            log.FightData.Logic.ComputeNPCCombatReplayActors(this, log, CombatReplay);
            if (CombatReplay.Rotations.Any())
            {
                CombatReplay.Decorations.Add(new FacingDecoration(((int)CombatReplay.TimeOffsets.start, (int)CombatReplay.TimeOffsets.end), new AgentConnector(this), CombatReplay.PolledRotations));
            }
        }

        public override JsonAbstractSingleActorCombatReplay GetCombatReplayJSON(CombatReplayMap map, ParsedLog log)
        {
            if (CombatReplay == null)
            {
                InitCombatReplay(log);
            }
            var aux = new JsonTargetCombatReplay
            {
                ID = AgentItem.UniqueID,
                Start = CombatReplay.TimeOffsets.start,
                End = CombatReplay.TimeOffsets.end,
                Positions = new List<double>()
            };
            foreach (Point3D pos in CombatReplay.PolledPositions)
            {
                (double x, double y) = map.GetMapCoord(pos.X, pos.Y);
                aux.Positions.Add(x);
                aux.Positions.Add(y);
            }
            return aux;
        }

        protected override void InitCombatReplay(ParsedLog log)
        {
            if (!log.CombatData.HasMovementData)
            {
                // no combat replay support on fight
                return;
            }
            CombatReplay = new CombatReplay();
            SetMovements(log);
            CombatReplay.PollingRate(log.FightData.FightDuration, log.FightData.GetMainTargets(log).Contains(this));
            TrimCombatReplay(log);
        }
    }
}

