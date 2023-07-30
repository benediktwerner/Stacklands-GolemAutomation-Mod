using System.Collections.Generic;
using UnityEngine;

namespace GolemAutomation
{
    class AreaGlyph : Glyph
    {
        public override List<GameCard> FindTargets()
        {
            var result = new List<GameCard>();
            foreach (var card in WorldManager.instance.AllCards)
            {
                if (card.MyBoard.IsCurrent && card.Parent == null)
                {
                    Vector3 dist = card.transform.position - MyGameCard.transform.position;
                    dist.y = 0f;
                    if (dist.sqrMagnitude <= 9f && !card.BeingDragged)
                        result.Add(card);
                }
            }
            result.Shuffle();
            return result;
        }
    }
}
