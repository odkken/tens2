using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Assets.Scripts.Card;
using Assets.Scripts.Player;
using UnityEngine;

namespace Assets.Code.MonoBehavior.GameSpecific.Tens
{
    internal class CardOrganizer
    {
        private readonly bool _areMine;
        private readonly float _tableWidth;
        private readonly float _handWidth;

        public CardOrganizer(bool areMine, float tableWidth, float handWidth)
        {
            _areMine = areMine;
            _tableWidth = tableWidth;
            _handWidth = handWidth;
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


        private static Vector3 GetHandPosition(Position pos)
        {
            switch (pos)
            {
                case Position.North:
                    return Vector3.up;
                case Position.South:
                    return Vector3.down;
                case Position.East:
                    return Vector3.right;
                case Position.West:
                    return Vector3.left;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        public void OrganizeHandCards(List<ICard> cards, Position pos)
        {
            var suits = cards.GroupBy(a => a.Suit).OrderByDescending(a => a.First().Suit);
            List<ICard> sortedCards = new List<ICard>();
            foreach (var suit in suits)
            {
                sortedCards.AddRange(suit.OrderByDescending(a => a.Rank));
            }
            if (sortedCards.Count != 10)
                DebugConsole.Log("Bad list passed in to organize");
            var i = 0;
            var handPosition = GetHandPosition(pos);
            foreach (var card in sortedCards)
            {
                var mover = card.Movable;
                mover.Orient(-handPosition);
                if (_areMine)
                    mover.Flip();
                var fraction = i / 9f;
                //mover.Tilt(Mathf.Lerp(-20, 20, fraction));
                mover.MoveTo(handPosition * .4f + RelativeRight(-handPosition) * Mathf.Lerp(-_handWidth * .5f, _handWidth * .5f, fraction) + new Vector3(0, 0, .1f * fraction));
                i++;
            }

        }

        private Vector3 RelativeRight(Vector3 forward)
        {
            return (Quaternion.Euler(0, 0, 90) * forward).normalized;
        }
    }
}