using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Misc;
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

        private IPlayer _lastBidder;
        public void Tick()
        {
            if (_biddingFinished || _waitingOnPlayer)
                return;

            var bidder = _lastBidder == null ? _players.Single(a => a.Id == _firstPlayerToBid) : _players.Where(a => !_playerBids.ContainsKey(a.Id) || _playerBids[a.Id] != 0).ToList().GetFrom(_lastBidder, 1);
            _waitingOnPlayer = true;
            DebugConsole.Log("Asking for bid from " + bidder.Name);
            IPlayer currentHolder = null;
            if (_playerBids.Any(a => a.Value != 0))
            {
                currentHolder = _players.Single(a => a.Id == _playerBids.Single(b => b.Value == _playerBids.Values.Max()).Key);
            }
            bidder.GetNextBid(_numBids == 0 ? 50 : Math.Max(_playerBids.Values.Max() + 5, 50), currentHolder != null ? currentHolder.Name : "Minimum", OnBid);
        }

        private void OnBid(int amount, IPlayer player)
        {
            _waitingOnPlayer = false;
            _lastBidder = player;
            _numBids++;

            if (!_playerBids.ContainsKey(player.Id))
                _playerBids.Add(player.Id, amount);
            else
            {
                _playerBids[player.Id] = amount;
            }
            if (_playerBids.Any(a => a.Value == 100)
                || (_playerBids.Count == _players.Count && _playerBids.Count(a => a.Value == 0) >= _players.Count - 1))
            {
                DebugConsole.Log("bidding finished");
                foreach (var playerBid in _playerBids)
                {
                    DebugConsole.Log(_players.Single(a => a.Id == playerBid.Key).Name + ": " + playerBid.Value);
                }
                _biddingFinished = true;
                _onBidsCompletedAction(_playerBids);
            }
        }
    }
}