using UnityEngine;

namespace Assets.Scripts.Card
{
    public class Interactable : MonoBehaviour
    {
        public enum CardEventType
        {
            MouseOver,
            MouseExit,
            MouseDown,
        }

        public delegate void CardEvent(CardEventType type, ICard card);

        public static event CardEvent OnCardEvent;

        void OnMouseDown()
        {
            NotifyCardEvent(CardEventType.MouseDown);
        }

        private ICard card;
        void Awake()
        {
            card = GetComponent<ICard>();
            _orignialScale = transform.localScale;
        }
        private Vector3 _orignialScale;
        void OnMouseOver()
        {
            NotifyCardEvent(CardEventType.MouseOver);
        }

        void OnMouseExit()
        {
            NotifyCardEvent(CardEventType.MouseExit);
        }

        private void NotifyCardEvent(CardEventType type)
        {
            if (OnCardEvent != null) OnCardEvent(type, card);
        }
    }
}
