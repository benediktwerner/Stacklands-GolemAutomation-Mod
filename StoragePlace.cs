namespace GolemAutomation
{
    class StoragePlace : HasFilter
    {
        public override bool DetermineCanHaveCardsWhenIsRoot => MyGameCard.Child?.CardData.DetermineCanHaveCardsWhenIsRoot ?? false;

        public override bool CanHaveCard(CardData otherCard) => MyGameCard.Child == null || MyGameCard.Child.CardData.CanHaveCard(otherCard);
    }
}
