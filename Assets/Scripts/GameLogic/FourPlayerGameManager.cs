using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using Assets.Code.GameSpecific.Tens;
using Assets.Scripts.Initialization;
using Assets.Scripts.Player;
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
        private readonly Action<IPlayer> _onFinished;
        private Dictionary<int, int> _cumulativeScores;
        private IHand _currentHand;
        private bool _isGameWon;
        private IPlayer _winner;
        private int _numHandsPlayed;
        private bool playersSeated;
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
                Debug.LogFormat("player {0} joined", player.Id);
                _players.Add(player);
                if (_players.Count == 2)
                {
                    _firstPlayerToBid = _players.Shuffle(new Random()).First().Id;
                }
            };
            FourPlayer.OnPlayerLeft += player =>
            {

                Debug.LogFormat("player {0} left", player.Id);
                _players.Remove(player);
            };
        }


        void Update()
        {
            if (_isGameWon)
                return;

            if (!playersSeated)
                return;

            if (_currentHand == null)
            {
                _currentHand = _handFactory.GetNewHand(_players, _players.GetFrom(_players.Single(a => a.Id == _firstPlayerToBid), _numHandsPlayed).Id, _cumulativeScores);
            }

            if (_currentHand.IsHandFinished())
            {
                _numHandsPlayed++;
                foreach (var player in _players)
                {
                    _cumulativeScores[player.Id] = _currentHand.GetPointsForPlayer(player.Id);
                }

                if (_cumulativeScores.Count(a => a.Value >= 200) > 1)
                {
                    _isGameWon = true;
                    _winner = _currentHand.GetBidHolder();
                }
                else if (_cumulativeScores.Any(a => a.Value >= 200))
                {
                    _isGameWon = true;
                    _winner = _players.Single(a => a.Id == _cumulativeScores.Single(b => b.Value == _cumulativeScores.Max(c => c.Value)).Key);
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
