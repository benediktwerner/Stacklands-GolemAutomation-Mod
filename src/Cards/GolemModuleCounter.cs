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
                    new TimerAction(SetCount),
                    "Setting count",
                    GetActionId(nameof(SetCount))
                );
            }
            else if (MyGameCard.TimerRunning)
            {
                MyGameCard.CancelTimer(GetActionId(nameof(SetCount)));
            }
            base.UpdateCard();
        }

        [TimedAction(Consts.GOLEM_MOD_COUNTER + ".set_count")]
        public void SetCount()
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
