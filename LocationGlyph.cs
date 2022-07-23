using UnityEngine;

namespace GolemAutomation
{
    class LocationGlyph : CardData
    {
        public override bool CanHaveCard(CardData otherCard)
        {
            return (otherCard.IsBuilding && otherCard is not Golem) || otherCard.Id == Id;
        }

        [ExtraData(Consts.LOCATION_GLYPH + ".target")]
        public string targetId;
        public GameCard target;

        public void Start()
        {
            if (!string.IsNullOrEmpty(targetId))
            {
                target = WorldManager.instance.GetCardWithUniqueId(targetId);
                UpdateDescription();
            }
        }
        public override void UpdateCard()
        {
            if (MyGameCard.Parent == null && MyGameCard.Child != null && target != MyGameCard.Child && MyGameCard.Child.CardData.Id != Id)
            {
                MyGameCard.StartTimer(1f, new TimerAction(Bind), "Binding to location", GetActionId(nameof(Bind)));
            }
            else
            {
                MyGameCard.CancelTimer(GetActionId(nameof(Bind)));
            }
            base.UpdateCard();
        }

        public void LateUpdate()
        {
            if ((WorldManager.instance.HoveredCard == MyGameCard || WorldManager.instance.DraggingCard == MyGameCard) && target != null)
            {
                target.HighlightRectangle.enabled = true;
                target.HighlightRectangle.Color = Color.cyan;
            }
        }

        [TimedAction(Consts.STORAGE_PLACE + ".bind")]
        public void Bind()
        {
            if (MyGameCard.Child != null)
            {
                target = MyGameCard.Child;
                targetId = target.CardData.UniqueId;
                UpdateDescription();
            }
        }

        public void UpdateDescription()
        {
            if (target != null)
            {
                descriptionOverride = "Bound to: " + target.CardData.Name + "\n\nUse a villager to unbind";
            }
            else
            {
                descriptionOverride = null;
            }
        }
    }
}
