using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Assets.Scripts.Card;
using Assets.Scripts.UI;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace Assets.Scripts.Player
{
    public class FourPlayer : NetworkBehaviour, IPlayer
    {
        private List<ICard> playedCards = new List<ICard>();
        private void Start()
        {
            Id = (int)netId.Value;
            Name = "Player " + Id;
            if (OnPlayerJoined != null)
                OnPlayerJoined(this);

            _organizer = new CardOrganizer(GetComponent<NetworkIdentity>().isLocalPlayer, 10, .5f, .45f, 20);

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
                    CmdSit(pos);
                    Camera.main.transform.rotation = Quaternion.LookRotation(Vector3.forward, -RuleHelpers.GetHandPosition(pos));
                }
            };

            Interactable.OnCardEvent += (type, card) =>
            {
                if (!_isMyPlayTurn || !isLocalPlayer || playedCards.Contains(card))
                    return;
                switch (type)
                {
                    case Interactable.CardEventType.MouseOver:
                        if (RuleHelpers.IsValidPlay(card, HandCards.Except(playedCards).ToList(), _currentRoundInfo))
                            card.Movable.Grow();
                        break;
                    case Interactable.CardEventType.MouseExit:
                        card.Movable.Shrink();
                        break;
                    case Interactable.CardEventType.MouseDown:
                        if (RuleHelpers.IsValidPlay(card, HandCards.Except(playedCards).ToList(), _currentRoundInfo))
                        {
                            _isMyPlayTurn = false;
                            CmdPickedCard(card.ID);
                        }
                        else
                        {
                            DebugConsole.Log(card + " is not a valid play");
                        }
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
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
            playedCards.Clear();
            RpcClearCards();
        }

        [ClientRpc]
        private void RpcClearCards()
        {
            playedCards.Clear();
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
            _organizer.OrganizeHandCards(HandCards, Position);
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

        public bool HasCard(ICard card)
        {
            return HandCards.Contains(card);
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
            if (bot)
            {
                CmdBid(minBid > 60 ? 0 : minBid);
                isMyTurnToBid = false;
                FindObjectOfType<BidGui>().Hide();
            }
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
            DebugConsole.Log("prompting " + Name + " to play a card");
            _playAction = playAction;
            RpcPlayCard(cardsPlayedInRound.Select(a => a.ID).ToArray(), playedSuit,
                trumpSuit);
        }

        private bool bot = true;

        private RuleHelpers.RoundInfo _currentRoundInfo;

        [ClientRpc]
        private void RpcPlayCard(int[] cardsPlayedInRound, Suit playedSuit, Suit trumpSuit)
        {
            if (!isLocalPlayer)
                return;
            DebugConsole.Log(Name + " received prompt to play a card");
            _isMyPlayTurn = true;
            _currentRoundInfo = new RuleHelpers.RoundInfo
            {
                CardsPlayedInRound = FindObjectsOfType<MBCard>().Where(a => cardsPlayedInRound.Contains(a.ID)).Select(a => (ICard)a).ToList(),
                PlayedSuit = playedSuit,
                TrumpSuit = trumpSuit
            };

            if (bot)
            {
                _isMyPlayTurn = false;
                CmdPickedCard(HandCards.First(a => RuleHelpers.IsValidPlay(a, HandCards.Except(playedCards).ToList(), _currentRoundInfo)).ID);
            }

        }

        [Command]
        private void CmdPickedCard(int id)
        {
            var card = HandCards.Single(a => a.ID == id);
            HandCards.Remove(card);
            _playAction(card, this);
            RpcPutCardOnTable(card.ID);
        }

        [ClientRpc]
        private void RpcPutCardOnTable(int id)
        {
            var card = FindObjectsOfType<MBCard>().Single(a => a.ID == id);
            playedCards.Add(card);
            _organizer.OrganizeHandCards(HandCards.Except(playedCards).ToList(), Position);
            card.Movable.MoveTo(RuleHelpers.GetHandPosition(Position) * .1f);
            card.Movable.Flip(FlipState.FaceUp, true);
        }
    }
}
