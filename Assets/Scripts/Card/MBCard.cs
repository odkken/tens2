using Assets.Scripts.Initialization;
using UnityEngine;

namespace Assets.Scripts.Card
{
    [RequireComponent(typeof(MBMovable))]
    public class MBCard : MonoBehaviour, ICard
    {
        private ICard _wrappedCard;
        public void Initialize(ICard realCard, ICardTextureProvider textureProvider)
        {
            _wrappedCard = realCard;
            transform.FindChild("Front").GetComponent<SpriteRenderer>().sprite = textureProvider.GetCardTexture(Rank, Suit);
            Movable = GetComponent<MBMovable>();
        }

        public Suit Suit
        {
            get { return _wrappedCard.Suit; }
        }

        public Rank Rank
        {
            get { return _wrappedCard.Rank; }
        }

        public IMovable Movable { get; private set; }
    }
}