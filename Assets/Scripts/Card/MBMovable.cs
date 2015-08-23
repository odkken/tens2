using System;
using UnityEngine;
using UnityEngine.Networking;

namespace Assets.Scripts.Card
{
    class MBMovable : NetworkBehaviour, IMovable
    {
        private bool _isStateLocked;

        private Vector3 normalScale;
        void Awake()
        {
            normalScale = transform.localScale;
            Flip(FlipState.FaceDown, true);
            transform.rotation = Quaternion.LookRotation(Vector3.forward);
        }

        public void Orient(Vector3 forward)
        {
            switch (currentState)
            {
                case FlipState.FaceUp:
                    transform.rotation = Quaternion.LookRotation(Vector3.back, forward);
                    break;
                case FlipState.FaceDown:
                    transform.rotation = Quaternion.LookRotation(Vector3.forward, forward);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }


        public void MoveTo(Vector3 position, Action onFinishedMoving = null)
        {
            if (isServer)
                RpcMove(position);
            transform.position = position;
            if (onFinishedMoving != null)
                onFinishedMoving();
        }

        [ClientRpc]
        void RpcMove(Vector3 pos)
        {
            transform.position = pos;
        }


        public void LockFlipState()
        {
            _isStateLocked = true;
        }

        public void Grow()
        {
            transform.localScale = normalScale * 1.1f;
        }

        public void Shrink()
        {
            transform.localScale = normalScale;
        }

        public void Tilt(float angle)
        {
            transform.RotateAround(transform.position, -transform.forward, angle);
        }

        private FlipState currentState;

        public void Flip(FlipState state, bool localOnly)
        {
            if (!isServer)
                flipForReal(state);
            else
            {
                if (localOnly)
                    flipForReal(state);
                else
                    RpcFlip(state);
            }
        }

        [ClientRpc]
        void RpcFlip(FlipState state)
        {
            flipForReal(state);
        }

        private void flipForReal(FlipState state)
        {
            if (currentState == state)
                return;
            transform.RotateAround(transform.position, transform.up, 180);
            currentState = state;
        }


        void Update()
        {
            //if (_isStateLocked)
            //{
            //    transform.rotation = Quaternion.LookRotation(currentState == FlipState.FaceUp ? Vector3.forward : Vector3.back, transform.up);
            //}
        }


        public bool IsMoving { get; private set; }
        public Vector3 CurrentPosition { get { return transform.position; } }
    }
}