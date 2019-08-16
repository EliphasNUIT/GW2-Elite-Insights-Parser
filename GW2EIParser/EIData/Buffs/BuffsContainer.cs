using GW2EIParser.Parser;
using GW2EIParser.Parser.ParsedData;
using System;
using System.Collections.Generic;
using System.Linq;
using static GW2EIParser.EIData.Buff;

namespace GW2EIParser.EIData
{
    public class BuffsContainer
    {

        public Dictionary<long, Buff> BuffsByIds { get; }
        public Dictionary<BoonNature, List<Buff>> BoonsByNature { get; }
        public Dictionary<BoonSource, List<Buff>> BoonsBySource { get; }
        public Dictionary<BoonType, List<Buff>> BoonsByType { get; }
        private readonly Dictionary<string, Buff> _boonsByName;
        public Dictionary<int, List<Buff>> BoonsByCapacity { get; }

        private readonly BuffSourceFinder _boonSourceFinder;

        public BuffsContainer(ulong build)
        {
            List<Buff> currentBoons = new List<Buff>();
            foreach (List<Buff> boons in AllBoons)
            {
                currentBoons.AddRange(boons.Where(x => x.MaxBuild > build && build >= x.MinBuild));
            }
            BuffsByIds = currentBoons.GroupBy(x => x.ID).ToDictionary(x => x.Key, x => x.First());
            BoonsByNature = currentBoons.GroupBy(x => x.Nature).ToDictionary(x => x.Key, x => x.ToList());
            BoonsBySource = currentBoons.GroupBy(x => x.Source).ToDictionary(x => x.Key, x => x.ToList());
            BoonsByType = currentBoons.GroupBy(x => x.Type).ToDictionary(x => x.Key, x => x.ToList());
            _boonsByName = currentBoons.GroupBy(x => x.Name).ToDictionary(x => x.Key, x => x.ToList().Count > 1 ? throw new InvalidOperationException(x.First().Name) : x.First());
            BoonsByCapacity = currentBoons.GroupBy(x => x.Capacity).ToDictionary(x => x.Key, x => x.ToList());
            _boonSourceFinder = GetBoonSourceFinder(build, new HashSet<long>(BoonsByNature[BoonNature.Boon].Select(x => x.ID)));
        }

        public Buff GetBoonByName(string name)
        {
            if (_boonsByName.TryGetValue(name, out Buff buff))
            {
                return buff;
            }
            throw new InvalidOperationException("Buff " + name + " does not exist");
        }

        public AgentItem TryFindSrc(AgentItem dst, long time, long extension, ParsedLog log, long buffID)
        {
            return _boonSourceFinder.TryFindSrc(dst, time, extension, log, buffID);
        }

        // Non shareable buffs
        private List<Buff> GetRemainingBuffsList(BoonSource source)
        {
            return BoonsBySource[source].Where(x => x.Nature == BoonNature.GraphOnlyBuff).ToList();
        }
        public List<Buff> GetRemainingBuffsList(string source)
        {
            return GetRemainingBuffsList(ProfToEnum(source));
        }
    }
}
