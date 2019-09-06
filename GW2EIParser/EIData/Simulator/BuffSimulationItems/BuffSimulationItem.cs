using System;
using System.Collections.Generic;
using static GW2EIParser.Builders.JsonModels.JsonBuffData;

namespace GW2EIParser.EIData
{
    public abstract class BuffSimulationItem : AbstractBuffSimulationItem
    {
        public long Duration { get; protected set; }
        public long Start { get; protected set; }
        public long End => Start + Duration;

        protected BuffSimulationItem(long start, long duration)
        {
            Start = start;
            Duration = duration;
        }

        public long GetClampedDuration(long start, long end)
        {
            if (end > 0 && end - start > 0)
            {
                long startoffset = Math.Max(Math.Min(Duration, start - Start), 0);
                long itemEnd = Start + Duration;
                long endOffset = Math.Max(Math.Min(Duration, itemEnd - end), 0);
                return Duration - startoffset - endOffset;
            }
            return 0;
        }

        public abstract void OverrideEnd(long end);

        public abstract int GetTickingStacksCount();

        public abstract List<JsonBuffStackStatus.JsonBuffStackStatusItem> GetStackStatusList();

    }
}
