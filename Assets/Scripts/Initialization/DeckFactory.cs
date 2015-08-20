using System;
using System.Collections.Generic;
using Assets.Scripts.Card;

namespace Assets.Scripts.Initialization
{
    public class DeckFactory : IDeckFactory
    {
        private readonly ICardFactory _cardFactory;

        public DeckFactory(ICardFactory cardFactory)
        {
            _cardFactory = cardFactory;
        }

        public IDeck GetDeck(List<Suit> excludedSuits = null, List<Rank> excludedRanks = null,
            List<int> shuffleSeeds = null)
        {
            var cards = new List<ICard>();
            foreach (Suit suit in Enum.GetValues(typeof(Suit)))
            {
                if (excludedSuits == null || !excludedSuits.Contains(suit))
                    foreach (Rank rank in Enum.GetValues(typeof(Rank)))
                    {
                        if (excludedRanks == null || !excludedRanks.Contains(rank))
                        {
                            cards.Add(_cardFactory.GetCard(suit, rank));
                        }
                    }
            }
            var deck = new SimpleDeck(cards);
            if (shuffleSeeds == null)
            {
                deck.Shuffle(0);
            }
            else
                foreach (var shuffleSeed in shuffleSeeds)
                {
                    deck.Shuffle(shuffleSeed);
                }
            return deck;
        }

        public IDeck GetTensDeck(List<int> shuffleSeeds)
        {
            return GetDeck(excludedRanks: new List<Rank> { Rank.Two, Rank.Three, Rank.Four }, shuffleSeeds: shuffleSeeds);
        }
    }
}