using Assets.Scripts.Initialization;
using UnityEngine;
using UnityEngine.Networking;

namespace Assets.Scripts.Card
{
    [RequireComponent(typeof(IMovable))]
    public class MBCard : NetworkBehaviour, ICard
    {
        private ICard _wrappedCard;

        public void Initialize(Rank rank, Suit suit)
        {
            Rank = rank;
            Suit = suit;
            RpcInitialize(suit, rank);
        }

        void Start()
        {
            Movable = GetComponent<IMovable>();
        }

        [ClientRpc]
        private void RpcInitialize(Suit suit, Rank rank)
        {
            Rank = rank;
            Suit = suit;
            Movable = GetComponent<IMovable>();
            transform.FindChild("Front").GetComponent<SpriteRenderer>().sprite = ResourceFolderCardTextureProvider.GetCardTexture(Rank, Suit);
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