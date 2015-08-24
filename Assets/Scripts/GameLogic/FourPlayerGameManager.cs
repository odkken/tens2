using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using Assets.Scripts.Initialization;
using Assets.Scripts.Player;
using Assets.Scripts.UI;
using UnityEngine;
using Random = System.Random;

namespace Assets.Scripts.GameLogic
{
    public class FourPlayerGameManager : MonoBehaviour
    {
        public GameObject CardPrefab;
        private int _firstPlayerToBid;
        private IHandFactory _handFactory;
        private List<int> _shuffleSeeds;
        private readonly Action<int> _onFinished;
        private int team1CumulativeScore;
        private int team2CumulativeScore;
        private IHand _currentHand;
        private bool _isGameWon;
        private int _winner;
        private int _numHandsPlayed;
        private bool _playersSeated;
        private readonly List<IPlayer> _players = new List<IPlayer>();
        // Use this for initialization
        void Start()
        {
            var bytes = new byte[10];
            new RNGCryptoServiceProvider().GetBytes(bytes);
            _shuffleSeeds = bytes.Select(a => (int)a).ToList();


            var deckFactory = new DeckFactory(new MBCardFactory(CardPrefab));
            _handFactory = new HandFactory(deckFactory, _shuffleSeeds);

            FourPlayer.OnPlayerJoined += player =>
            {
                DebugConsole.Log(String.Format("player {0} joined", player.Id));
                _players.Add(player);

            };
            FourPlayer.OnPlayerLeft += player =>
            {

                DebugConsole.Log(String.Format("player {0} left", player.Id));
                _players.Remove(player);
            };

            SeatManager.OnAllPlayersSeated += () =>
            {
                DebugConsole.Log("All Players Seated, starting game.");
                _firstPlayerToBid = _players.Shuffle(new Random()).First().Id;
                _playersSeated = true;
            };
        }


        public delegate void ScoreUpdate(int team1Score, int team2Score);
        public static event ScoreUpdate OnScoreUpdate;


        private bool loggedFinish = false;
        private float finishTime;
        void Update()
        {
            if (_isGameWon)
                return;

            if (!_playersSeated)
                return;

            if (_currentHand == null)
            {
                DebugConsole.Log("Starting new hand...");
                _currentHand = _handFactory.GetNewHand(_players, _players.GetFrom(_players.Single(a => a.Id == _firstPlayerToBid), _numHandsPlayed).Id, team1CumulativeScore, team2CumulativeScore);
            }

            if (_currentHand.IsHandFinished())
            {
                if (!loggedFinish)
                {
                    finishTime = Time.time;
                    loggedFinish = true;
                }
            }
            if (loggedFinish && Time.time - finishTime > 5)
            {
                loggedFinish = false;
                _numHandsPlayed++;
                team1CumulativeScore = _currentHand.GetPointsForTeam(1);
                team2CumulativeScore = _currentHand.GetPointsForTeam(2);
                if (OnScoreUpdate != null)
                    OnScoreUpdate(team1CumulativeScore, team2CumulativeScore);
                if (team1CumulativeScore >= 200 || team2CumulativeScore >= 200)
                {
                    _isGameWon = true;
                    if (team1CumulativeScore >= 200 && team2CumulativeScore >= 200)
                        _winner = RuleHelpers.GetTeam(_currentHand.GetBidHolder());
                    else
                    {
                        _winner = team1CumulativeScore > team2CumulativeScore ? 1 : 2;
                    }
                    DebugConsole.Log(_winner + " won");
                }
                else
                {
                    _currentHand = null;
                }
                if (_isGameWon)
                    _onFinished(_winner);
                return;
            }
            _currentHand.Tick();
        }
    }
}
