using Assets.Scripts.Card;

namespace Assets.Scripts.Initialization
{
    public interface ICardFactory
    {
        ICard GetCard(Suit suit, Rank rank);
    }
}