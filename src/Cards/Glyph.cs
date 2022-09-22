using System.Collections.Generic;

namespace GolemAutomation
{
    abstract class Glyph : Resource
    {
        public abstract List<GameCard> FindTargets();
    }
}
