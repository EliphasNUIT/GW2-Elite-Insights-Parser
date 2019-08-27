using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using GW2EIParser.EIData;
using GW2EIParser.Parser;
using static GW2EIParser.Builders.JsonModels.JsonCombatReplayActors;
using static GW2EIParser.Builders.JsonModels.JsonCombatReplayDecorations;

namespace GW2EIParser.Builders.JsonModels
{
    /// <summary>
    /// Combat Replay data
    /// </summary>
    public class JsonCombatReplay
    {
        public List<JsonAbstractSingleActorCombatReplay> Players { get; set; } = new List<JsonAbstractSingleActorCombatReplay>();
        public List<JsonAbstractSingleActorCombatReplay> Npcs { get; set; } = new List<JsonAbstractSingleActorCombatReplay>();
        public List<JsonAbstractSingleActorCombatReplay> Minions { get; set; } = new List<JsonAbstractSingleActorCombatReplay>();
        public List<JsonCombatReplayGenericDecoration> Decorations { get; set; } = new List<JsonCombatReplayGenericDecoration>();
        public List<int> Sizes { get; set; }
        public int MaxTime { get; set; }
        public float Inch { get; set; }
        public int PollingRate { get; set; }
        public List<JsonCombatReplayMap> Maps { get; set; }

        public class JsonCombatReplayMap
        {
            public string Link { get; set; }
            [DefaultValue(-1)]
            public long Start { get; set; }
            [DefaultValue(-1)]
            public long End { get; set; }

            public JsonCombatReplayMap(CombatReplayMap.MapItem map)
            {
                Link = map.Link;
                Start = map.Start;
                End = map.End;
            }
        }

        public JsonCombatReplay(ParsedLog log)
        {
            CombatReplayMap map = log.FightData.Logic.GetCombatMap(log);
            Maps = map.Maps.Select(x => new JsonCombatReplayMap(x)).ToList();
            (int width, int height) = map.GetPixelMapSize();
            Sizes = new List<int> { width, height };
            Inch = map.GetInch();
            MaxTime = log.PlayerList.First().GetCombatReplayTimes(log).Last();
            GetCombatReplayActors(log, map);
            if (Decorations.Count == 0)
            {
                Decorations = null;
            }
        }

        private void FillCombatReplayArray(IEnumerable<AbstractSingleActor> actors, ParsedLog log, CombatReplayMap map, List<JsonAbstractSingleActorCombatReplay> toFill)
        {
            foreach (AbstractSingleActor actor in actors)
            {
                if (actor.IsFakeActor || actor.GetCombatReplayPolledPositions(log).Count == 0)
                {
                    continue;
                }
                toFill.Add(actor.GetCombatReplayJSON(map, log));
                foreach (GenericDecoration dec in actor.GetCombatReplayActors(log))
                {
                    Decorations.Add(dec.GetCombatReplayJSON(map, log));
                }
                foreach (Minions minions in actor.GetMinions(log).Values)
                {
                    FillCombatReplayArray(minions.MinionList, log, map, Minions);
                }
            }
        }

        private void GetCombatReplayActors(ParsedLog log, CombatReplayMap map)
        {
            FillCombatReplayArray(log.PlayerList, log, map, Players);
            FillCombatReplayArray(log.FightData.Logic.NPCs, log, map, Npcs);
        }
    }
}
