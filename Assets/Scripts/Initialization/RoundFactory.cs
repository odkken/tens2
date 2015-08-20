using System.Collections.Generic;
using Assets.Scripts.Card;
using Assets.Scripts.GameLogic;
using Assets.Scripts.Player;

namespace Assets.Scripts.Initialization
{
    public class RoundFactory : IRoundFactory
    {
        public IRound GetFirstRoundInHand(List<IPlayer> players, int startPlayerId)
        {
            return new SimpleRound(players, startPlayerId);
        }

        public IRound GetNewRound(List<IPlayer> players, int startPlayerId, Suit trumpSuit)
        {
            return new SimpleRound(players, startPlayerId, trumpSuit);
        }
    }
}