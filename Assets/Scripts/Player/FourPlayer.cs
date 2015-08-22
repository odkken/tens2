using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Code.GameSpecific.Tens;
using Assets.Code.MonoBehavior.GameSpecific.Tens;
using Assets.Scripts.Card;
using Assets.Scripts.UI;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace Assets.Scripts.Player
{
    public class FourPlayer : NetworkBehaviour, IPlayer
    {
        private void Start()
        {
            Id = (int)netId.Value;
            Name = "Player " + Id;
            if (OnPlayerJoined != null)
                OnPlayerJoined(this);

            MBCard.OnClicked += card =>
            {
                if (!isLocalPlayer || !_isMyPlayTurn || !RuleHelpers.IsValidPlay(card, HandCards, _currentRoundInfo)) return;
                CmdPickedCard(card.Suit, card.Rank);
                _isMyPlayTurn = false;
            };

            BidGui.OnBidSubmitted += bid =>
            {
                if (!isMyTurnToBid || !isLocalPlayer) return;
                CmdBid(bid);
                isMyTurnToBid = false;
                FindObjectOfType<BidGui>().Hide();
            };

            SeatManager.OnClickedSit += pos =>
            {
                if (isLocalPlayer)
                {
                    IsSeated = true;
                    _position = (int)pos;
                    _organizer = new CardOrganizer(pos, GetComponent<NetworkIdentity>().isLocalPlayer, 10, 1);
                    CmdSit(pos);
                }
            };
        }

        private void OnDestroy()
        {
            if (OnPlayerLeft != null)
                OnPlayerLeft(this);
        }

        public delegate void PlayerJoinOrLeave(IPlayer player);

        public static event PlayerJoinOrLeave OnPlayerJoined;
        public static event PlayerJoinOrLeave OnPlayerLeft;
        public List<ICard> HandCards = new List<ICard>();

        private bool _isMyPlayTurn;
        private CardOrganizer _organizer;
        public int Id { get; private set; }
        public string Name { get; private set; }

        [Command]
        void CmdSetName(string name)
        {
            Name = name;
            RpcSetName(name);
        }

        [ClientRpc]
        void RpcSetName(string name)
        {
            Name = name;
        }

        public void GiveCards(List<ICard> cards)
        {
            RpcGiveCards(cards.Select(a => a.ID).ToArray());
        }

        public void ClearCards()
        {
            HandCards.Clear();
            RpcClearCards();
        }

        [ClientRpc]
        private void RpcClearCards()
        {
            HandCards.Clear();
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
                DebugConsole.Log("waiting for cards...");
                var spawnedIds = FindObjectsOfType<MBCard>().Select(a => a.ID);
                if (cardIds.Any(a => !spawnedIds.Contains(a)))
                    yield return null;
                break;
            }
            DebugConsole.Log("all cards loaded");
            HandCards =
                new List<ICard>(
                    FindObjectsOfType<MBCard>().Where(a => cardIds.Contains(a.ID)).Select(a => a.GetComponent<ICard>()));
            _organizer.OrganizeHandCards(HandCards);
        }

        private Action<int, IPlayer> _bidAction;
        private bool isMyTurnToBid;
        private int minBid;

        public void GetNextBid(int minimum, Action<int, IPlayer> onBidAction)
        {
            _bidAction = onBidAction;
            DebugConsole.Log(Name + " prompted for bid, asking them.");
            RpcPromptBid(minimum);
        }

        [ClientRpc]
        private void RpcPromptBid(int minBid)
        {
            if (!isLocalPlayer)
                return;
            DebugConsole.Log(Name + " received prompt to bid");
            isMyTurnToBid = true;
            var bidGui = FindObjectOfType<BidGui>();
            bidGui.Show();
            bidGui.SetMinBid(minBid);
        }

        [Command]
        private void CmdBid(int amount)
        {
            _bidAction(amount, this);
        }

        [Command]
        void CmdSit(Position pos)
        {
            Seat(pos);
        }

        public void Seat(Position position)
        {
            Position = position;
            IsSeated = true;
            RpcSit(Position);
        }

        [ClientRpc]
        void RpcSit(Position pos)
        {
            Position = pos;
            IsSeated = true;
        }

        [SyncVar]
        private int _position;

        [SyncVar]
        public bool IsSeated;

        public Position Position
        {
            get { return (Position)_position; }
            private set { _position = (int)value; }
        }

        private Action<ICard, IPlayer> _playAction;

        public void PlayCard(List<ICard> cardsPlayedInRound, Suit playedSuit, Suit trumpSuit,
            Action<ICard, IPlayer> playAction)
        {
            _playAction = playAction;
            RpcPlayCard(cardsPlayedInRound.Select(a => new CardInfo { Suit = a.Suit, Rank = a.Rank }).ToList(), playedSuit,
                trumpSuit);
        }


        private RuleHelpers.RoundInfo _currentRoundInfo;

        [ClientRpc]
        private void RpcPlayCard(List<CardInfo> cardsPlayedInRound, Suit playedSuit, Suit trumpSuit)
        {
            _isMyPlayTurn = true;
            _currentRoundInfo = new RuleHelpers.RoundInfo
            {
                CardsPlayedInRound = cardsPlayedInRound,
                PlayedSuit = playedSuit,
                TrumpSuit = trumpSuit
            };
        }

        [Command]
        private void CmdPickedCard(Suit suit, Rank rank)
        {
            var card = HandCards.Single(a => a.Suit == suit && a.Rank == rank);
            HandCards.Remove(card);
            _playAction(FindObjectsOfType<MBCard>().Single(a => a.Suit == suit && a.Rank == rank), this);
        }
    }
}
