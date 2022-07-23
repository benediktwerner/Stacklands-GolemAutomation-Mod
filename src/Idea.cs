using System.Collections.Generic;

namespace GolemAutomation
{
    class Idea
    {
        public readonly string Name;
        public readonly BlueprintGroup Group;
        public readonly List<Subprint> Subprints;

        public Idea(string name, BlueprintGroup group, List<Subprint> subprints)
        {
            Name = name;
            Group = group;
            Subprints = subprints;
        }
    }
}
