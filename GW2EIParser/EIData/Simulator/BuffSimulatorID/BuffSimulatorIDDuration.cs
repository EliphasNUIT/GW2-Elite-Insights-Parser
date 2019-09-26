﻿using System;
using System.Collections.Generic;
using System.Linq;
using GW2EIParser.Parser;
using GW2EIParser.Parser.ParsedData;
using GW2EIParser.Parser.ParsedData.CombatEvents;

namespace GW2EIParser.EIData
{
    public class BuffSimulatorIDDuration : BuffSimulatorID
    {
        private BuffStackItem _activeStack;

        // Constructor
        public BuffSimulatorIDDuration(ParsedLog log) : base(log)
        {
        }

        public override void Activate(uint stackID)
        {
            BuffStackItem active = BuffStack.Find(x => x.StackID == stackID);
            _activeStack = active ?? throw new InvalidOperationException("Activate has failed");
        }

        public override void Add(long duration, AgentItem src, long start, uint stackID, bool addedActive, uint overstackDuration)
        {
            var toAdd = new BuffStackItem(start, duration, src, ++ID, stackID);
            BuffStack.Add(toAdd);
            AddedSimulationResult.Add(new BuffCreationItem(src, duration, start, toAdd.ID));
            if (overstackDuration > 0)
            {
                OverrideCandidates.Add((overstackDuration, src));
            }
            if (addedActive)
            {
                _activeStack = toAdd;
            }
        }

        protected override void Update(long timePassed)
        {
            if (BuffStack.Count > 0 && timePassed > 0 && _activeStack != null)
            {
                var toAdd = new BuffSimulationItemQueue(BuffStack, _activeStack);
                if (toAdd.End > toAdd.Start + timePassed)
                {
                    toAdd.OverrideEnd(toAdd.Start + timePassed);
                }
                GenerationSimulation.Add(toAdd);
                long timeDiff = _activeStack.BoonDuration - timePassed;
                long diff;
                long leftOver = 0;
                if (timeDiff < 0)
                {
                    diff = _activeStack.BoonDuration;
                    leftOver = timePassed - diff;
                }
                else
                {
                    diff = timePassed;
                }
                BuffStackItem oldActive = _activeStack;
                _activeStack.Shift(diff,diff);
                for (int i = 0; i < BuffStack.Count; i++)
                {
                    if (BuffStack[i] != oldActive)
                    {
                        BuffStack[i].Shift(diff, 0);
                    }
                }
                // that means the stack was not an extension, extend duration to match time passed
                if (_activeStack.BoonDuration == 0)
                {
                    _activeStack.Shift(0, -leftOver);
                }
                Update(leftOver);
            } 
        }
    }
}
