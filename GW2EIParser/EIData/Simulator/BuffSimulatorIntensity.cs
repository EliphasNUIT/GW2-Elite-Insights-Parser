﻿using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIParser.Parser;
using GW2EIParser.Parser.ParsedData;

namespace GW2EIParser.EIData
{
    public class BuffSimulatorIntensity : BuffSimulator
    {
        private List<(AgentItem agent, bool extension)> _lastSrcRemoves = new List<(AgentItem agent, bool extension)>();
        // Constructor
        public BuffSimulatorIntensity(int capacity, ParsedLog log, StackingLogic logic) : base(capacity, log, logic)
        {
        }

        public override void Extend(long extension, long oldValue, AgentItem src, long start)
        {
            if ((BoonStack.Count > 0 && oldValue > 0) || BoonStack.Count == Capacity)
            {
                BoonStackItem minItem = BoonStack.MinBy(x => Math.Abs(x.TotalBoonDuration() - oldValue));
                if (minItem != null)
                {
                    minItem.Extend(extension, src);
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
                    Add(oldValue + extension, src, start);
                }
            }
        }

        // Public Methods

        protected override void Update(long timePassed)
        {
            if (BoonStack.Count > 0 && timePassed > 0)
            {
                _lastSrcRemoves.Clear();
                var toAdd = new BuffSimulationItemIntensity(BoonStack);
                if (GenerationSimulation.Count > 0)
                {
                    BuffSimulationItem last = GenerationSimulation.Last();
                    if (last.End > toAdd.Start)
                    {
                        last.SetEnd(toAdd.Start);
                    }
                }
                GenerationSimulation.Add(toAdd);
                long diff = Math.Min(BoonStack.Min(x => x.BoonDuration), timePassed);
                long leftOver = timePassed - diff;
                // Subtract from each
                for (int i = BoonStack.Count - 1; i >= 0; i--)
                {
                    var item = new BoonStackItem(BoonStack[i], diff, diff);
                    BoonStack[i] = item;
                    if (item.BoonDuration == 0)
                    {
                        _lastSrcRemoves.Add((item.SeedSrc, item.IsExtension));
                    }
                }
                BoonStack.RemoveAll(x => x.BoonDuration == 0);
                Update(leftOver);
            }
        }
    }
}
