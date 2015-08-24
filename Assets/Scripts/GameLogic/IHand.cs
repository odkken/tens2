using Assets.Scripts.Card;
using Assets.Scripts.Player;

namespace Assets.Scripts.GameLogic
{
    public interface IHand
    {
        bool IsTrumpDetermined { get; }
        Suit TrumpSuit { get; }
        void Tick();
        IPlayer GetBidHolder();
        bool IsHandFinished();
        /// <summary>
        /// During the hand, this returns the player's points accumulated in the hand.  After the hand, bids and previous points are factored in and this contains the new total score for the player.
        /// </summary>
        /// <param name="playerId"></param>
        /// <returns></returns>
        int GetPointsForTeam(int playerId);
    }
}