using System;
using System.Collections.Generic;
using Assets.Scripts.GameLogic;
using Assets.Scripts.Player;

namespace Assets.Scripts.Initialization
{
    public interface IBidManagerFactory
    {
        IBidManager GetBidManager(List<IPlayer> players, int firstPlayerToBid,
            Action<Dictionary<int, int>> onBidsCompletedAction);
    }
}