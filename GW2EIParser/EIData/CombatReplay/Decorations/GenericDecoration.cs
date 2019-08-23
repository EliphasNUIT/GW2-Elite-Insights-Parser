using GW2EIParser.Parser;
using static GW2EIParser.Builders.JsonModels.JsonCombatReplayDecorations;

namespace GW2EIParser.EIData
{
    public abstract class GenericDecoration
    {    
        public (int start, int end) Lifespan { get; }
        protected Connector ConnectedTo;
        
        protected GenericDecoration((int start, int end) lifespan, Connector connector)
        {
            Lifespan = lifespan;
            ConnectedTo = connector;
        }
        //

        public abstract JsonCombatReplayGenericDecoration GetCombatReplayJSON(CombatReplayMap map, ParsedLog log);

    }
}
