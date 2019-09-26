﻿using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIParser.Parser;
using GW2EIParser.Parser.ParsedData;

namespace GW2EIParser.EIData
{
    public class BuffSimulatorIntensity : BuffSimulator
    {
        private readonly List<(AgentItem agent, bool extension)> _lastSrcRemoves = new List<(AgentItem agent, bool extension)>();
        // Constructor
        public BuffSimulatorIntensity(int capacity, ParsedLog log, StackingLogic logic) : base(capacity, log, logic)
        {
        }

        public override void Extend(long extension, long oldValue, AgentItem src, long start, uint stackID)
        {
            if ((BuffStack.Count > 0 && oldValue > 0) || BuffStack.Count == Capacity)
            {
                BuffStackItem minItem = BuffStack.MinBy(x => Math.Abs(x.TotalBoonDuration() - oldValue));
                if (minItem != null)
                {
                    minItem.Extend(extension, src);
                    ExtendedSimulationResult.Add(new BuffCreationItem(src, extension, start, minItem.ID));
                }
            }
            else
            {
                if (_lastSrcRemoves.Count > 0)
                {
                    Add(oldValue + extension, src, _lastSrcRemoves.First().agent, start, false, _lastSrcRemoves.First().extension);
                    _lastSrcRemoves.RemoveAt(0);
                }
                else
                {
                    Add(oldValue + extension, src, start, 0, true, 0);
                }
            }
        }

        // Public Methods

        protected override void Update(long timePassed)
        {
            if (BuffStack.Count > 0 && timePassed > 0)
            {
                _lastSrcRemoves.Clear();
                var toAdd = new BuffSimulationItemIntensity(BuffStack);
                if (GenerationSimulation.Count > 0)
                {
                    BuffSimulationItem last = GenerationSimulation.Last();
                    if (last.End > toAdd.Start)
                    {
                        last.OverrideEnd(toAdd.Start);
                    }
                }
                GenerationSimulation.Add(toAdd);
                long diff = Math.Min(BuffStack.Min(x => x.BoonDuration), timePassed);
                long leftOver = timePassed - diff;
                // Subtract from each
                for (int i = BuffStack.Count - 1; i >= 0; i--)
                {
                    var item = new BuffStackItem(BuffStack[i], diff, diff);
                    BuffStack[i] = item;
                    if (item.BoonDuration == 0)
                    {
                        _lastSrcRemoves.Add((item.SeedSrc, item.IsExtension));
                    }
                }
                BuffStack.RemoveAll(x => x.BoonDuration == 0);
                Update(leftOver);
            }
        }
    }
}
