using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Card;
using Assets.Scripts.Initialization;
using Assets.Scripts.Player;

namespace Assets.Scripts.GameLogic
{
    public class SimpleHand : IHand
    {
        private const int RoundLimit = 10;
        private readonly IBidManagerFactory _bidManagerFactory;
        private readonly List<IPlayer> _players;
        private readonly Dictionary<int, int> _playerScores = new Dictionary<int, int>();
        private readonly Dictionary<int, int> _previousPlayerScores;
        private readonly IRoundFactory _roundFactory;
        private bool _biddingPrompted;
        private IBidManager _bidManager;
        private Dictionary<int, int> _bids;
        private IRound _currentRound;
        private int _numRoundsPlayed;
        private int _startPlayerId;

        public SimpleHand(IRoundFactory roundFactory, List<IPlayer> players, Dictionary<int, int> previousPlayerScores,
            IBidManagerFactory bidManagerFactory, int startPlayerId)
        {
            _roundFactory = roundFactory;
            _players = players;
            _previousPlayerScores = previousPlayerScores;
            _bidManagerFactory = bidManagerFactory;
            _startPlayerId = startPlayerId;
            foreach (var player in players)
            {
                _playerScores.Add(player.Id, 0);
            }
            _currentRound = roundFactory.GetFirstRoundInHand(players, startPlayerId);
        }

        public void Tick()
        {
            if (IsHandFinished()) return;

            if (!_biddingPrompted)
            {
                _biddingPrompted = true;
                _bidManager = _bidManagerFactory.GetBidManager(_players, _startPlayerId, (ints =>
                {
                    _bids = ints;
                    _startPlayerId = _bids.Single(b => b.Value == _bids.Values.Max()).Key;
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