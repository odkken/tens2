using System.Collections.Generic;
using Assets.Scripts.GameLogic;
using Assets.Scripts.Player;

namespace Assets.Scripts.Initialization
{
    public interface IHandFactory
    {
        IHand GetNewHand(List<IPlayer> players, int firstPlayerId, Dictionary<int, int> previousScores);
    }
}