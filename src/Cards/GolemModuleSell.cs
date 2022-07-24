namespace GolemAutomation
{
    class GolemModuleSell : GolemModule
    {
        public override bool CanInsert(Golem g) => !g.HasSellingModule;

        public override void Insert(Golem g)
        {
            g.HasSellingModule = true;
        }
    }
}
