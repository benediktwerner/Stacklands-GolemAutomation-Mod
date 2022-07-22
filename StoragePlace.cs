namespace GolemAutomation
{
    class StoragePlace : HasFilter
    {
        public override bool CanHaveCard(CardData otherCard) => true;
    }
}
