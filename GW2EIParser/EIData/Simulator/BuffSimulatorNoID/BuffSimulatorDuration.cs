using System.Linq;
using GW2EIParser.Parser;
using GW2EIParser.Parser.ParsedData;

namespace GW2EIParser.EIData
{
    public class BuffSimulatorDuration : BuffSimulator
    {
        private (AgentItem agent, bool extension) _lastSrcRemove = (GeneralHelper.UnknownAgent, false);
        // Constructor
        public BuffSimulatorDuration(int capacity, ParsedLog log, StackingLogic logic) : base(capacity, log, logic)
        {
        }

        public override void Extend(long extension, long oldValue, AgentItem src, long start, uint stackID)
        {
            if ((BuffStack.Count > 0 && oldValue > 0) || BuffStack.Count == Capacity)
            {
                BuffStack[0].Extend(extension, src);
                ExtendedSimulationResult.Add(new BuffCreationItem(src, extension, start, BuffStack[0].ID));
            }
            else
            {
                Add(oldValue + extension, src, _lastSrcRemove.agent, start, true, _lastSrcRemove.extension);
            }
        }

        // Public Methods

        protected override void Update(long timePassed)
        {
            if (BuffStack.Count > 0)
            {
                if (timePassed > 0)
                {
                    _lastSrcRemove = (GeneralHelper.UnknownAgent, false);
                }
                var toAdd = new BuffSimulationItemQueue(BuffStack);
                if (GenerationSimulation.Count > 0)
                {
                    BuffSimulationItem last = GenerationSimulation.Last();
                    if (last.End > toAdd.Start)
                    {
                        last.OverrideEnd(toAdd.Start);
                    }
                }
                GenerationSimulation.Add(toAdd);
                long timeDiff = BuffStack[0].BoonDuration - timePassed;
                long diff;
                long leftOver = 0;
                if (timeDiff < 0)
                {
                    diff = BuffStack[0].BoonDuration;
                    leftOver = timePassed - diff;
                }
                else
                {
                    diff = timePassed;
                }
                BuffStack[0] = new BuffStackItem(BuffStack[0], diff, diff);
                for (int i = 1; i < BuffStack.Count; i++)
                {
                    BuffStack[i] = new BuffStackItem(BuffStack[i], diff, 0);
                }
                if (BuffStack[0].BoonDuration == 0)
                {
                    _lastSrcRemove = (BuffStack[0].SeedSrc, BuffStack[0].IsExtension);
                    BuffStack.RemoveAt(0);
                }
                if (leftOver > 0)
                {
                    Update(leftOver);
                }
            }
        }
    }
}
