using System;
using System.Collections.Generic;
using GW2EIParser.Parser;
using GW2EIParser.Parser.ParsedData;
using GW2EIParser.Parser.ParsedData.CombatEvents;

namespace GW2EIParser.EIData
{
    public abstract class BuffSimulatorID : AbstractBuffSimulator
    {
        protected List<(long duration, AgentItem src)> OverrideCandidates { get; } = new List<(long duration, AgentItem src)>();

        // Constructor
        protected BuffSimulatorID(ParsedLog log) : base(log)
        {
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
            switch (removeType)
            {
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
                            //removedStacks = BuffStack.Count;
                            throw new InvalidOperationException("remove all failed");
                        }
                        // buff cleanse all
                        for (int i = 0; i < removedStacks; i++)
                        {
                            BuffStackItem stackItem = BuffStack[i];
                            RemovalSimulationResult.Add(new BuffRemoveItem(stackItem.Src, by, stackItem.Duration, time, stackItem.ID));
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
                //return;
                throw new InvalidOperationException("Remove has failed");
            }
            BuffStack.Remove(toRemove);
            // Removed due to override
            (long duration, AgentItem src)? candidate = OverrideCandidates.Find(x => Math.Abs(x.duration - removedDuration) < 15);
            if (candidate.Value.src != null)
            {
                (long duration, AgentItem candSrc) = candidate.Value;
                OverrideCandidates.Remove(candidate.Value);
                OverrideSimulationResult.Add(new BuffOverrideItem(toRemove.Src, candSrc, toRemove.Duration, toRemove.Start, toRemove.ID));
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
                RemovalSimulationResult.Add(new BuffRemoveItem(toRemove.Src, by, toRemove.Duration, time, toRemove.ID));
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
