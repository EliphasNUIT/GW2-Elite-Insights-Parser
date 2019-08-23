using System.Collections.Generic;
using GW2EIParser.Parser;
using GW2EIParser.Parser.ParsedData;
using static GW2EIParser.Builders.JsonModels.JsonCombatReplayActors;

namespace GW2EIParser.EIData
{
    public class Mob : AbstractMasterActor
    {
        // Constructors
        public Mob(AgentItem agent) : base(agent)
        {
        }

        // Combat Replay
        protected override void InitAdditionalCombatReplayData(ParsedLog log)
        {
            log.FightData.Logic.ComputeMobCombatReplayActors(this, log, CombatReplay);
        }

        public override JsonAbstractMasterActorCombatReplay GetCombatReplayJSON(CombatReplayMap map, ParsedLog log)
        {
            if (CombatReplay == null)
            {
                InitCombatReplay(log);
            }
            JsonMobCombatReplay aux = new JsonMobCombatReplay
            {
                Img = CombatReplay.Icon,
                Positions = new List<double>(),
                Start = CombatReplay.TimeOffsets.start,
                End = CombatReplay.TimeOffsets.end,
                ID = GetCombatReplayID(log)
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
            CombatReplay = new CombatReplay
            {
                Icon = GeneralHelper.GetNPCIcon(ID)
            };
            SetMovements(log);
            CombatReplay.PollingRate(log.FightData.FightDuration, false);
            TrimCombatReplay(log);
        }
    }
}
