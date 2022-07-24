namespace GolemAutomation
{
    class GolemModuleCounter : GolemModule
    {
        public override bool CanHaveCard(CardData otherCard)
        {
            return base.CanHaveCard(otherCard) || otherCard.Id == Card.Currency || otherCard is Villager;
        }

        public override bool CanInsert(Golem g) => Counter > 0 && g.Counter == 0;
        public override void Insert(Golem g)
        {
            g.Counter = Counter;
        }

        [ExtraData(Consts.GOLEM_MOD_COUNTER + ".counter")]
        public int Counter = 0;

        public void Start()
        {
            UpdateDescription();
        }

        public override void UpdateCard()
        {
            if (MyGameCard.Child?.CardData.Id == Card.Currency)
            {
                MyGameCard.StartTimer(10f, new TimerAction(IncreaseCount), "Increasing count", GetActionId(nameof(IncreaseCount)));
            }
            else if (MyGameCard.TimerRunning)
            {
                MyGameCard.CancelTimer(GetActionId(nameof(IncreaseCount)));
            }
            base.UpdateCard();
        }

        [TimedAction(Consts.GOLEM_MOD_COUNTER + ".increase_count")]
        public void IncreaseCount()
        {
            var child = MyGameCard.Child;
            var currency = Card.Currency;
            while (child != null)
            {
                if (child.CardData.Id == currency) Counter++;
                child = child.Child;
            }
            DestroyChildrenMatchingPredicateAndRestack(c => c.Id == currency, 100);
            UpdateDescription();
        }

        public void UpdateDescription()
        {
            descriptionOverride = "Count: " + Counter + "\n\nUse a villager to reset";
        }
    }
}
