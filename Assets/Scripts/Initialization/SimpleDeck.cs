using System;
using System.Collections.Generic;
using Assets.Scripts.Card;
using Assets.Scripts.Misc;

namespace Assets.Scripts.Initialization
{
    internal class SimpleDeck : IDeck
    {
        private Queue<ICard> _cards;

        public SimpleDeck(IEnumerable<ICard> cards)
        {
            _cards = new Queue<ICard>(cards);
        }

        public void Shuffle(int seed)
        {
            _cards = new Queue<ICard>(_cards.Shuffle(new Random(seed)));
        }

        public int NumCards
        {
            get { return _cards.Count; }
        }

        public ICard PopCard()
        {
            return _cards.Dequeue();
        }
    }
}