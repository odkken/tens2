using System;
using System.Collections.Generic;
using Assets.Scripts.GameLogic;
using Assets.Scripts.Player;

namespace Assets.Scripts.Initialization
{
    internal class BidManagerFactory : IBidManagerFactory
    {
        public IBidManager GetBidManager(List<IPlayer> players, int firstPlayerToBid,
            Action<Dictionary<int, int>> onBidsCompletedAction)
        {
            return new BidManager(players, firstPlayerToBid, onBidsCompletedAction);
        }
    }
}