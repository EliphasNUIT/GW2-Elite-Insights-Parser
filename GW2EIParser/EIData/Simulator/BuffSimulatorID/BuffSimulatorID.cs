﻿using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIParser.Parser;
using GW2EIParser.Parser.ParsedData;
using GW2EIParser.Parser.ParsedData.CombatEvents;

namespace GW2EIParser.EIData
{
    public abstract class BuffSimulatorID : AbstractBuffSimulator
    {
        protected List<(long duration, AgentItem src)> OverrideCandidates { get; } = new List<(long duration, AgentItem src)>();

        protected class BuffStackItemID : BuffStackItem
        {

            public BuffStackItemID(long start, long boonDuration, AgentItem src, long id, long stackID) : base(start, boonDuration, src, id)
            {
                StackID = stackID;
            }

            public BuffStackItemID(BuffStackItemID other, long startShift, long durationShift) : base(other, startShift, durationShift)
            {
                StackID = other.StackID;
            }
        }
        // Constructor
        protected BuffSimulatorID(ParsedLog log) : base(log)
        {
        }

        public override void Add(long duration, AgentItem src, long start, uint stackID, uint overstackDuration)
        {
            var toAdd = new BuffStackItemID(start, duration, src, ++ID, stackID);
            BuffStack.Add(toAdd);
            AddedSimulationResult.Add(new BuffCreationItem(src, duration, start, toAdd.ID));
            if (overstackDuration > 0)
            {
                OverrideCandidates.Add((overstackDuration, src));
            }
        }

        public override void Extend(long extension, long oldValue, AgentItem src, long time, uint stackID)
        {
            BuffStackItem toExtend = BuffStack.Find(x => x.StackID == stackID);
            if (toExtend == null)
            {
                throw new InvalidOperationException("Extend has failed");
            }
            toExtend.Extend(extension, src);
            ExtendedSimulationResult.Add(new BuffCreationItem(src, extension, time, toExtend.ID));
        }

        public override void Remove(AgentItem by, long removedDuration, int removedStacks, long time, ParseEnum.BuffRemove removeType, uint stackID)
        {
            BuffStackItem toRemove;
            switch (removeType) {
                case ParseEnum.BuffRemove.All:
                    // remove all due to despawn event
                    if (removedStacks == BuffRemoveAllEvent.FullRemoval)
                    {
                        BuffStack.Clear();
                        return;
                    }
                    if (BuffStack.Count != 1)
                    {
                        if (BuffStack.Count < removedStacks)
                        {
                            throw new InvalidOperationException("remove all failed");
                        }
                        // buff cleanse all
                        for (int i = 0; i < removedStacks; i++)
                        {
                            BuffStackItem stackItem = BuffStack[i];
                            RemovalSimulationResult.Add(new BuffRemoveItem(stackItem.Src, by, stackItem.BoonDuration, time, stackItem.ID));
                            if (stackItem.Extensions.Count > 0)
                            {
                                foreach ((AgentItem src, long value) in stackItem.Extensions)
                                {
                                    RemovalSimulationResult.Add(new BuffRemoveItem(src, by, value, time, stackItem.ID));
                                }
                            }
                        }
                        BuffStack = BuffStack.GetRange(removedStacks, BuffStack.Count - removedStacks);
                        return;
                    }
                    toRemove = BuffStack[0];
                    break;
                case ParseEnum.BuffRemove.Single:
                    toRemove = BuffStack.Find(x => x.StackID == stackID);
                    break;
                default:
                    throw new InvalidOperationException("Unknown remove type");
            }
            if (toRemove == null)
            {
                throw new InvalidOperationException("Remove has failed");
            }
            BuffStack.Remove(toRemove);
            // Removed due to override
            (long duration, AgentItem src)? candidate = OverrideCandidates.Find(x => Math.Abs(x.duration - removedDuration) < 15);
            if (candidate.Value.src != null)
            {
                (long duration, AgentItem candSrc) = candidate.Value;
                OverrideCandidates.Remove(candidate.Value);
                OverrideSimulationResult.Add(new BuffOverrideItem(toRemove.Src, candSrc, toRemove.BoonDuration, toRemove.Start, toRemove.ID));
                if (toRemove.Extensions.Count > 0)
                {
                    foreach ((AgentItem src, long value) in toRemove.Extensions)
                    {
                        OverrideSimulationResult.Add(new BuffOverrideItem(src, candSrc, value, toRemove.Start, toRemove.ID));
                    }
                }
            } 
            // Removed due to a cleanse
            else if (removedDuration > 50 && by != GeneralHelper.UnknownAgent)
            {
                RemovalSimulationResult.Add(new BuffRemoveItem(toRemove.Src, by, toRemove.BoonDuration, time, toRemove.ID));
                if (toRemove.Extensions.Count > 0)
                {
                    foreach ((AgentItem src, long value) in toRemove.Extensions)
                    {
                        RemovalSimulationResult.Add(new BuffRemoveItem(src, by, value, time, toRemove.ID));
                    }
                }
            }
        }

        public override void Reset(uint id, long toDuration)
        {
            // nothing to do? an activate should always accompany this
        }

    }
}