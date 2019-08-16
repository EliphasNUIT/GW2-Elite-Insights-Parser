using System.Collections.Generic;

namespace GW2EIParser.Builders.HtmlModels
{   
    public class ChartDataDto
    {      
        public List<PhaseChartDataDto> Phases = new List<PhaseChartDataDto>();      
        public List<MechanicChartDataDto> Mechanics = new List<MechanicChartDataDto>();
    }
}
