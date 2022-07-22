using System;
using System.Collections.Generic;
using System.Linq;

namespace GolemAutomation
{
    class Filter : CardData
    {
        public override bool DetermineCanHaveCardsWhenIsRoot => true;

        public override bool CanHaveCard(CardData otherCard) => true;

        [ExtraData(Consts.FILTER + ".filter")]
        public string filterData = "";
        public HashSet<string> filter = new HashSet<string>();

        public void Start()
        {
            if (!string.IsNullOrWhiteSpace(filterData))
            {
                filter = new HashSet<string>(filterData.Split(',').Where(x => WorldManager.instance.GameDataLoader.idToCard.ContainsKey(x)));
                UpdateDescription();
            }
        }

        public override void UpdateCard()
        {
            if (CanStartAction())
            {
                MyGameCard.StartTimer(1f, new TimerAction(AddFilter), "Adding to filter", GetActionId(nameof(AddFilter)));
            }
            else
            {
                MyGameCard.CancelTimer(GetActionId(nameof(AddFilter)));
            }
            base.UpdateCard();
        }

        public void UpdateDescription()
        {
            var array = filter.Select(x => WorldManager.instance.GameDataLoader.GetCardFromId(x).Name).ToArray();
            Array.Sort(array);
            descriptionOverride = string.Join(", ", array) + "\n\n" + "Use a villager to clear";
        }

        public bool CanStartAction()
        {
            var child = MyGameCard.Child;
            var hasNew = false;
            while (child != null)
            {
                if (child.CardData.MyCardType != CardType.Humans)
                    hasNew |= !filter.Contains(child.CardData.Id);
                child = child.Child;
            }
            return hasNew;
        }

        [TimedAction(Consts.FILTER + ".add_filter")]
        public void AddFilter()
        {
            var child = MyGameCard.Child;
            while (child != null)
            {
                if (child.CardData.MyCardType != CardType.Humans)
                {
                    filter.Add(child.CardData.Id);
                }
                child = child.Child;
            }

            UpdateDescription();
            filterData = string.Join(",", filter.ToArray());
        }
    }
}
