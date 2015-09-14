using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Card;
using Assets.Scripts.GameLogic;
using Assets.Scripts.Misc;
using UnityEngine;

namespace Assets.Scripts.Player
{
    internal class CardOrganizer
    {
        private readonly bool _areMine;
        private readonly float _tableWidth;
        private readonly float _handWidth;
        private readonly float _handDistance;
        private readonly float _handTilt;

        public CardOrganizer(bool areMine, float tableWidth, float handWidth, float handDistance, float handTilt)
        {
            _areMine = areMine;
            _tableWidth = tableWidth;
            _handWidth = handWidth;
            _handDistance = handDistance;
            _handTilt = handTilt;
        }

        public void OrganizeTableCards(List<ICard> cards, Vector3 tablePosition)
        {
            //if (cards.Count != 10)
            //    DebugConsole.Log("Bad list passed in to organize");

            //var faceDown = cards.GetRange(0, 5);
            //var faceUp = cards.GetRange(5, 5);
            //for (int i = 0; i < 5; i++)
            //{
            //    var mover = faceDown[i].Movable;
            //    mover.LockFlipState(FlipState.FaceDown);
            //    mover.MoveTo(tablePosition + RelativeRight(_forward) * Mathf.Lerp(-_tableWidth * .5f, _tableWidth * .5f, i / 4f));
            //}
            //for (int i = 0; i < 5; i++)
            //{
            //    var mover = faceUp[i].Movable;
            //    mover.LockFlipState(FlipState.FaceUp);
            //    mover.MoveTo(tablePosition + RelativeRight(_forward) * Mathf.Lerp(-_tableWidth * .5f, _tableWidth * .5f, i / 4f));
            //}

        }

        public void OrganizeHandCards(List<ICard> cards, Position pos)
        {
            try
            {
                var suits = cards.GroupBy(a => a.Suit).OrderByDescending(a => a.First().Suit);
                List<ICard> sortedCards = new List<ICard>();
                foreach (var suit in suits)
                {
                    sortedCards.AddRange(suit.OrderByDescending(a => a.Rank));
                }
                var i = 0;
                var handPosition = RuleHelpers.GetHandPosition(pos);
                var spacing = _handWidth / (10 + cards.Count);
                foreach (var card in sortedCards)
                {
                    var mover = card.Movable;
                    mover.Orient(-handPosition);
                    var fraction = i * 1f / Math.Max(cards.Count - 1, 1);
                    var tiltAngle = Mathf.Lerp(-_handTilt, _handTilt, fraction);
                    mover.Tilt(tiltAngle);
                    mover.MoveTo(handPosition * _handDistance
                                 + RelativeRight(-handPosition) * Mathf.Lerp(-_handWidth * .5f, _handWidth * .5f, fraction)
                                 + new Vector3(0, 0, .1f * fraction)
                                 + handPosition * Math.Abs(tiltAngle) * .002f, 0, .5f);
                    mover.Flip(_areMine ? FlipState.FaceUp : FlipState.FaceDown);
                    i++;
                }
            }
            catch (Exception e)
            {
                DebugConsole.Log(e.ToString(), "red");
            }

        }

        private Vector3 RelativeRight(Vector3 forward)
        {
            return (Quaternion.Euler(0, 0, 90) * forward).normalized;
        }
    }
}