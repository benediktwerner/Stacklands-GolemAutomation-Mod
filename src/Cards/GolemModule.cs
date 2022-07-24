namespace GolemAutomation
{
    abstract class GolemModule : CardData
    {
        public override bool CanHaveCard(CardData otherCard) => otherCard is GolemModule;

        public abstract bool CanInsert(Golem g);
        public abstract void Insert(Golem g);
    }
}
