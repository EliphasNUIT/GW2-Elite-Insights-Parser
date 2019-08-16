﻿using System.Collections.Generic;

namespace GW2EIParser.Builders.HtmlModels
{  
    public class PhaseChartDataDto
    {    
        public List<PlayerChartDataDto> Players = new List<PlayerChartDataDto>();      
        public List<TargetChartDataDto> Targets = new List<TargetChartDataDto>();

        public List<double[]> TargetsHealthForCR = null;
    }
}
