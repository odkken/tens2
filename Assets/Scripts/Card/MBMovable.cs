using System;
using UnityEngine;

namespace Assets.Scripts.Card
{
    class MBMovable : MonoBehaviour, IMovable
    {
        private FlipState _lockedState;
        private bool _isStateLocked;
        public void LockFlipState(FlipState state)
        {
            _lockedState = state;
            _isStateLocked = true;
        }

        public void Flip()
        {
            _isStateLocked = false;
            transform.rotation = Quaternion.LookRotation(-transform.forward);
        }

        void Update()
        {
            if (_isStateLocked)
            {
                transform.rotation = Quaternion.LookRotation(_lockedState == FlipState.FaceUp ? Vector3.up : Vector3.down);
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