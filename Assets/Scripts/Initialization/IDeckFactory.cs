using System.Collections.Generic;
using Assets.Scripts.Card;

namespace Assets.Scripts.Initialization
{
    public interface IDeckFactory
    {
        IDeck GetDeck(List<Suit> excludedSuits, List<Rank> excludedRanks, List<int> shuffleSeeds);
        IDeck GetTensDeck(List<int> shuffleSeeds);
    }
}