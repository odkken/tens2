using Assets.Scripts.Card;
using UnityEngine;

namespace Assets.Scripts.Initialization
{
    public class MBCardFactory : ICardFactory
    {
        private readonly GameObject _cardPrefab;
        private readonly ICardFactory _realCardFactory;
        private readonly ICardTextureProvider _textureProvider;

        public MBCardFactory(GameObject cardPrefab, ICardFactory realCardFactory, ICardTextureProvider textureProvider)
        {
            _cardPrefab = cardPrefab;
            _realCardFactory = realCardFactory;
            _textureProvider = textureProvider;
        }

        public ICard GetCard(Suit suit, Rank rank)
        {
            var card = Object.Instantiate(_cardPrefab).GetComponent<MBCard>();
            card.Initialize(_realCardFactory.GetCard(suit, rank), _textureProvider);
            return card;
        }
    }
}