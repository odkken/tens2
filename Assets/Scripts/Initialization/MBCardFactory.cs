using Assets.Scripts.Card;
using UnityEngine;
using UnityEngine.Networking;

namespace Assets.Scripts.Initialization
{
    public class MBCardFactory : ICardFactory
    {
        private readonly GameObject _cardPrefab;

        public MBCardFactory(GameObject cardPrefab)
        {
            _cardPrefab = cardPrefab;
        }

        public ICard GetCard(Suit suit, Rank rank)
        {
            var card = Object.Instantiate(_cardPrefab);
            NetworkServer.Spawn(card);
            card.GetComponent<MBCard>().Initialize(rank, suit);
            return card.GetComponent<MBCard>();
        }
    }
}