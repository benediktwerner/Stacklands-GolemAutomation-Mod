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
                if (
                    card.MyBoard.IsCurrent
                    && card.CardData is StoragePlace f
                    && card.Parent == null
                    && card.Child != null
                )
                {
                    Vector3 vec = card.transform.position - MyGameCard.transform.position;
                    vec.y = 0f;
                    var dist = vec.sqrMagnitude;
                    if (dist <= 9f && !card.BeingDragged)
                    {
                        result.Add(card);
                    }
                }
            }
            for (int i = 0; i < result.Count; i++)
            {
                var temp = result[i];
                int randomIndex = UnityEngine.Random.Range(i, result.Count);
                result[i] = result[randomIndex];
                result[randomIndex] = temp;
            }
            return result;
        }
    }
}
