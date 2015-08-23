using System;
using UnityEngine;

namespace Assets.Scripts.Card
{
    public enum FlipState
    {
        FaceUp,
        FaceDown
    }
    public interface IMovable
    {
        void Orient(Vector3 forward);
        void LockFlipState();
        void Grow();
        void Shrink();
        void Tilt(float angle);
        void Flip(FlipState state, bool localOnly);
        void MoveTo(Vector3 position, Action onFinishedMoving = null);
        bool IsMoving { get; }
        Vector3 CurrentPosition { get; }
    }
}