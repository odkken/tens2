namespace Assets.Scripts.Card
{
    public enum Suit
    {
        Diamonds,
        Clubs,
        Hearts,
        Spades
    }

    public enum Rank
    {
        Two,
        Three,
        Four,
        Five,
        Six,
        Seven,
        Eight,
        Nine,
        Ten,
        Jack,
        Queen,
        King,
        Ace
    }

    public struct CardInfo
    {
        public Suit Suit;
        public Rank Rank;
    }

    public interface ICard
    {
        Suit Suit { get; }
        Rank Rank { get; }
        IMovable Movable { get; }
    }
}