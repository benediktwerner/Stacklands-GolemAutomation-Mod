using UnityEngine;

namespace GolemAutomation
{
    class Golem : HasFilter
    {
        public override bool DetermineCanHaveCardsWhenIsRoot => true;
        public override bool CanHaveCardsWhileHasStatus() => true;

        public override bool CanHaveCard(CardData otherCard)
        {
            if (MyGameCard.Child?.CardData is LocationGlyph g1)
            {
                if (g1.MyGameCard.Child?.CardData is LocationGlyph g2)
                {
                    return g2.MyGameCard.Child == null && otherCard.MyGameCard.Child == null;
                }
                return g1.MyGameCard.Child == null && otherCard is LocationGlyph && otherCard.MyGameCard.Child == null;
            }
            if (MyGameCard.Child != null) return false;
            if (otherCard.Id == Consts.FILTER) return true;
            return otherCard is LocationGlyph && (otherCard.MyGameCard.Child == null || (otherCard.MyGameCard.Child.CardData is LocationGlyph && otherCard.MyGameCard.Child.Child == null));
        }

        public override void UpdateCard()
        {
            if (MyGameCard.Parent == null && MyGameCard.Child?.CardData is LocationGlyph && MyGameCard.Child.Child?.CardData is LocationGlyph)
            {
                MyGameCard.StartTimer(5f, new TimerAction(Work), "Working", GetActionId(nameof(Work)));
            }
            else
            {
                MyGameCard.CancelTimer(GetActionId(nameof(Work)));
            }
            base.UpdateCard();
        }

        [TimedAction(Consts.GOLEM + ".work")]
        public void Work()
        {
            if (MyGameCard.Child?.CardData is LocationGlyph g1 && MyGameCard.Child.Child?.CardData is LocationGlyph g2)
            {
                var card = g2.MyGameCard.Child;
                if (card != null)
                {
                    if (g2.target != null && g2.target.MyBoard.IsCurrent)
                    {
                        var leaf = g2.target.GetLeafCard();
                        if (leaf.CardData.CanHaveCardOnTop(card.CardData))
                        {
                            card.RemoveFromStack();
                            card.BounceTarget = leaf;
                            var vec = leaf.transform.position - card.transform.position;
                            card.Velocity = new Vector3(vec.x * 4f, 7f, vec.z * 4f);
                        }
                    }
                    return;
                }

                if (g1.target != null && g1.target.MyBoard.IsCurrent && g1.target.Child != null)
                {
                    var leaf = g1.target.GetLeafCard();
                    if (filter.Count > 0 && !filter.Contains(leaf.CardData.Id)) return;
                    leaf.RemoveFromStack();
                    leaf.BounceTarget = g1.MyGameCard;
                    var vec = g1.MyGameCard.transform.position - leaf.transform.position;
                    leaf.Velocity = new Vector3(vec.x * 4f, 7f, vec.z * 4f);
                }
            }
        }
    }
}
