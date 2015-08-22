using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Card;
using Assets.Scripts.Player;

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

        public struct RoundInfo
        {
            public List<CardInfo> CardsPlayedInRound;
            public Suit PlayedSuit;
            public Suit TrumpSuit;
        }

        public static bool IsValidPlay(ICard card, List<ICard> allRemainingCards, RoundInfo info)
        {
            return allRemainingCards.Contains(card) &&
                (!info.CardsPlayedInRound.Any() ||
                   card.Suit == info.PlayedSuit ||
                   allRemainingCards.All(a => a.Suit != info.PlayedSuit));
        }

        public static IPlayer GetNextPlayer(Position currentPlayerPosition, List<IPlayer> players)
        {
            var nextPos = GetNextPosition(currentPlayerPosition);
            while (players.All(a => a.Position != nextPos))
            {
                nextPos = GetNextPosition(nextPos);
            }
            return players.Single(a => a.Position == nextPos);
        }

        private static Position GetNextPosition(Position pos)
        {
            switch (pos)
            {
                case Position.North:
                    return Position.East;
                case Position.South:
                    return Position.West;
                case Position.East:
                    return Position.South;
                case Position.West:
                    return Position.North;
                default:
                    throw new ArgumentOutOfRangeException();
            }
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