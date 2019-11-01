using System.Collections.Generic;
using System.ComponentModel;
using GW2EIParser.EIData;
using GW2EIParser.Parser;

namespace GW2EIParser.Builders.JsonModels
{
    /// <summary>
    /// Class corresponding to a phase
    /// </summary>
    public class JsonPhase
    {
        /// <summary>
        /// Start time of the phase
        /// </summary>
        [DefaultValue(null)]
        public long Start { get; set; }
        /// <summary>
        /// End time of the phase
        /// </summary>
        [DefaultValue(null)]
        public long End { get; set; }
        /// <summary>
        /// Name of the phase
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Index of sub phases
        /// </summary>
        /// <seealso cref="JsonLog.Phases"/>
        public List<int> SubPhases { get; set; }

        public JsonPhase(ParsedLog log, PhaseData phase)
        {
            Start = phase.Start;
            End = phase.End;
            Name = phase.Name;
            List<PhaseData> phases = log.FightData.GetPhases(log);
            for (int j = 1; j < phases.Count; j++)
            {
                PhaseData curPhase = phases[j];
                if (curPhase.Start < Start || curPhase.End > End ||
                     (curPhase.Start == Start && curPhase.End == End))
                {
                    continue;
                }
                if (SubPhases == null)
                {
                    SubPhases = new List<int>();
                }
                SubPhases.Add(j);
            }
        }
    }
}

