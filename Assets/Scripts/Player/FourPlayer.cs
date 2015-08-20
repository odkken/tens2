using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Card;
using UnityEngine.Networking;

namespace Assets.Scripts.Player
{
    public class FourPlayer : NetworkBehaviour, IPlayer
    {
        public class CardSyncList : SyncListStruct<CardInfo>
        {

        }

        void Start()
        {
            Id = (int)netId.Value;
            if (OnPlayerJoined != null)
                OnPlayerJoined(this);
        }

        void OnDestroy()
        {
            if (OnPlayerLeft != null)
                OnPlayerLeft(this);
        }

        public delegate void PlayerJoinOrLeave(IPlayer player);

        public static event PlayerJoinOrLeave OnPlayerJoined;
        public static event PlayerJoinOrLeave OnPlayerLeft;

        public CardSyncList HandCards = new CardSyncList();
        public int Id { get; private set; }
        public string Name { get; private set; }

        public void Initialize(int id, string playerName)
        {
            Id = id;
            Name = playerName;
        }

        public void GiveCards(List<CardInfo> cards)
        {
            foreach (var cardInfo in cards)
            {
                HandCards.Add(cardInfo);
            }
        }

        public void GetNextBid(int minimum, Action<int, IPlayer> onBidAction)
        {
        }

        private Action<ICard, IPlayer> _playAction;
        public void PlayCard(List<ICard> cardsPlayedInRound, Suit playedSuit, Suit trumpSuit, Action<ICard, IPlayer> playAction)
        {
            _playAction = playAction;
            RpcPlayCard(cardsPlayedInRound.Select(a => new CardInfo { Suit = a.Suit, Rank = a.Rank }).ToList(), playedSuit, trumpSuit);
        }

        public void GiveCard(ICard card)
        {
            HandCards.Add(new CardInfo { Rank = card.Rank, Suit = card.Suit });
        }

        [ClientRpc]
        private void RpcPlayCard(List<CardInfo> cardsPlayedInRound, Suit playedSuit, Suit trumpSuit)
        {
            var a = HandCards.First();
            CmdPickedCard(a.Suit, a.Rank);
        }

        [Command]
        void CmdPickedCard(Suit suit, Rank rank)
        {
            var card = HandCards.Single(a => a.Suit == suit && a.Rank == rank);
            HandCards.Remove(card);
            _playAction(FindObjectsOfType<MBCard>().Single(a => a.Suit == suit && a.Rank == rank), this);
        }
    }
}
