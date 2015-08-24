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

        public void Flip(FlipState state)
        {
        }

        public void MoveTo(Vector3 position, float delay, float inSeconds, Action onFinishedMoving)
        {
            throw new NotImplementedException();
        }

        public void MoveToInstant(Vector3 position)
        {
            throw new NotImplementedException();
        }

        public void MoveTo(Vector3 position)
        {
        }

        public bool IsMoving { get { return false; } }
        public Vector3 CurrentPosition { get; private set; }
    }
}