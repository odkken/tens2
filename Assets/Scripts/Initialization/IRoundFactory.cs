using System.Collections.Generic;
using Assets.Scripts.Card;
using Assets.Scripts.GameLogic;
using Assets.Scripts.Player;

namespace Assets.Scripts.Initialization
{
    public interface IRoundFactory
    {
        IRound GetFirstRoundInHand(List<IPlayer> players, int startPlayerId);
        IRound GetNewRound(List<IPlayer> players, int startPlayerId, Suit trumpSuit);
    }
}