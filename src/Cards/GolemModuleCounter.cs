namespace GolemAutomation
{
    class GolemModuleCounter : GolemModule
    {
        public override bool CanHaveCard(CardData otherCard)
        {
            return base.CanHaveCard(otherCard) || otherCard.Id == Card.Currency;
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
            if (
                MyGameCard.Child?.CardData.Id == Card.Currency
                && MyGameCard.GetChildCount() != Counter
            )
            {
                MyGameCard.StartTimer(
                    10f,
                    new TimerAction(IncreaseCount),
                    "Increasing count",
                    GetActionId(nameof(IncreaseCount))
                );
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
            Counter = MyGameCard.GetChildCount();
            UpdateDescription();
        }

        public void UpdateDescription()
        {
            descriptionOverride = "Count: " + Counter + "\n\nUse a villager to reset";
        }
    }
}
