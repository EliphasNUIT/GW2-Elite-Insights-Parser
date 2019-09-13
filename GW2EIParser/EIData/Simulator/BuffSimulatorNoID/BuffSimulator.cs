using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIParser.Parser;
using GW2EIParser.Parser.ParsedData;
using GW2EIParser.Parser.ParsedData.CombatEvents;

namespace GW2EIParser.EIData
{
    public abstract class BuffSimulator : AbstractBuffSimulator
    {
        private long _id = 0;

        // Fields
        protected int Capacity { get; }
        private readonly StackingLogic _logic;

        // Constructor
        protected BuffSimulator(int capacity, ParsedLog log, StackingLogic logic) : base(log)
        {
            Capacity = capacity;
            _logic = logic;
        }


        // Abstract Methods

        public override void Add(long boonDuration, AgentItem src, long start, uint id, bool addedAsActive)
        {
            var toAdd = new BoonStackItem(start, boonDuration, src, ++_id);
            bool addToCreationList = false;
            // Find empty slot
            if (BoonStack.Count < Capacity)
            {
                BoonStack.Add(toAdd);
                _logic.Sort(Log, BoonStack);
                addToCreationList = true;
            }
            // Replace lowest value
            else
            {
                addToCreationList = _logic.StackEffect(Log, toAdd, BoonStack, OverrideSimulationResult);
                if (!addToCreationList)
                {
                    OverstackSimulationResult.Add(new BuffOverstackItem(src, boonDuration, start));
                }
            }
            if (addToCreationList)
            {
                AddedSimulationResult.Add(new BuffCreationItem(src, boonDuration, start, toAdd.ID));
            }
        }

        protected void Add(long boonDuration, AgentItem src, AgentItem seedSrc, long start, bool atFirst, bool isExtension)
        {
            var toAdd = new BoonStackItem(start, boonDuration, src, seedSrc,_id++, isExtension);
            bool addToCreationList = false;
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
                _logic.Sort(Log, BoonStack);
                addToCreationList = true;
            }
            // Replace lowest value
            else
            {
                addToCreationList = _logic.StackEffect(Log, toAdd, BoonStack, OverrideSimulationResult);
                if (!addToCreationList)
                {
                    OverstackSimulationResult.Add(new BuffOverstackItem(src, boonDuration, start));
                }
            }
            if (addToCreationList)
            {
                AddedSimulationResult.Add(new BuffCreationItem(src, boonDuration, start, toAdd.ID));
            }
        }

        public override void Remove(AgentItem by, long boonDuration, long start, ParseEnum.EvtcBuffRemove removeType, uint id)
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
            _logic.Sort(Log, BoonStack);
        }
    }
}
