using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Code.MonoBehavior.GameSpecific.Tens;
using Assets.Scripts.Card;
using UnityEngine;
using UnityEngine.Networking;

namespace Assets.Scripts.Player
{
    public class FourPlayer : NetworkBehaviour, IPlayer
    {
        void Start()
        {
            Id = (int)netId.Value;
            if (OnPlayerJoined != null)
                OnPlayerJoined(this);
            organizer = new CardOrganizer(Vector3.forward, GetComponent<NetworkIdentity>().isLocalPlayer, 10, 1);
        }

        void OnDestroy()
        {
            if (OnPlayerLeft != null)
                OnPlayerLeft(this);
        }

        public delegate void PlayerJoinOrLeave(IPlayer player);
        public static event PlayerJoinOrLeave OnPlayerJoined;
        public static event PlayerJoinOrLeave OnPlayerLeft;
        public List<ICard> HandCards = new List<ICard>();
        private CardOrganizer organizer;
        public int Id { get; private set; }
        public string Name { get; private set; }

        public void GiveCards(List<ICard> cards)
        {
            RpcGiveCards(cards.Select(a => a.ID).ToArray());
        }

        [ClientRpc]
        private void RpcGiveCards(int[] cardIds)
        {
            StartCoroutine(WaitForCardsToSpawn(cardIds));
        }

        private IEnumerator WaitForCardsToSpawn(int[] cardIds)
        {
            while (true)
            {
                Debug.Log("waiting for cards...");
                var spawnedIds = FindObjectsOfType<MBCard>().Select(a => a.ID);
                if (cardIds.Any(a => !spawnedIds.Contains(a)))
                    yield return null;
                break;
            }
            Debug.Log("all cards loaded");
            HandCards = new List<ICard>(FindObjectsOfType<MBCard>().Where(a => cardIds.Contains(a.ID)).Select(a => a.GetComponent<ICard>()));
            organizer.OrganizeHandCards(HandCards, Vector3.zero);
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
