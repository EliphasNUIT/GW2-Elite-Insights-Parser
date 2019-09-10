using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIParser.Parser;
using GW2EIParser.Parser.ParsedData;
using GW2EIParser.Parser.ParsedData.CombatEvents;

namespace GW2EIParser.EIData
{
    public abstract class BuffSimulator
    {
        private long _id = 0;

        public class BoonStackItem
        {
            public long Start { get; private set; }
            public long BoonDuration { get; private set; }
            public AgentItem Src { get; private set; }
            public AgentItem SeedSrc { get; }
            public bool IsExtension { get; }

            public long ID { get; }

            public List<(AgentItem src, long value)> Extensions { get; } = new List<(AgentItem src, long value)>();

            public BoonStackItem(long start, long boonDuration, AgentItem src, AgentItem seedSrc, long id, bool isExtension)
            {
                Start = start;
                SeedSrc = seedSrc;
                BoonDuration = boonDuration;
                Src = src;
                IsExtension = isExtension;
                ID = id;
            }

            public BoonStackItem(long start, long boonDuration, AgentItem src, long id)
            {
                ID = id;
                Start = start;
                SeedSrc = src;
                BoonDuration = boonDuration;
                Src = src;
                IsExtension = false;
            }

            public BoonStackItem(BoonStackItem other, long startShift, long durationShift)
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

        // Fields
        protected List<BoonStackItem> BoonStack { get; }
        public List<BuffSimulationItem> GenerationSimulation { get; } = new List<BuffSimulationItem>();
        public List<BuffOverstackItem> OverstackSimulationResult { get; } = new List<BuffOverstackItem>();
        public List<BuffOverrideItem> OverrideSimulationResult { get; } = new List<BuffOverrideItem>();
        public List<BuffRemoveItem> RemovalSimulationResult { get; } = new List<BuffRemoveItem>();
        protected int Capacity { get; }
        private readonly ParsedLog _log;
        private readonly StackingLogic _logic;

        // Constructor
        protected BuffSimulator(int capacity, ParsedLog log, StackingLogic logic)
        {
            Capacity = capacity;
            BoonStack = new List<BoonStackItem>(capacity);
            _log = log;
            _logic = logic;
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
            BoonStack.Clear();
        }

        protected abstract void Update(long timePassed);

        public void Add(long boonDuration, AgentItem src, long start)
        {
            var toAdd = new BoonStackItem(start, boonDuration, src, ++_id);
            // Find empty slot
            if (BoonStack.Count < Capacity)
            {
                BoonStack.Add(toAdd);
                _logic.Sort(_log, BoonStack);
            }
            // Replace lowest value
            else
            {
                bool found = _logic.StackEffect(_log, toAdd, BoonStack, OverrideSimulationResult);
                if (!found)
                {
                    OverstackSimulationResult.Add(new BuffOverstackItem(src, boonDuration, start));
                }
            }
        }

        protected void Add(long boonDuration, AgentItem srcinstid, AgentItem seedSrc, long start, bool atFirst, bool isExtension)
        {
            var toAdd = new BoonStackItem(start, boonDuration, srcinstid, seedSrc,_id++, isExtension);
            // Find empty slot
            if (BoonStack.Count < Capacity)
            {
                if (atFirst)
                {
                    BoonStack.Insert(0, toAdd);
                }
                else
                {

                    BoonStack.Add(toAdd);
                }
                _logic.Sort(_log, BoonStack);
            }
            // Replace lowest value
            else
            {
                bool found = _logic.StackEffect(_log, toAdd, BoonStack, OverrideSimulationResult);
                if (!found)
                {
                    OverstackSimulationResult.Add(new BuffOverstackItem(srcinstid, boonDuration, start));
                }
            }
        }

        public void Remove(AgentItem by, long boonDuration, long start, ParseEnum.EvtcBuffRemove removeType)
        {
            if (GenerationSimulation.Count > 0)
            {
                BuffSimulationItem last = GenerationSimulation.Last();
                if (last.End > start)
                {
                    last.OverrideEnd(start);
                }
            }
            switch (removeType)
            {
                case ParseEnum.EvtcBuffRemove.All:
                    foreach (BoonStackItem stackItem in BoonStack)
                    {
                        RemovalSimulationResult.Add(new BuffRemoveItem(stackItem.Src, by, stackItem.BoonDuration, start, stackItem.ID));
                        if (stackItem.Extensions.Count > 0)
                        {
                            foreach ((AgentItem src, long value) in stackItem.Extensions)
                            {
                                RemovalSimulationResult.Add(new BuffRemoveItem(src, by, value, start, stackItem.ID));
                            }
                        }
                    }
                    BoonStack.Clear();
                    break;
                case ParseEnum.EvtcBuffRemove.Single:
                    for (int i = 0; i < BoonStack.Count; i++)
                    {
                        BoonStackItem stackItem = BoonStack[i];
                        if (Math.Abs(boonDuration - stackItem.TotalBoonDuration()) < 10)
                        {
                            RemovalSimulationResult.Add(new BuffRemoveItem(stackItem.Src, by, stackItem.BoonDuration, start, stackItem.ID));
                            if (stackItem.Extensions.Count > 0)
                            {
                                foreach ((AgentItem src, long value) in stackItem.Extensions)
                                {
                                    RemovalSimulationResult.Add(new BuffRemoveItem(src, by, value, start, stackItem.ID));
                                }
                            }
                            BoonStack.RemoveAt(i);
                            break;
                        }
                    }
                    break;
                default:
                    break;
            }
            _logic.Sort(_log, BoonStack);
        }

        public abstract void Extend(long extension, long oldValue, AgentItem src, long start);
    }
}
