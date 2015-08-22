using Assets.Scripts.Card;
using UnityEngine;
using UnityEngine.Networking;

namespace Assets.Scripts.Initialization
{
    public class MBCardFactory : ICardFactory
    {
        private readonly GameObject _cardPrefab;
        private readonly bool _networked;

        public MBCardFactory(GameObject cardPrefab, bool networked = true)
        {
            _cardPrefab = cardPrefab;
            _networked = networked;
        }

        public ICard GetCard(Suit suit, Rank rank)
        {
            var card = Object.Instantiate(_cardPrefab);
            NetworkServer.Spawn(card);
            if (_networked)
            {
                card.GetComponent<MBCard>().Initialize(rank, suit);
                return card.GetComponent<MBCard>();
            }
            else
            {
                card.GetComponent<NonNetworkedMBCard>().Initialize(rank, suit);
                return card.GetComponent<NonNetworkedMBCard>();
            }

        }
    }
}