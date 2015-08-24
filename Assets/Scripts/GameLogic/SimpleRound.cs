using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Card;
using Assets.Scripts.Misc;
using Assets.Scripts.Player;
using UnityEngine;

namespace Assets.Scripts.GameLogic
{
    public class SimpleRound : IRound
    {
        private readonly Dictionary<int, ICard> _cardsPlayed;
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
            _cardsPlayed = new Dictionary<int, ICard>();
            foreach (var player in players)
            {
                _cardsPlayed.Add(player.Id, null);
            }
        }

        public SimpleRound(List<IPlayer> players, int startPlayerId)
        {
            _players = players;
            _startPlayerId = startPlayerId;
            _cardsPlayed = new Dictionary<int, ICard>();
            foreach (var player in players)
            {
                _cardsPlayed.Add(player.Id, null);
            }
        }

        private bool _cardsCleanedUp;

        public bool IsRoundFinished()
        {
            return _cardsPlayed.Values.All(a => a != null);
        }

        public void Tick()
        {
            if (IsRoundFinished() || _isWaitingOnPlayer) return;

            _isWaitingOnPlayer = true;
            var player = _players.GetFrom(_players.Single(a => a.Id == _startPlayerId),
                _cardsPlayed.Values.Count(a => a != null));
            DebugConsole.Log(player.Name + "'s turn to play");
            player.PlayCard(_cardsPlayed.Values.Where(a => a != null).ToList(), _playedSuit, _trumpSuit, OnPlayCardCallback);
        }

        private void CleanUpCards()
        {
            DebugConsole.Log("telling " + GetWinner().Name + " to clean up cards");
            GetWinner().TakeWonCards(_cardsPlayed.Values.ToList());
            _cardsCleanedUp = true;
        }

        public IPlayer GetWinner()
        {
            if (!IsRoundFinished())
                return null;
            var winningCard = RuleHelpers.GetWinningCard(_cardsPlayed.Values.ToList(), _trumpSuit, _playedSuit);
            return _players.Single(a => a.Id == _cardsPlayed.Single(b => b.Value == winningCard).Key);
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
            return RuleHelpers.GetPointValue(_cardsPlayed.Values.ToList());
        }

        private void OnPlayCardCallback(ICard card, IPlayer player)
        {
            player.PutCardOnTable(card);
            if (_cardsPlayed.Values.All(a => a == null))
            {
                _playedSuit = card.Suit;
                if (!_trumpDetermined)
                {
                    _trumpSuit = _playedSuit;
                    _trumpDetermined = true;
                }
            }
            _cardsPlayed[player.Id] = card;
            _isWaitingOnPlayer = false;
            if (IsRoundFinished() && !_cardsCleanedUp)
                CleanUpCards();
        }
    }
}