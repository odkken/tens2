using System;
using UnityEngine;

namespace Assets.Scripts.Card
{
    class MBMovable : MonoBehaviour, IMovable
    {
        private FlipState _lockedState;
        private bool _isStateLocked;
        public void Orient(Vector3 forward)
        {
            transform.rotation = Quaternion.FromToRotation(transform.up, forward);
        }

        public void LockFlipState(FlipState state)
        {
            _lockedState = state;
            _isStateLocked = true;
        }

        public void Tilt(float angle)
        {
            transform.RotateAround(transform.position, transform.forward, angle);
        }

        public void Flip()
        {
            _isStateLocked = false;
            transform.RotateAround(transform.position, transform.up, 180);
        }

        void Update()
        {
            if (_isStateLocked)
            {
                transform.rotation = Quaternion.LookRotation(_lockedState == FlipState.FaceUp ? Vector3.forward : Vector3.back, transform.up);
            }
        }

        public void MoveTo(Vector3 position, Action onFinishedMoving = null)
        {
            transform.position = position;
            if (onFinishedMoving != null)
                onFinishedMoving();
        }

        public bool IsMoving { get; private set; }
        public Vector3 CurrentPosition { get { return transform.position; } }
    }
}