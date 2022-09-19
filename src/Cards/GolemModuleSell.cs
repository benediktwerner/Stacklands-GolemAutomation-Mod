namespace GolemAutomation
{
    class GolemModuleSell : GolemModule
    {
        public override bool CanInsert(Golem g) => !g.HasSellingModule && !g.HasRecipe;

        public override void Insert(Golem g)
        {
            g.HasSellingModule = true;
        }
    }
}
