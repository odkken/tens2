using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using Assets.Scripts.Card;
using Assets.Scripts.Player;
using UnityEngine;

namespace Assets.Scripts.GameLogic
{
    public class SimpleRound : IRound
    {
        private readonly Dictionary<int, List<ICard>> _cardsPlayed;
        private readonly List<IPlayer> _players;
        private readonly int _startPlayerId;
        private bool _isWaitingOnPlayer;
        private Suit _playedSuit;
        private bool _trumpDetermined;
        private Suit _trumpSuit;

        public SimpleRound(List<IPlayer> players, int startPlayerId, Suit trumpSuit)
        {
            _players = players;
            _startPlayerId = startPlayerId;
            _trumpSuit = trumpSuit;
            _trumpDetermined = true;
            _cardsPlayed = new Dictionary<int, List<ICard>>();
            foreach (var player in players)
            {
                _cardsPlayed.Add(player.Id, new List<ICard>());
            }
        }

        public SimpleRound(List<IPlayer> players, int startPlayerId)
        {
            _players = players;
            _startPlayerId = startPlayerId;
            _cardsPlayed = new Dictionary<int, List<ICard>>();
            foreach (var player in players)
            {
                _cardsPlayed.Add(player.Id, new List<ICard>());
            }
        }

        private bool cardsCleanedUp = false;

        public bool IsRoundFinished()
        {
            return _cardsPlayed.Values.SelectMany(a => a).Count() == 4;
        }

        public void Tick()
        {
            if (IsRoundFinished() && !cardsCleanedUp)
                CleanUpCards();
            if (IsRoundFinished() || _isWaitingOnPlayer) return;

            _isWaitingOnPlayer = true;
            var player = _players.GetFrom(_players.Single(a => a.Id == _startPlayerId),
                _cardsPlayed.Values.SelectMany(a => a).Count());
            DebugConsole.Log(player.Name + "'s turn to play");
            player.PlayCard(_cardsPlayed.Values.SelectMany(a => a).ToList(), _playedSuit, _trumpSuit, OnPlayCardCallback);
        }

        private void CleanUpCards()
        {
            foreach (var card in _cardsPlayed.SelectMany(a => a.Value))
            {
                card.Movable.MoveTo(new Vector3(50000, 0, 0));
            }
            cardsCleanedUp = true;
        }

        public IPlayer GetWinner()
        {
            var winningCard = RuleHelpers.GetWinningCard(_cardsPlayed.Values.SelectMany(a => a).ToList(), _trumpSuit,
                _playedSuit);
            return _players.Single(a => a.Id == _cardsPlayed.Single(b => b.Value.Contains(winningCard)).Key);
        }

        public bool HaveCardsBeenPlayed()
        {
            return _cardsPlayed.Any();
        }

        public Suit GetPlayedSuit()
        {
            return _playedSuit;
        }

        public int GetWinnersPoints()
        {
            return RuleHelpers.GetPointValue(_cardsPlayed.Values.SelectMany(a => a).ToList());
        }

        private void OnPlayCardCallback(ICard card, IPlayer player)
        {
            if (!_cardsPlayed.Values.SelectMany(a => a).Any())
            {
                _playedSuit = card.Suit;
                if (!_trumpDetermined)
                {
                    _trumpSuit = _playedSuit;
                    _trumpDetermined = true;
                }
            }
            _cardsPlayed[player.Id].Add(card);
            _isWaitingOnPlayer = false;
            card.Movable.MoveTo(RuleHelpers.GetHandPosition(player.Position) * .1f);
            card.Movable.Flip(FlipState.FaceUp, true);
        }
    }
}