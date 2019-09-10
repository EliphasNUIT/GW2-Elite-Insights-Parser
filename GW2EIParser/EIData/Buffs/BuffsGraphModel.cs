﻿using System.Collections.Generic;
using System.Linq;
using GW2EIParser.Parser;
using static GW2EIParser.Builders.JsonModels.JsonBuffData;
using static GW2EIParser.Builders.JsonModels.JsonLog;

namespace GW2EIParser.EIData
{
    public class BuffsGraphModel
    {
        public Buff Boon { get; }
        public List<BuffSegment> ValueBasedBoonChart { get; private set; } = new List<BuffSegment>();
        private readonly List<BuffSimulationItem> _sourceBasedBoonChart;
        private readonly List<BuffOverstackItem> _overstackChart;
        private readonly List<BuffOverrideItem> _overrideChart;
        private readonly List<BuffRemoveItem> _removalChart;
        public bool IsSourceBased => _sourceBasedBoonChart != null;

        // Constructor
        public BuffsGraphModel(Buff boon)
        {
            Boon = boon;
        }
        public BuffsGraphModel(Buff boon, List<BuffSegment> segments, List<BuffSimulationItem> boonChartWithSource, List<BuffOverstackItem> overstackChart, List<BuffOverrideItem> overrideChart, List<BuffRemoveItem> removalChart)
        {
            Boon = boon;
            _sourceBasedBoonChart = boonChartWithSource;
            ValueBasedBoonChart = segments;
            _overstackChart = overstackChart;
            _overrideChart = overrideChart;
            _removalChart = removalChart;
            // needed for fast is present, stack and condi/boon graphs
            FuseSegments();
        }

        public int GetStackCount(long time)
        {
            for (int i = ValueBasedBoonChart.Count - 1; i >= 0; i--)
            {
                BuffSegment seg = ValueBasedBoonChart[i];
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
            foreach (BuffSegment seg in ValueBasedBoonChart)
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
            foreach (BuffSegment seg in ValueBasedBoonChart)
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
            ValueBasedBoonChart = newChart;
        }

        public List<int> GetStatesList()
        {
            if (ValueBasedBoonChart.Count == 0)
            {
                return null;
            }
            var res = new List<int>();
            foreach (BuffSegment item in ValueBasedBoonChart)
            {
                res.Add((int)item.Start);
                res.Add(item.Value);
            }
            return res.Count > 0 ? res : null;
        }

        public JsonBuffStackStatus GetStackStatusList(ParsedLog log, Dictionary<string, Desc> description)
        {
            if (ValueBasedBoonChart.Count == 0 || _sourceBasedBoonChart == null)
            {
                return null;
            }
            var res = new JsonBuffStackStatus(_sourceBasedBoonChart, log, description);
            _sourceBasedBoonChart.Clear();
            return res;
        }

        public (List<JsonBuffOverstackItem>, List<JsonBuffOverrideItem>, List<JsonBuffRemoveItem>) GetWasteStatusList(ParsedLog log, Dictionary<string, Desc> description)
        {
            if (ValueBasedBoonChart.Count == 0 || _sourceBasedBoonChart == null)
            {
                return (null, null, null);
            }
            var overstack = new List<JsonBuffOverstackItem>();
            var overriden = new List<JsonBuffOverrideItem>();
            var removed = new List<JsonBuffRemoveItem>();
            foreach (BuffOverstackItem item in _overstackChart)
            {
                overstack.Add(new JsonBuffOverstackItem(item, log, description));
            }
            _overstackChart.Clear();
            foreach (BuffOverrideItem item in _overrideChart)
            {
                overriden.Add(new JsonBuffOverrideItem(item, log, description));
            }
            _overrideChart.Clear();
            foreach (BuffRemoveItem item in _removalChart)
            {
                removed.Add(new JsonBuffRemoveItem(item, log, description));
            }
            _removalChart.Clear();
            return (overstack.Any() ? overstack : null, overriden.Any() ? overriden : null, removed.Any() ? removed : null);
        }

    }
}
