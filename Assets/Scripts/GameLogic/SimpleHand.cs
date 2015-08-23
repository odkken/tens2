using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Card;
using Assets.Scripts.Initialization;
using Assets.Scripts.Player;
using UnityEngine;

namespace Assets.Scripts.GameLogic
{
    public class SimpleHand : IHand
    {
        private const int RoundLimit = 10;
        private readonly IBidManagerFactory _bidManagerFactory;
        private readonly List<IPlayer> _players;
        private readonly IDeck _deck;
        private readonly Dictionary<int, int> _playerScores = new Dictionary<int, int>();
        private readonly Dictionary<int, int> _previousPlayerScores;
        private readonly IRoundFactory _roundFactory;
        private bool _biddingPrompted;
        private IBidManager _bidManager;
        private Dictionary<int, int> _bids;
        private IRound _currentRound;
        private int _numRoundsPlayed;
        private int _startPlayerId;

        public SimpleHand(IRoundFactory roundFactory, List<IPlayer> players, IDeck deck, Dictionary<int, int> previousPlayerScores,
            IBidManagerFactory bidManagerFactory, int startPlayerId)
        {
            _roundFactory = roundFactory;
            _players = players;
            _deck = deck;
            _previousPlayerScores = previousPlayerScores;
            _bidManagerFactory = bidManagerFactory;
            _startPlayerId = startPlayerId;

            foreach (var player in _players)
            {
                _playerScores.Add(player.Id, 0);
            }
            DealCards();

            DebugConsole.Log("cards dealt, starting new hand");
            DebugConsole.Log("Getting first round in hand...");
        }

        private void DealCards()
        {
            foreach (var player in _players)
            {
                player.ClearCards();
                var theirCards = new List<ICard>();
                while (theirCards.Count < 10)
                {
                    theirCards.Add(_deck.PopCard());
                }
                player.GiveCards(theirCards);
            }
        }


        public void Tick()
        {
            if (IsHandFinished()) return;

            if (!_biddingPrompted)
            {
                DebugConsole.Log("prompting bids from simplehand...");
                _biddingPrompted = true;
                _bidManager = _bidManagerFactory.GetBidManager(_players, _startPlayerId, (ints =>
                {
                    if (ints.All(a => a.Value == 0))
                    {
                        DebugConsole.Log("Everyone passed, re-dealing and bidding.");
                        _biddingPrompted = false;
                        DealCards();
                    }
                    else
                    {
                        _bids = ints;
                        _startPlayerId = _bids.Single(b => b.Value == _bids.Values.Max()).Key;
                        DebugConsole.Log(_players.Single(a => a.Id == _startPlayerId).Name + " held the bid");
                        _currentRound = _roundFactory.GetFirstRoundInHand(_players, _startPlayerId);
                    }
                }));
            }

            if (_bids == null)
            {
                _bidManager.Tick();
                return;
            }

            if (_currentRound.IsRoundFinished())
            {
                _numRoundsPlayed++;
                DebugConsole.Log("round " + _numRoundsPlayed + " finished");
                TallyScores(_currentRound);
                if (IsHandFinished())
                {
                    foreach (var player in _players)
                    {
                        var bid = _bids[player.Id];
                        var scoreThisHand = _playerScores[player.Id];
                        var previousPoints = _previousPlayerScores[player.Id];
                        var isBidHolder = bid == _bids.Keys.Max();
                        if (isBidHolder)
                        {
                            if (scoreThisHand < bid)
                                _playerScores[player.Id] = previousPoints - bid;
                            else
                                _playerScores[player.Id] += scoreThisHand;
                        }
                        else if (bid > 0 || previousPoints < 150)
                        {
                            _playerScores[player.Id] = previousPoints + scoreThisHand;
                        }
                    }
                    return;
                }
                if (_numRoundsPlayed == 1)
                    TrumpSuit = _currentRound.GetPlayedSuit();
                _currentRound = _roundFactory.GetNewRound(_players, _currentRound.GetWinner().Id, TrumpSuit);
            }
            _currentRound.Tick();
        }

        public IPlayer GetBidHolder()
        {
            return _players.Single(p => p.Id == _bids.Single(a => a.Value == _bids.Values.Max()).Key);
        }

        public bool IsHandFinished()
        {
            return _numRoundsPlayed == RoundLimit;
        }

        public bool IsTrumpDetermined { get; set; }
        public Suit TrumpSuit { get; set; }

        public int GetPointsForPlayer(int playerId)
        {
            return _playerScores[playerId];
        }

        private void TallyScores(IRound round)
        {
            _playerScores[round.GetWinner().Id] += round.GetWinnersPoints();
        }
    }
}