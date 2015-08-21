using System;
using System.Collections.Generic;
using Assets.Scripts.Card;

namespace Assets.Scripts.Player
{
    public enum Position
    {
        North,
        South,
        East,
        West
    }

    public interface IPlayer
    {
        /// <summary>
        ///     Guaranteed to be unique among players in a game
        /// </summary>
        int Id { get; }

        string Name { get; }

        /// <summary>
        ///     Get the player's bid
        /// </summary>
        /// <returns>Value between minimum-100 for a valid bid.  0 if pass</returns>
        void GetNextBid(int minimum, Action<int, IPlayer> onBidAction);

        void Seat(Position position);
        Position Position { get; }
        void PlayCard(List<ICard> cardsPlayedInRound, Suit playedSuit, Suit trumpSuit, Action<ICard, IPlayer> playAction);
        void GiveCards(List<ICard> cards);
    }
}