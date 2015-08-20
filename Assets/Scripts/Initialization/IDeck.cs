using Assets.Scripts.Card;

namespace Assets.Scripts.Initialization
{
    public interface IDeck
    {
        int NumCards { get; }
        void Shuffle(int seed);
        ICard PopCard();
    }
}