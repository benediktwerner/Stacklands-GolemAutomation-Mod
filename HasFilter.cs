using System;
using System.Collections.Generic;
using System.Linq;

namespace GolemAutomation
{
    class HasFilter : CardData
    {
        [ExtraData(Consts.PREFIX + "filter")]
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

        public virtual void UpdateDescription()
        {
            if (filter.Count > 0)
            {
                var array = filter.Select(x => WorldManager.instance.GameDataLoader.GetCardFromId(x).Name).ToArray();
                Array.Sort(array);
                descriptionOverride = "Filter: " + string.Join(", ", array) + "\n\n" + "Use a different filter to override";
            }
            else
            {
                descriptionOverride = null;
            }
        }

        public override void UpdateCard()
        {
            if (MyGameCard.Parent == null && MyGameCard.Child?.CardData is Filter f && !f.filter.SetEquals(filter))
            {
                MyGameCard.StartTimer(1f, new TimerAction(UpdateFilter), "Applying filter", GetActionId(nameof(UpdateFilter)));
            }
            else
            {
                MyGameCard.CancelTimer(GetActionId(nameof(UpdateFilter)));
            }
            base.UpdateCard();
        }

        [TimedAction(Consts.STORAGE_PLACE + ".update_filter")]
        public void UpdateFilter()
        {
            if (MyGameCard.Child?.CardData is Filter f)
            {
                filter = new HashSet<string>(f.filter);
                filterData = string.Join(",", filter.ToArray());
                UpdateDescription();
            }
        }
    }
}
