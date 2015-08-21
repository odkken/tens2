using System;
using UnityEngine;

namespace Assets.Scripts.Card
{
    class DummyMovable : IMovable
    {
        public void LockFlipState(FlipState state)
        {

        }

        public void Tilt(float angle)
        {

        }

        public void Flip()
        {

        }

        public void MoveTo(Vector3 position, Action onFinishedMoving = null)
        {
            if (onFinishedMoving != null)
                onFinishedMoving();
        }

        public bool IsMoving { get { return false; } }
        public Vector3 CurrentPosition { get; private set; }
    }
}