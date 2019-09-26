using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIParser.Parser;
using GW2EIParser.Parser.ParsedData;
using GW2EIParser.Parser.ParsedData.CombatEvents;

namespace GW2EIParser.EIData
{
    public abstract class AbstractBuffSimulator
    {
        public class BuffStackItem
        {
            public long Start { get; private set; }
            public long BoonDuration { get; private set; }
            public AgentItem Src { get; private set; }
            public AgentItem SeedSrc { get; }
            public bool IsExtension { get; }

            public long ID { get; }

            public long StackID { get; protected set; } = 0;

            public List<(AgentItem src, long value)> Extensions { get; } = new List<(AgentItem src, long value)>();

            public BuffStackItem(long start, long boonDuration, AgentItem src, AgentItem seedSrc, long id, bool isExtension)
            {
                Start = start;
                SeedSrc = seedSrc;
                BoonDuration = boonDuration;
                Src = src;
                IsExtension = isExtension;
                ID = id;
            }

            public BuffStackItem(long start, long boonDuration, AgentItem src, long id)
            {
                ID = id;
                Start = start;
                SeedSrc = src;
                BoonDuration = boonDuration;
                Src = src;
                IsExtension = false;
            }

            public BuffStackItem(BuffStackItem other, long startShift, long durationShift)
            {
                ID = other.ID;
                Start = other.Start + startShift;
                BoonDuration = other.BoonDuration - durationShift;
                Src = other.Src;
                SeedSrc = other.SeedSrc;
                Extensions = other.Extensions;
                IsExtension = other.IsExtension;
                if (BoonDuration == 0 && Extensions.Count > 0)
                {
                    (AgentItem src, long value) = Extensions.First();
                    Extensions.RemoveAt(0);
                    Src = src;
                    BoonDuration = value;
                    IsExtension = true;
                }
            }

            public long TotalBoonDuration()
            {
                long res = BoonDuration;
                foreach ((AgentItem src, long value) in Extensions)
                {
                    res += value;
                }
                return res;
            }

            public void Extend(long value, AgentItem src)
            {
                Extensions.Add((src, value));
            }
        }

        protected long ID { get; set; } = 0;
        // Fields
        protected List<BuffStackItem> BuffStack { get; }
        public List<BuffSimulationItem> GenerationSimulation { get; } = new List<BuffSimulationItem>();
        public List<BuffOverstackItem> OverstackSimulationResult { get; } = new List<BuffOverstackItem>();
        public List<BuffOverrideItem> OverrideSimulationResult { get; } = new List<BuffOverrideItem>();
        public List<BuffRemoveItem> RemovalSimulationResult { get; } = new List<BuffRemoveItem>();

        public List<BuffCreationItem> AddedSimulationResult { get; } = new List<BuffCreationItem>();
        public List<BuffCreationItem> ExtendedSimulationResult { get; } = new List<BuffCreationItem>();

        protected ParsedLog Log { get; }

        // Constructor
        protected AbstractBuffSimulator(ParsedLog log)
        {
            BuffStack = new List<BuffStackItem>();
            Log = log;
        }


        // Abstract Methods
        /// <summary>
        /// Make sure the last element does not overflow the fight
        /// </summary>
        /// <param name="fightDuration">Duration of the fight</param>
        public void Trim(long fightDuration)
        {
            for (int i = GenerationSimulation.Count - 1; i >= 0; i--)
            {
                BuffSimulationItem data = GenerationSimulation[i];
                if (data.End > fightDuration)
                {
                    data.OverrideEnd(fightDuration);
                }
                else
                {
                    break;
                }
            }
            GenerationSimulation.RemoveAll(x => x.Duration <= 0);
        }

        public void Simulate(List<AbstractBuffEvent> logs, long fightDuration)
        {
            long firstTimeValue = logs.Count > 0 ? Math.Min(logs.First().Time, 0) : 0;
            long timeCur = firstTimeValue;
            long timePrev = firstTimeValue;
            foreach (AbstractBuffEvent log in logs)
            {
                timeCur = log.Time;
                if (timeCur - timePrev < 0)
                {
                    throw new InvalidOperationException("Negative passed time in boon simulation");
                }
                Update(timeCur - timePrev);
                log.UpdateSimulator(this);
                timePrev = timeCur;
            }
            Update(fightDuration - timePrev);
            GenerationSimulation.RemoveAll(x => x.Duration <= 0);
            BuffStack.Clear();
        }

        protected abstract void Update(long timePassed);

        public abstract void Add(long duration, AgentItem src, long start, uint id, bool addedActive, uint overstackDuration);

        public abstract void Remove(AgentItem by, long removedDuration, long start, ParseEnum.BuffRemove removeType, uint id);

        public abstract void Extend(long extension, long oldValue, AgentItem src, long start, uint id);

        public abstract void Activate(uint id);
        public abstract void Reset(uint id, long toDuration);
    }
}
