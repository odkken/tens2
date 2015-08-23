using System;
using UnityEngine;

namespace Assets.Scripts.Card
{
    class DummyMovable : IMovable
    {
        public void Orient(Vector3 forward)
        {

        }

        public void LockFlipState()
        {
        }

        public void Grow()
        {
            
        }

        public void Shrink()
        {
        }


        public void Tilt(float angle)
        {

        }

        public void Flip(FlipState state, bool localOnly)
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