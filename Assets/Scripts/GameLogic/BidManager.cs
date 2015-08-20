using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Player;

namespace Assets.Scripts.GameLogic
{
    public class BidManager : IBidManager
    {
        private readonly int _firstPlayerToBid;
        private readonly Action<Dictionary<int, int>> _onBidsCompletedAction;
        private readonly Dictionary<int, int> _playerBids = new Dictionary<int, int>();
        private readonly List<IPlayer> _players;
        private bool _biddingFinished;
        private int _numBids;
        private bool _waitingOnPlayer;

        public BidManager(List<IPlayer> players, int firstPlayerToBid,
            Action<Dictionary<int, int>> onBidsCompletedAction)
        {
            _players = players;
            _firstPlayerToBid = firstPlayerToBid;
            _onBidsCompletedAction = onBidsCompletedAction;
        }

        public void Tick()
        {
            if (_biddingFinished || _waitingOnPlayer)
                return;
            var bidder = _players.GetFrom(_players.Single(a => a.Id == _firstPlayerToBid), _numBids);
            _waitingOnPlayer = true;
            bidder.GetNextBid(_numBids == 0 ? 50 : _playerBids.Values.Max() + 5, OnBid);
        }

        private void OnBid(int amount, IPlayer player)
        {
            _waitingOnPlayer = false;
            _numBids++;
            if (amount == 0 && (_playerBids.Any()))
            {
                _biddingFinished = true;
                _onBidsCompletedAction(_playerBids);
            }
            else
            {
                if (!_playerBids.ContainsKey(player.Id))
                    _playerBids.Add(player.Id, amount);
                else
                {
                    _playerBids[player.Id] = amount;
                }
            }
        }
    }
}