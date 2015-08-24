using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Card;
using Assets.Scripts.GameLogic;
using Assets.Scripts.UI;
using UnityEngine;
using UnityEngine.Networking;

namespace Assets.Scripts.Player
{
    public class FourPlayer : NetworkBehaviour, IPlayer
    {
        private List<ICard> _playedCards = new List<ICard>();
        private void Start()
        {
            Id = (int)netId.Value;
            Name = "Player " + Id;
            if (OnPlayerJoined != null)
                OnPlayerJoined(this);

            _organizer = new CardOrganizer(isLocalPlayer, 10, .5f, .45f, 0);

            BidGui.OnBidSubmitted += bid =>
            {
                if (!_isMyTurnToBid || !isLocalPlayer) return;
                CmdBid(bid);
                _isMyTurnToBid = false;
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
            FourPlayerGameManager.OnScoreUpdate += (i, score) =>
            {
                RpcGotScores(i, score);
            };
            Interactable.OnCardEvent += (type, card) =>
            {
                if (!_isMyPlayTurn || !isLocalPlayer || _playedCards.Contains(card))
                    return;
                switch (type)
                {
                    case Interactable.CardEventType.MouseOver:
                        if (RuleHelpers.IsValidPlay(card, HandCards.Except(_playedCards).ToList(), _currentRoundInfo))
                            card.Movable.Grow();
                        break;
                    case Interactable.CardEventType.MouseExit:
                        card.Movable.Shrink();
                        break;
                    case Interactable.CardEventType.MouseDown:
                        if (RuleHelpers.IsValidPlay(card, HandCards.Except(_playedCards).ToList(), _currentRoundInfo))
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



        [ClientRpc]
        void RpcGotScores(int team1Score, int team2Score)
        {
            if (!isLocalPlayer)
                return;
            FindObjectOfType<ScoreDisplay>().UpdateScores(team1Score, team2Score, RuleHelpers.GetTeam(this));
        }
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
            _playedCards.Clear();
            RpcClearCards();
        }

        [ClientRpc]
        private void RpcClearCards()
        {
            _playedCards.Clear();
            HandCards.Clear();
        }

        [ClientRpc]
        private void RpcGiveCards(int[] cardIds)
        {
            DebugConsole.Log(Name + " given cards RPC");
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
            foreach (var handCard in HandCards)
            {
                handCard.Movable.Shrink();
                handCard.Movable.MoveToInstant(Vector3.zero);
            }
            _organizer.OrganizeHandCards(HandCards, Position);
        }

        private Action<int, IPlayer> _bidAction;
        private bool _isMyTurnToBid;
        private int _minBid;

        public void GetNextBid(int minimum, string currentHolder, Action<int, IPlayer> onBidAction)
        {
            _bidAction = onBidAction;
            DebugConsole.Log(Name + " prompted for bid, asking them.");
            RpcPromptBid(minimum, currentHolder);
        }


        public bool HasCard(ICard card)
        {
            return HandCards.Contains(card);
        }

        [ClientRpc]
        private void RpcPromptBid(int minBid, string currentHolder)
        {
            if (!isLocalPlayer)
                return;
            DebugConsole.Log(Name + " received prompt to bid");
            _isMyTurnToBid = true;
            var bidGui = FindObjectOfType<BidGui>();
            bidGui.Show();
            bidGui.SetMinBid(minBid, currentHolder);
            if (_bot)
            {
                CmdBid(minBid > 60 ? 0 : minBid);
                _isMyTurnToBid = false;
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

        private bool _bot = true;

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

            if (_bot)
            {
                _isMyPlayTurn = false;
                CmdPickedCard(HandCards.First(a => RuleHelpers.IsValidPlay(a, HandCards.Except(_playedCards).ToList(), _currentRoundInfo)).ID);
            }

        }


        public void PutCardOnTable(ICard card)
        {
            RpcPutCardOnTable(card.ID);
        }

        public void TakeWonCards(List<ICard> cards)
        {
            RpcTakeWonCards(cards.Select(a => a.ID).ToArray());
        }

        [ClientRpc]
        private void RpcTakeWonCards(int[] ids)
        {
            var cards = FindObjectsOfType<MBCard>().Where(a => ids.Contains(a.ID));
            foreach (var mbCard in cards)
            {
                mbCard.Movable.MoveTo(RuleHelpers.GetHandPosition(Position) * 1, 1, 2, () => mbCard.Movable.Flip(FlipState.FaceDown));
            }
        }

        [Command]
        private void CmdPickedCard(int id)
        {
            var card = HandCards.Single(a => a.ID == id);
            _playedCards.Add(card);
            _playAction(card, this);
        }

        [ClientRpc]
        private void RpcPutCardOnTable(int id)
        {
            var card = HandCards.Single(a => a.ID == id);
            _playedCards.Add(card);
            _organizer.OrganizeHandCards(HandCards.Except(_playedCards).ToList(), Position);
            card.Movable.MoveTo(RuleHelpers.GetHandPosition(Position) * .1f, 0, .3f);
            card.Movable.Flip(FlipState.FaceUp);
        }
    }
}
