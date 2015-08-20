﻿namespace Assets.Scripts.Card
{
    public class SimpleCard : ICard
    {
        public SimpleCard(Suit suit, Rank rank, IMovable movable)
        {
            Movable = movable;
            Suit = suit;
            Rank = rank;
        }

        public Suit Suit { get; set; }
        public Rank Rank { get; set; }
        public IMovable Movable { get; private set; }

        public override string ToString()
        {
            return Rank + " Of " + Suit;
        }
    }
}