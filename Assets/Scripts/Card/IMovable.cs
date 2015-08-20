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
        void LockFlipState(FlipState state);
        void Flip();
        void MoveTo(Vector3 position, Action onFinishedMoving = null);
        bool IsMoving { get; }
        Vector3 CurrentPosition { get; }
    }
}