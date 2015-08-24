using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace Assets.Scripts.Card
{
    class MBMovable : MonoBehaviour, IMovable
    {
        private bool _isStateLocked;

        private Vector3 _normalScale;
        void Awake()
        {
            _normalScale = transform.localScale;
            Flip(FlipState.FaceDown);
            transform.rotation = Quaternion.LookRotation(Vector3.forward);
        }

        public void Orient(Vector3 forward)
        {
            switch (_currentState)
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

        public void LockFlipState()
        {
            _isStateLocked = true;
        }

        public void Grow()
        {
            transform.localScale = _normalScale * 1.1f;
        }

        public void Shrink()
        {
            transform.localScale = _normalScale;
        }

        public void Tilt(float angle)
        {
            transform.RotateAround(transform.position, transform.forward, angle);
        }

        private FlipState _currentState;

        public void Flip(FlipState state)
        {
            FlipForReal(state);
        }

        public void MoveTo(Vector3 position, float delay, float inSeconds, Action onFinishedMoving)
        {
            StartCoroutine(AnimateMove(position, delay, inSeconds, onFinishedMoving));
        }

        public void MoveToInstant(Vector3 position)
        {
            transform.position = position;
        }

        IEnumerator AnimateMove(Vector3 newPosition, float delay, float seconds, Action onFinishedAction)
        {
            _moving = true;
            var t0 = 0f;
            while (t0 < delay)
            {
                t0 += Time.deltaTime;
                yield return null;
            }

            var startPos = transform.position;
            var timeFraction = 0f;
            while (timeFraction < 1)
            {
                timeFraction += Time.deltaTime / seconds;
                transform.position = Vector3.Lerp(startPos, newPosition, Mathf.Log10(timeFraction * 9 + 1));
                yield return null;
            }
            transform.position = newPosition;
            _moving = false;
            if (onFinishedAction != null)
                onFinishedAction();
        }

        private bool _moving;

        //[ClientRpc]
        //void RpcFlip(FlipState state)
        //{
        //    FlipForReal(state);
        //}

        private void FlipForReal(FlipState state)
        {
            if (_currentState == state)
                return;
            transform.RotateAround(transform.position, transform.up, 180);
            _currentState = state;
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