namespace GolemAutomation
{
    class StoragePlace : HasFilter
    {
        public override bool DetermineCanHaveCardsWhenIsRoot =>
            MyGameCard.Child?.CardData.DetermineCanHaveCardsWhenIsRoot ?? false;

        public override bool CanHaveCard(CardData otherCard)
        {
            if (MyGameCard.Child != null)
                return MyGameCard.Child.CardData.CanHaveCard(otherCard);

            return !Card.IsAnimal(otherCard);
        }
    }
}
