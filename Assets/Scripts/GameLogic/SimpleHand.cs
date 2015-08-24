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
        private Dictionary<int, int> _previousTeamScores;
        private Dictionary<int, int> _teamScores;
        private readonly IRoundFactory _roundFactory;
        private bool _biddingPrompted;
        private IBidManager _bidManager;
        private Dictionary<int, int> _bids;
        private IRound _currentRound;
        private int _numRoundsPlayed;
        private int _startPlayerId;

        public SimpleHand(IRoundFactory roundFactory, List<IPlayer> players, IDeck deck, int team1PreviousScore, int team2PreviousScore,
            IBidManagerFactory bidManagerFactory, int startPlayerId)
        {
            _roundFactory = roundFactory;
            _players = players;
            _deck = deck;
            _previousTeamScores = new Dictionary<int, int> { { 1, team1PreviousScore }, { 2, team2PreviousScore } };
            _teamScores = new Dictionary<int, int> { { 1, 0 }, { 2, 0 } };
            _bidManagerFactory = bidManagerFactory;
            _startPlayerId = startPlayerId;

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
                    var t1prior = _previousTeamScores[1];
                    var t2prior = _previousTeamScores[2];

                    var t1ThisHand = _teamScores[1];
                    var t2ThisHand = _teamScores[2];
                    Debug.LogFormat("t1 prior: {0}, t2 prior: {1}", t1prior, t2prior);
                    Debug.LogFormat("t1 this Hand: {0}, t2 this Hand: {1}", t1ThisHand, t2ThisHand);

                    var heldBid = _bids.Values.Max();
                    var bidHoldingTeam = RuleHelpers.GetTeam(_players.Single(a => a.Id == _bids.Single(b => b.Value == heldBid).Key));
                    Debug.LogFormat("team {0} held the bid at {1}", bidHoldingTeam, heldBid);
                    var holdersScoreThisHand = _teamScores[bidHoldingTeam];
                    if (holdersScoreThisHand < heldBid)
                    {
                        Debug.LogFormat("Penalizing team {0} (bid holders) for not making their bid of {1}.  They made {2}", bidHoldingTeam, heldBid, holdersScoreThisHand);
                        var beforePenalty = _previousTeamScores[bidHoldingTeam];
                        _teamScores[bidHoldingTeam] = _previousTeamScores[bidHoldingTeam] - heldBid;
                        var afterPenalty = _teamScores[bidHoldingTeam];
                        Debug.LogFormat("subtracted {0} points from team {1}", beforePenalty - afterPenalty, bidHoldingTeam);
                    }
                    else
                    {
                        Debug.LogFormat("Awarding points to team {0} (bid holders) for making their bid of {1}.  They made {2}", bidHoldingTeam, heldBid, holdersScoreThisHand);
                        _teamScores[bidHoldingTeam] += _previousTeamScores[bidHoldingTeam];
                    }

                    var nonBidHOldingTeam = bidHoldingTeam == 1 ? 2 : 1;
                    var anyoneFromNonBidHoldingTeamBid =
                        _bids.Any(
                            a =>
                                RuleHelpers.GetTeam(_players.Single(b => b.Id == a.Key)) != bidHoldingTeam &&
                                a.Value > 0);
                    if (anyoneFromNonBidHoldingTeamBid || _previousTeamScores[nonBidHOldingTeam] < 150)
                    {
                        _teamScores[nonBidHOldingTeam] += _previousTeamScores[nonBidHOldingTeam];
                    }
                    else
                    {
                        _teamScores[nonBidHOldingTeam] = _previousTeamScores[nonBidHOldingTeam];
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

        public int GetPointsForTeam(int team)
        {
            return _teamScores[team];
        }

        private void TallyScores(IRound round)
        {
            var winner = round.GetWinner();
            var team = RuleHelpers.GetTeam(winner);
            _teamScores[team] += round.GetWinnersPoints();
        }
    }
}