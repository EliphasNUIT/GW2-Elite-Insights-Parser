using System.Collections.Generic;
using System.Linq;
using GW2EIParser.Parser;
using static GW2EIParser.Builders.JsonModels.JsonBuffData;
using static GW2EIParser.Builders.JsonModels.JsonLog;

namespace GW2EIParser.EIData
{
    public class BuffsGraphModel
    {
        public Buff Buff { get; }
        public List<BuffSegment> BuffChart { get; private set; } = new List<BuffSegment>();
        private readonly AbstractBuffSimulator _simulator;
        public bool IsSourceBased => _simulator != null;

        // Constructor
        public BuffsGraphModel(Buff buff)
        {
            Buff = buff;
        }
        public BuffsGraphModel(Buff buff, List<BuffSegment> segments, AbstractBuffSimulator simulator)
        {
            Buff = buff;
            _simulator = simulator;
            BuffChart = segments;
            // needed for fast is present, stack and condi/boon graphs
            FuseSegments();
        }

        public int GetStackCount(long time)
        {
            for (int i = BuffChart.Count - 1; i >= 0; i--)
            {
                BuffSegment seg = BuffChart[i];
                if (seg.Start <= time && time <= seg.End)
                {
                    return seg.Value;
                }
            }
            return 0;
        }


        public bool IsPresent(long time, long window)
        {
            int count = 0;
            foreach (BuffSegment seg in BuffChart)
            {
                if (seg.Intersect(time - window, time + window))
                {
                    count += seg.Value;
                }
            }
            return count > 0;
        }
        public void FuseSegments()
        {
            var newChart = new List<BuffSegment>();
            BuffSegment last = null;
            foreach (BuffSegment seg in BuffChart)
            {
                if (seg.Start == seg.End)
                {
                    continue;
                }
                if (last == null)
                {
                    newChart.Add(new BuffSegment(seg));
                    last = newChart.Last();
                }
                else
                {
                    if (seg.Value == last.Value)
                    {
                        last.End = seg.End;
                    }
                    else
                    {
                        newChart.Add(new BuffSegment(seg));
                        last = newChart.Last();
                    }
                }
            }
            BuffChart = newChart;
        }

        public List<int> GetStatesList()
        {
            if (BuffChart.Count == 0)
            {
                return null;
            }
            var res = new List<int>();
            foreach (BuffSegment item in BuffChart)
            {
                res.Add((int)item.Start);
                res.Add(item.Value);
            }
            return res.Count > 0 ? res : null;
        }

        public JsonBuffStackStatus GetStackStatusList(ParsedLog log, Dictionary<string, Desc> description)
        {
            if (BuffChart.Count == 0 || _simulator == null)
            {
                return null;
            }
            var res = new JsonBuffStackStatus(_simulator.GenerationSimulation, log, description);
            _simulator.GenerationSimulation.Clear();
            return res;
        }

        public (List<JsonBuffOverstackItem>, List<JsonBuffOverrideItem>, List<JsonBuffRemoveItem>) GetWasteStatusList(ParsedLog log, Dictionary<string, Desc> description)
        {
            if (BuffChart.Count == 0 || _simulator == null)
            {
                return (null, null, null);
            }
            var overstack = new List<JsonBuffOverstackItem>();
            var overriden = new List<JsonBuffOverrideItem>();
            var removed = new List<JsonBuffRemoveItem>();
            foreach (BuffOverstackItem item in _simulator.OverstackSimulationResult)
            {
                overstack.Add(new JsonBuffOverstackItem(item, log, description));
            }
            _simulator.OverstackSimulationResult.Clear();
            foreach (BuffOverrideItem item in _simulator.OverrideSimulationResult)
            {
                overriden.Add(new JsonBuffOverrideItem(item, log, description));
            }
            _simulator.OverrideSimulationResult.Clear();
            foreach (BuffRemoveItem item in _simulator.RemovalSimulationResult)
            {
                removed.Add(new JsonBuffRemoveItem(item, log, description));
            }
            _simulator.RemovalSimulationResult.Clear();
            return (overstack.Any() ? overstack : null, overriden.Any() ? overriden : null, removed.Any() ? removed : null);
        }

        public (List<JsonCreationItem>, List<JsonCreationItem>) GetCreationStatusList(ParsedLog log, Dictionary<string, Desc> description)
        {
            if (BuffChart.Count == 0 || _simulator == null)
            {
                return (null, null);
            }
            var added = new List<JsonCreationItem>();
            var extended = new List<JsonCreationItem>();
            foreach (BuffCreationItem item in _simulator.AddedSimulationResult)
            {
                added.Add(new JsonCreationItem(item, log, description));
            }
            _simulator.AddedSimulationResult.Clear();
            foreach (BuffCreationItem item in _simulator.ExtendedSimulationResult)
            {
                extended.Add(new JsonCreationItem(item, log, description));
            }
            _simulator.ExtendedSimulationResult.Clear();
            return (added.Any() ? added : null, extended.Any() ? extended : null);
        }

    }
}

