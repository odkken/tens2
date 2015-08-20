using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Card;

namespace Assets.Code.GameSpecific.Tens
{
    public static class RuleHelpers
    {
        public static ICard GetWinningCard(List<ICard> playedCards, Suit trumpSuit, Suit playedSuit)
        {
            return playedCards.Any(a => a.Suit == trumpSuit)
                ? playedCards.Where(a => a.Suit == trumpSuit).OrderBy(a => a.Rank).Last()
                : playedCards.Where(a => a.Suit == playedSuit).OrderBy(a => a.Rank).Last();
        }

        public static int GetPointValue(List<ICard> cards)
        {
            var points = 0;
            foreach (var card in cards)
            {
                if (card.Rank == Rank.Five)
                    points += 5;
                if (card.Rank == Rank.Ten || card.Rank == Rank.Ace)
                    points += 10;
            }
            return points;
        }
    }
}