using GW2EIParser.EIData;
using GW2EIParser.Parser;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using static GW2EIParser.Builders.JsonModels.JsonCombatReplayActors;
using static GW2EIParser.Builders.JsonModels.JsonCombatReplayDecorations;

namespace GW2EIParser.Builders.JsonModels
{
    /// <summary>
    /// Combat Replay data
    /// </summary>
    public class JsonCombatReplay
    {
        public List<JsonAbstractMasterActorCombatReplay> Players { get; set; } = new List<JsonAbstractMasterActorCombatReplay>();
        public List<JsonAbstractMasterActorCombatReplay> Mobs { get; set; } = new List<JsonAbstractMasterActorCombatReplay>();
        public List<JsonAbstractMasterActorCombatReplay> Npcs { get; set; } = new List<JsonAbstractMasterActorCombatReplay>();
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
            if (Mobs.Count == 0)
            {
                Mobs = null;
            }
            if (Decorations.Count == 0)
            {
                Decorations = null;
            }
        }

        private void GetCombatReplayActors(ParsedLog log, CombatReplayMap map)
        {
            foreach (Player p in log.PlayerList)
            {
                if (p.IsFakeActor)
                {
                    continue;
                }
                if (p.GetCombatReplayPolledPositions(log).Count == 0)
                {
                    continue;
                }
                Players.Add(p.GetCombatReplayJSON(map, log));
                foreach (GenericDecoration a in p.GetCombatReplayActors(log))
                {
                    Decorations.Add(a.GetCombatReplayJSON(map, log));
                }
            }
            foreach (Mob m in log.FightData.Logic.TrashMobs)
            {
                if (m.GetCombatReplayPolledPositions(log).Count == 0)
                {
                    continue;
                }
                Mobs.Add(m.GetCombatReplayJSON(map, log));
                foreach (GenericDecoration a in m.GetCombatReplayActors(log))
                {
                    Decorations.Add(a.GetCombatReplayJSON(map, log));
                }
            }
            foreach (Target target in log.FightData.Logic.Targets)
            {
                if (target.GetCombatReplayPolledPositions(log).Count == 0)
                {
                    continue;
                }
                Npcs.Add(target.GetCombatReplayJSON(map, log));
                foreach (GenericDecoration a in target.GetCombatReplayActors(log))
                {
                    Decorations.Add(a.GetCombatReplayJSON(map, log));
                }
            }
        }
    }
}
