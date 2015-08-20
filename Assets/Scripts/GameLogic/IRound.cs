using Assets.Scripts.Card;
using Assets.Scripts.Player;

namespace Assets.Scripts.GameLogic
{
    public interface IRound
    {
        bool IsRoundFinished();
        void Tick();
        IPlayer GetWinner();
        bool HaveCardsBeenPlayed();
        Suit GetPlayedSuit();
        int GetWinnersPoints();
    }
}