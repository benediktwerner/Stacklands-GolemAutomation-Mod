namespace GolemAutomation
{
    class GolemModuleSpeed : GolemModule
    {
        public override bool CanInsert(Golem g) => true;

        public override void Insert(Golem g)
        {
            g.SpeedModules++;
            g.SpeedModifier /= 2f;
        }
    }
}
