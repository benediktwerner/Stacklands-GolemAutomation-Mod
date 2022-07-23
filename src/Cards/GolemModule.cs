namespace GolemAutomation
{
    class GolemModule : CardData
    {
        public override bool CanHaveCard(CardData otherCard) => otherCard is GolemModule;

        public GolemModuleType ModType;

        public bool CanInsert(Golem g)
        {
            return ModType switch
            {
                GolemModuleType.Sell => !g.HasSellingModule,
                GolemModuleType.Speed => true,
                _ => false
            };
        }

        public void Insert(Golem g)
        {
            switch (ModType)
            {
                case GolemModuleType.Sell:
                    g.HasSellingModule = true;
                    return;
                case GolemModuleType.Speed:
                    g.SpeedModules += 1;
                    g.SpeedModifier /= 2;
                    return;
            }
        }
    }

    public enum GolemModuleType
    {
        Sell,
        Speed,
    }
}
