using Assets.Scripts.Card;

namespace Assets.Scripts.Initialization
{
    public class SimpleCardFactory : ICardFactory
    {
        public ICard GetCard(Suit suit, Rank rank)
        {
            return new SimpleCard(suit, rank, new DummyMovable());
        }
    }
}