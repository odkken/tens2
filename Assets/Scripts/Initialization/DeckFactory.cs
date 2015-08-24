using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Card;

namespace Assets.Scripts.Initialization
{
    public class DeckFactory : IDeckFactory
    {
        private readonly ICardFactory _cardFactory;
        private readonly List<ICard> _cachedCards = new List<ICard>();
        public DeckFactory(ICardFactory cardFactory)
        {
            _cardFactory = cardFactory;
        }

        public IDeck GetDeck(List<Suit> excludedSuits = null, List<Rank> excludedRanks = null,
            List<int> shuffleSeeds = null)
        {
            foreach (Suit suit in Enum.GetValues(typeof(Suit)))
            {
                if (excludedSuits == null || !excludedSuits.Contains(suit))
                    foreach (Rank rank in Enum.GetValues(typeof(Rank)))
                    {
                        if (excludedRanks == null || !excludedRanks.Contains(rank))
                        {
                            if (!_cachedCards.Any(a => a.Suit == suit && a.Rank == rank))
                                _cachedCards.Add(_cardFactory.GetCard(suit, rank));
                        }
                    }
            }
            var deck = new SimpleDeck(_cachedCards.Where(a => (excludedSuits == null || !excludedSuits.Contains(a.Suit)) && (excludedRanks == null || !excludedRanks.Contains(a.Rank))));
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