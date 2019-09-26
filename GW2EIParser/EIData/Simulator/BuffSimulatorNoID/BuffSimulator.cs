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

        // Fields
        private readonly StackingLogic _logic;
        protected int Capacity { get; }

        // Constructor
        protected BuffSimulator(int capacity, ParsedLog log, StackingLogic logic) : base(log)
        {
            Capacity = capacity;
            _logic = logic;
        }


        // Abstract Methods

        public override void Add(long boonDuration, AgentItem src, long start, uint id, bool addedAsActive, uint overstackDuration)
        {
            var toAdd = new BuffStackItem(start, boonDuration, src, ++ID);
            bool addToCreationList;
            // Find empty slot
            if (BuffStack.Count < Capacity)
            {
                BuffStack.Add(toAdd);
                _logic.Sort(Log, BuffStack);
                addToCreationList = true;
            }
            // Replace lowest value
            else
            {
                addToCreationList = _logic.StackEffect(Log, toAdd, BuffStack, OverrideSimulationResult);
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
            var toAdd = new BuffStackItem(start, boonDuration, src, seedSrc,++ID, isExtension);
            bool addToCreationList;
            // Find empty slot
            if (BuffStack.Count < Capacity)
            {
                if (atFirst)
                {
                    BuffStack.Insert(0, toAdd);
                }
                else
                {

                    BuffStack.Add(toAdd);
                }
                _logic.Sort(Log, BuffStack);
                addToCreationList = true;
            }
            // Replace lowest value
            else
            {
                addToCreationList = _logic.StackEffect(Log, toAdd, BuffStack, OverrideSimulationResult);
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

        public override void Remove(AgentItem by, long boonDuration, long start, ParseEnum.BuffRemove removeType, uint id)
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
                case ParseEnum.BuffRemove.All:
                    foreach (BuffStackItem stackItem in BuffStack)
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
                    BuffStack.Clear();
                    break;
                case ParseEnum.BuffRemove.Single:
                    for (int i = 0; i < BuffStack.Count; i++)
                    {
                        BuffStackItem stackItem = BuffStack[i];
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
                            BuffStack.RemoveAt(i);
                            break;
                        }
                    }
                    break;
                default:
                    break;
            }
            _logic.Sort(Log, BuffStack);
        }


        public override void Activate(uint id)
        {

        }
        public override void Reset(uint id, long toDuration)
        {

        }
    }
}
