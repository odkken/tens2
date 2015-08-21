using System.Collections.Generic;
using System.Diagnostics;
using Assets.Scripts.Card;
using UnityEngine;

namespace Assets.Code.MonoBehavior.GameSpecific.Tens
{
    internal class CardOrganizer
    {
        private readonly Vector3 _forward;
        private readonly bool _areMine;
        private readonly float _tableWidth;
        private readonly float _handWidth;

        public CardOrganizer(Vector3 forward, bool areMine, float tableWidth, float handWidth)
        {
            _forward = forward;
            _areMine = areMine;
            _tableWidth = tableWidth;
            _handWidth = handWidth;
        }

        public void OrganizeTableCards(List<ICard> cards, Vector3 tablePosition)
        {
            if (cards.Count != 10)
                UnityEngine.Debug.LogError("Bad list passed in to organize");

            var faceDown = cards.GetRange(0, 5);
            var faceUp = cards.GetRange(5, 5);
            for (int i = 0; i < 5; i++)
            {
                var mover = faceDown[i].Movable;
                mover.LockFlipState(FlipState.FaceDown);
                mover.MoveTo(tablePosition + RelativeRight(_forward) * Mathf.Lerp(-_tableWidth * .5f, _tableWidth * .5f, i / 4f));
            }
            for (int i = 0; i < 5; i++)
            {
                var mover = faceUp[i].Movable;
                mover.LockFlipState(FlipState.FaceUp);
                mover.MoveTo(tablePosition + RelativeRight(_forward) * Mathf.Lerp(-_tableWidth * .5f, _tableWidth * .5f, i / 4f));
            }

        }

        public void OrganizeHandCards(List<ICard> cards, Vector3 handPosition)
        {
            if (cards.Count != 10)
                UnityEngine.Debug.LogError("Bad list passed in to organize");
            var i = 0;
            foreach (var card in cards)
            {
                var mover = card.Movable;
                mover.LockFlipState(_areMine ? FlipState.FaceUp : FlipState.FaceDown);
                mover.MoveTo(handPosition + RelativeRight(_forward) * Mathf.Lerp(-_handWidth * .5f, _handWidth * .5f, i / 9f));
                i++;
            }

        }

        private Vector3 RelativeRight(Vector3 forward)
        {
            return (Quaternion.FromToRotation(Vector3.forward, Vector3.right) * forward).normalized;
        }
    }
}