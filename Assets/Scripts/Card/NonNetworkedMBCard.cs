using Assets.Scripts.Initialization;
using UnityEngine;

namespace Assets.Scripts.Card
{
    [RequireComponent(typeof(IMovable))]
    public class NonNetworkedMBCard : MonoBehaviour, ICard
    {
        private ICard _wrappedCard;

        public void Initialize(Rank rank, Suit suit)
        {
            Rank = rank;
            Suit = suit;
            transform.FindChild("Front").GetComponent<SpriteRenderer>().sprite = ResourceFolderCardTextureProvider.GetCardTexture(Rank, Suit);
            Movable = GetComponent<IMovable>();
        }

        public delegate void CardClicked(ICard card);
        public static event CardClicked OnClicked;

        void OnMouseDown()
        {
            if (OnClicked != null)
                OnClicked(this);
        }

        void Start()
        {
            Movable = GetComponent<IMovable>();
        }
        

        public int ID { get { return ((int)Suit << 8) + (int)Rank; } }

        public Suit Suit { get; private set; }

        public Rank Rank { get; private set; }

        public IMovable Movable { get; private set; }

        public override string ToString()
        {
            return Rank + " Of " + Suit;
        }
    }
}